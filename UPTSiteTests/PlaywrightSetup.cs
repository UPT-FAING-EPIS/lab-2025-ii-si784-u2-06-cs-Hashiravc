using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UPTSiteTests;

[TestClass]
public class PlaywrightSetup
{
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext context)
    {
        var exitCode = Program.Main(new[] { "install" });
        if (exitCode != 0)
        {
            Assert.Fail($"Playwright browser installation failed with exit code {exitCode}.");
        }
    }
}
