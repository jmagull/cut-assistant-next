using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutAssistantNext.App.Naming;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.App.ViewModels;

internal sealed class NamingSettingsViewModel :
    INotifyPropertyChanged
{
    private readonly NameTemplateContext _nameContext;

    private readonly string _initialDefaultNameTemplate;

    private string _defaultNameTemplate;

    internal NamingSettingsViewModel(
        NamingSettings settings,
        NameTemplateContext nameContext)
    {
        ArgumentNullException.ThrowIfNull(
            settings);

        ArgumentNullException.ThrowIfNull(
            nameContext);

        _nameContext =
            nameContext;

        _defaultNameTemplate =
            settings.DefaultNameTemplate;

        _initialDefaultNameTemplate =
            settings.DefaultNameTemplate;

        OriginalFileName =
            nameContext.OriginalName
            ?? string.Empty;

    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string DefaultNameTemplate
    {
        get => _defaultNameTemplate;
        set
        {
            if (_defaultNameTemplate == value)
            {
                return;
            }

            _defaultNameTemplate =
                value;

            OnPropertyChanged();

            OnPropertyChanged(
                nameof(Preview));

            OnPropertyChanged(
                nameof(TemplateBlocks));
        }
    }

    public string OriginalFileName { get; }

    public string Preview =>
        NameTemplateRenderer.Render(
            _defaultNameTemplate,
            _nameContext);

    public IReadOnlyList<NameTemplateBlock> TemplateBlocks =>
        NameTemplateBlockParser.Parse(
            _defaultNameTemplate);

    internal int InsertTemplateElement(
        string templateElement,
        int selectionStart,
        int selectionLength)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            templateElement);

        var start =
            Math.Clamp(
                selectionStart,
                0,
                _defaultNameTemplate.Length);

        var length =
            Math.Clamp(
                selectionLength,
                0,
                _defaultNameTemplate.Length - start);

        DefaultNameTemplate =
            _defaultNameTemplate
                .Remove(
                    start,
                    length)
                .Insert(
                    start,
                    templateElement);

        return start + templateElement.Length;
    }

    internal bool RemoveTemplateBlockAt(
        int blockIndex)
    {
        var blocks =
            NameTemplateBlockParser.Parse(
                _defaultNameTemplate)
                .ToList();

        if (blockIndex < 0 ||
            blockIndex >= blocks.Count)
        {
            return false;
        }

        blocks.RemoveAt(
            blockIndex);

        DefaultNameTemplate =
            NameTemplateBlockParser.Render(
                blocks);

        return true;
    }

    internal int MoveTemplateBlock(
        int sourceIndex,
        int insertionIndex)
    {
        var blocks =
            NameTemplateBlockParser.Parse(
                _defaultNameTemplate)
                .ToList();

        if (sourceIndex < 0 ||
            sourceIndex >= blocks.Count)
        {
            return -1;
        }

        if (insertionIndex < 0 ||
            insertionIndex > blocks.Count)
        {
            return -1;
        }

        var block =
            blocks[sourceIndex];

        blocks.RemoveAt(
            sourceIndex);

        var targetIndex =
            insertionIndex;

        if (insertionIndex > sourceIndex)
        {
            targetIndex--;
        }

        blocks.Insert(
            targetIndex,
            block);

        DefaultNameTemplate =
            NameTemplateBlockParser.Render(
                blocks);

        return targetIndex;
    }

    internal int InsertTemplateBlock(
        string templateElement,
        int insertionIndex)
    {
        ArgumentException.ThrowIfNullOrEmpty(
            templateElement);

        var blocks =
            NameTemplateBlockParser.Parse(
                _defaultNameTemplate)
                .ToList();

        if (insertionIndex < 0 ||
            insertionIndex > blocks.Count)
        {
            return -1;
        }

        var newBlock =
            NameTemplateBlockParser.Parse(
                templateElement)
                .Single();

        blocks.Insert(
            insertionIndex,
            newBlock);

        DefaultNameTemplate =
            NameTemplateBlockParser.Render(
                blocks);

        return insertionIndex;
    }

    internal void ResetDefaultNameTemplate()
    {
        DefaultNameTemplate =
            _initialDefaultNameTemplate;
    }

    internal void RestoreCanDefaultNameTemplate()
    {
        DefaultNameTemplate =
            NamingSettings.CreateDefault().DefaultNameTemplate;
    }

    internal NamingSettings CreateSettings()
    {
        return new NamingSettings
        {
            DefaultNameTemplate =
                DefaultNameTemplate
        };
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
