"""Synthetic media only. All outputs belong in an ignored, new working directory."""
import argparse
import hashlib
import json
import pathlib
import struct
import subprocess

parser = argparse.ArgumentParser()
for name in ("work", "harness", "engine", "indexer", "ffmpeg", "ffprobe", "mp4box"):
    parser.add_argument("--" + name, type=pathlib.Path, required=True)
parser.add_argument("--legacy-fixtures", type=pathlib.Path)
args = parser.parse_args()
for name, value in vars(args).items():
    if isinstance(value, pathlib.Path):
        setattr(args, name, value.resolve())
args.work.mkdir(parents=True, exist_ok=False)


def run(command, log):
    result = subprocess.run([str(x) for x in command], capture_output=True, timeout=180)
    log.write_bytes(result.stdout + result.stderr)
    if result.returncode:
        raise RuntimeError(f"Exit {result.returncode}: {command[0]}\n{(result.stdout + result.stderr)[-3500:].decode('utf-8', errors='replace')}")
    return result


def boxes(data):
    offset = 0
    while offset < len(data):
        size, kind = struct.unpack_from(">I4s", data, offset)
        assert 8 <= size <= len(data) - offset
        yield kind, data[offset + 8:offset + size]
        offset += size
    assert offset == len(data)


def box(kind, data):
    return struct.pack(">I4s", len(data) + 8, kind) + data


def patch(source, target, changes, update_metadata=True):
    top = list(boxes(source.read_bytes()))
    assert top[-1][0] == b"moov"  # mdat offsets stay unchanged
    track = -1
    total = sum(changes.values())

    def edit(kind, payload):
        nonlocal track
        if kind == b"trak":
            track += 1
        if kind == b"stts" and track == 1:
            version, count = struct.unpack_from(">II", payload)
            durations = []
            for i in range(count):
                n, delta = struct.unpack_from(">II", payload, 8 + 8 * i)
                durations.extend([delta] * n)
            assert all(d == 1024 for d in durations), "Fixture must contain regular AAC packets"
            for i, delta in changes.items():
                durations[i] += delta
            rows = []
            for delta in durations:
                if rows and rows[-1][1] == delta:
                    rows[-1] = (rows[-1][0] + 1, delta)
                else:
                    rows.append((1, delta))
            payload = struct.pack(">II", version, len(rows)) + b"".join(struct.pack(">II", *r) for r in rows)
        elif update_metadata and track == 1 and kind in (b"mdhd", b"tkhd", b"elst"):
            b = bytearray(payload)
            assert b[0] == 0
            position = {b"mdhd": 16, b"tkhd": 20, b"elst": 8}[kind]
            if kind == b"mdhd":
                assert struct.unpack_from(">I", b, 12)[0] == 48000
            if kind == b"elst":
                assert struct.unpack_from(">I", b, 4)[0] == 1
            struct.pack_into(">I", b, position, struct.unpack_from(">I", b, position)[0] + total)
            payload = bytes(b)
        elif kind in (b"moov", b"trak", b"mdia", b"minf", b"stbl", b"edts"):
            payload = b"".join(edit(k, b) for k, b in boxes(payload))
        return box(kind, payload)

    target.write_bytes(b"".join(edit(k, b) for k, b in top))


clean = args.work / "regular.mp4"
run([args.ffmpeg, "-v", "warning", "-f", "lavfi", "-i", "testsrc2=size=160x96:rate=25:duration=9.6",
     "-f", "lavfi", "-i", "aevalsrc=0.12*sin(2*PI*(137*t+17*t*t))|0.13*sin(2*PI*(196*t+24*t*t)):s=48000:d=8.96:c=stereo",
     "-map", "0:v", "-map", "1:a", "-c:v", "libx264", "-threads", "1", "-g", "25", "-keyint_min", "25",
     "-sc_threshold", "0", "-c:a", "aac", "-b:a", "160k", "-movie_timescale", "48000", "-n", clean], args.work / "generate.log")
single = args.work / "single-gap.mp4"
multiple = args.work / "multiple-gaps.mp4"
unknown = args.work / "unknown-end.mp4"
exact_end = args.work / "exact-audio-end.mp4"
patch(clean, single, {47: 512})
patch(clean, multiple, {47: 512, 141: 512})
patch(clean, unknown, {47: 513, 141: -1})  # Mixed gap/overlap must not be classified as pure missing coverage.
patch(clean, exact_end, {i: 512 for i in range(10, 151, 10)})

cases = [
    ("regular", "healthy", clean, [[0, 4]], 0, 0),
    ("single-inside", "decline", single, [[0, 4]], 512, 0),
    ("multiple-inside", "decline", multiple, [[0, 4]], 1024, 0),
    ("multiple-keeps", "decline", multiple, [[0.8, 1.2], [2.8, 3.2]], 1024, 0),
    ("outside-keeps", "healthy", single, [[0, 0.8], [1.2, 2]], 0, 0),
    ("begin-after-gap", "healthy", single, [[1.04, 2]], 0, 0),
    ("exact-audio-end", "decline", exact_end, [[0, 9.12]], 7680, 0),
    ("short-audio", "decline", clean, [[0, 9.6]], 0, 30720),
    ("gap-and-short-audio", "decline", single, [[0, 9.6]], 512, 30208),
    ("unknown", "unknown", unknown, [[0, 4]], None, None),
    ("cancel", "cancel", single, [[0, 4]], 512, 0),
    ("mp4box-confirmed", "accept", single, [[0, 4]], 512, 0),
]
if args.legacy_fixtures:
    for name, mode in [("healthy", "healthy"), ("discarded", "aac"), ("decline", "aac-decline"), ("bad-aac", "bad-aac")]:
        cases.append(("legacy-" + name, mode, args.legacy_fixtures / name / (name + ".mp4"), [[1.12, 3.4], [10.04, 15.08]], None, None))

results = []
for name, mode, source, keeps, internal, tail in cases:
    job = args.work / name
    job.mkdir()
    (job / "plan.json").write_text(json.dumps(keeps), encoding="utf-8")
    run([args.harness, mode, source, job, args.engine, args.indexer, args.ffmpeg, args.ffprobe, args.mp4box], job / "native.log")
    result = json.loads((job / "results.json").read_text(encoding="utf-8"))
    if mode not in ("healthy", "aac", "aac-decline", "bad-aac", "unknown"):
        diagnostic = result["Diagnosis"]
        assert diagnostic["Classification"] == "source_coverage_gap", result
        actual_internal = sum(i["MissingSamples"] for i in diagnostic["Issues"] if i["Kind"] == "internal_gap")
        actual_tail = sum(i["MissingSamples"] for i in diagnostic["Issues"] if i["Kind"] == "audio_end")
        assert (actual_internal, actual_tail) == (internal, tail), result
    if mode == "healthy":
        # Independently decode the final track: native validation already checked its source selection.
        run([args.ffmpeg, "-v", "warning", "-xerror", "-i", result["Output"], "-map", "0:a", "-vn", "-f", "null", "-"], job / "decode.log")
        assert not (job / "decode.log").read_bytes()
    results.append({"Name": name, **result})
    print("PASS", name, flush=True)
(args.work / "all-results.json").write_text(json.dumps(results, indent=2), encoding="utf-8")
print(f"{len(results)} native CAN cases passed", flush=True)
