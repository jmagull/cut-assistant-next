namespace CutAssistantNext.Core.Tests;

public class ApplicationInfoTests
{
    [Fact]
    public void ProductName_IdentifiesApplication()
    {
        Assert.Equal("Cut Assistant Next", ApplicationInfo.ProductName);
    }
}
