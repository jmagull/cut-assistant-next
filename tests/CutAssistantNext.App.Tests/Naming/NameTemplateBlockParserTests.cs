using CutAssistantNext.App.Naming;

namespace CutAssistantNext.App.Tests.Naming;

public sealed class NameTemplateBlockParserTests
{
    [Fact]
    public void ParseAndRender_PreservesTemplateAsBlocks()
    {
        const string template =
            "%Name% [%Tag%.%Monat%.%YY%]";

        var blocks =
            NameTemplateBlockParser.Parse(
                template);

        Assert.Equal(
            [
                "%Name%",
                " ",
                "[",
                "%Tag%",
                ".",
                "%Monat%",
                ".",
                "%YY%",
                "]"
            ],
            blocks.Select(
                block => block.Value));

        Assert.Equal(
            [
                NameTemplateBlockKind.Token,
                NameTemplateBlockKind.Literal,
                NameTemplateBlockKind.Literal,
                NameTemplateBlockKind.Token,
                NameTemplateBlockKind.Literal,
                NameTemplateBlockKind.Token,
                NameTemplateBlockKind.Literal,
                NameTemplateBlockKind.Token,
                NameTemplateBlockKind.Literal
            ],
            blocks.Select(
                block => block.Kind));

        Assert.Equal(
            template,
            NameTemplateBlockParser.Render(
                blocks));
    }
    [Fact]
    public void ParseAndRender_PreservesFreeTextAsSingleBlock()
    {
        const string template =
            "%Name% alles in Hoth is cool";

        var blocks =
            NameTemplateBlockParser.Parse(
                template);

        Assert.Equal(
            2,
            blocks.Count);

        Assert.Equal(
            "%Name%",
            blocks[0].Value);

        Assert.Equal(
            " alles in Hoth is cool",
            blocks[1].Value);

        Assert.Equal(
            NameTemplateBlockKind.Literal,
            blocks[1].Kind);

        Assert.Equal(
            template,
            NameTemplateBlockParser.Render(
                blocks));
    }
    [Fact]
    public void Parse_ProvidesFriendlyDisplayTextForBlocks()
    {
        const string template =
            "%Name% %Staffel:S%%Folge:E% [%YY%]";

        var blocks =
            NameTemplateBlockParser.Parse(
                template);

        Assert.Equal(
            [
                "Name",
                "Leerzeichen",
                "Staffel (S)",
                "Folge (E)",
                "Leerzeichen",
                "[",
                "Jahr (2-stellig)",
                "]"
            ],
            blocks.Select(
                block => block.DisplayText));
    }
}
