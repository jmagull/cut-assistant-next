"""Read-only motor diagnosis on new synthetic media; no real-film writes."""
import argparse, hashlib, json, pathlib, subprocess, sys
p=argparse.ArgumentParser()
for n in ('ffmpeg','ffprobe','ffmsindex','engine','work'): p.add_argument('--'+n,required=True)
p.add_argument('--fps',type=int,choices=(25,50),default=50)
a=p.parse_args(); w=pathlib.Path(a.work).resolve(); w.mkdir(parents=True,exist_ok=False)
records=[]
def run(name,args,expected=0):
    r=subprocess.run([str(x) for x in args],capture_output=True,text=True,encoding='utf-8',errors='replace')
    (w/(name+'.stdout.txt')).write_text(r.stdout,encoding='utf-8'); (w/(name+'.stderr.txt')).write_text(r.stderr,encoding='utf-8')
    (w/(name+'.command.json')).write_text(json.dumps([str(x) for x in args],indent=2),encoding='utf-8')
    if expected is not None: assert r.returncode==expected,(name,r.returncode,r.stderr[-2000:])
    return r
video=w/'video.mp4'
run('generate-video',[a.ffmpeg,'-v','error','-f','lavfi','-i',f'testsrc2=size=160x90:rate={a.fps}:duration=12','-an','-c:v','libx264','-preset','ultrafast','-pix_fmt','yuv420p','-g',str(a.fps),'-keyint_min',str(a.fps),'-sc_threshold','0','-bf','2',video])
for codec in ('aac','eac3'):
    run('generate-'+codec,[a.ffmpeg,'-v','error','-i',video,'-f','lavfi','-i','sine=frequency=440:sample_rate=48000:duration=12','-map','0:v:0','-map','1:a:0','-c:v','copy','-c:a',codec,'-b:a','192k','-ac','2','-metadata:s:a:0','language=deu',w/(codec+'.mp4')])
run('generate-mixed',[a.ffmpeg,'-v','error','-i',w/'aac.mp4','-i',w/'eac3.mp4','-map','0:v:0','-map','0:a:0','-map','1:a:0','-c','copy',w/'mixed.mp4'])
# Shift EAC3 packet timestamps by 64 ms after four seconds without altering payloads.
expr=r'PTS+if(gte(PTS*TB\,4)\,0.064/TB\,0)'
run('generate-eac3-gap',[a.ffmpeg,'-v','error','-copyts','-i',w/'eac3.mp4','-map','0','-c','copy','-bsf:a','setts=pts='+expr+':dts='+expr,'-avoid_negative_ts','disabled',w/'eac3-gap.mp4'])
for stem in ('aac','eac3','mixed','eac3-gap'):
    probe=run('probe-'+stem,[a.ffprobe,'-v','error','-show_streams','-show_format','-of','json',w/(stem+'.mp4')])
    (w/(stem+'.metadata.json')).write_text(probe.stdout,encoding='utf-8')
    frame=run('frames-'+stem,[a.ffprobe,'-v','error','-select_streams','a','-show_frames','-show_entries','frame=stream_index,pts,nb_samples','-of','json',w/(stem+'.mp4')])
    (w/(stem+'.frames.json')).write_text(frame.stdout,encoding='utf-8')
    streams=json.loads(probe.stdout)['streams']; frames=json.loads(frame.stdout)['frames']; checks=[]
    for stream in streams:
        if stream.get('codec_type')!='audio': continue
        rows=[f for f in frames if f.get('stream_index')==stream['index']]
        assert rows and all('pts' in f and 'nb_samples' in f for f in rows)
        deltas=[y['pts']-(x['pts']+x['nb_samples']) for x,y in zip(rows,rows[1:])]
        assert all(y['pts']>x['pts'] for x,y in zip(rows,rows[1:]))
        if stem=='eac3': assert all(d==0 for d in deltas)
        if stem=='eac3-gap': assert any(d==3072 for d in deltas),set(deltas)
        checks.append({'stream':stream['index'],'codec':stream['codec_name'],'frames':len(rows),'nonzero_sample_deltas':[d for d in deltas if d]})
    records.append({'name':'timestamps-'+stem,'passed':True,'streams':checks})
run('standalone-eac3-decode',[a.ffmpeg,'-v','error','-xerror','-i',w/'eac3.mp4','-map','0:a:0','-c:a','pcm_f32le','-f','f32le',w/'eac3-decoded.raw'])
samples=(w/'eac3-decoded.raw').stat().st_size//8
assert samples==576000,samples
records.append({'name':'standalone-eac3-decode','passed':True,'samples_per_channel':samples})
for stem,variant in [('eac3','on'),('eac3','off'),('eac3','whole'),('mixed','on'),('eac3-gap','on'),('aac','on'),('aac','off')]:
    name=stem+'-'+variant; work=w/(name+'-work'); work.mkdir(); index=work/'index.ffindex'
    run(name+'-index',[a.ffmsindex,'-f','-c','-k',w/(stem+'.mp4'),index])
    ranges=[(i,i+1+(1/a.fps if variant=='off' else 0)) for i in (1,3,5,7,9)] if variant!='whole' else [(0,12)]
    intervals='times:'+''.join('[00:00:%09.6f,00:00:%09.6f]'%(s,e) for s,e in ranges)
    output=w/(name+'-output.mp4')
    r=run(name+'-cut',[a.engine,'cut-can','-v','--input',w/(stem+'.mp4'),'--output',output,'--index',index,'--temp-dir',work,'--cutlist',intervals,'--ffmpeg',a.ffmpeg,'--ffprobe',a.ffprobe],None)
    log=r.stdout+r.stderr
    if stem!='aac':
        assert r.returncode!=0 and 'Unsupported audio codec eac3' in log and '[audio-processing-failure-v1]' in log,(name,r.returncode,log[-2000:])
        assert not output.exists() and 'Decoding original audio once' not in log and 'Stage 3/3' not in log
    else:
        assert r.returncode==0 and output.is_file(),(name,r.returncode,log[-2000:])
        assert 'Verified audio' in log
    records.append({'name':name,'passed':True,'expected':'codec rejection' if stem!='aac' else 'successful strict cut','exit_code':r.returncode,'keeps':ranges,'output_exists':output.exists()})
result={'passed':True,'fps':a.fps,'count':len(records),'cases':records,'engine_sha256':hashlib.sha256(pathlib.Path(a.engine).read_bytes()).hexdigest(),'source_hashes':{n:hashlib.sha256((w/n).read_bytes()).hexdigest() for n in ['aac.mp4','eac3.mp4','mixed.mp4','eac3-gap.mp4']}}
(w/'results.json').write_text(json.dumps(result,indent=2),encoding='utf-8'); print(json.dumps(result,indent=2))
