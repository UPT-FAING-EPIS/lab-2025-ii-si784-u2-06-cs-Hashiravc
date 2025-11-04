using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UPTSiteTests;

[TestClass]
public class UPTSiteTest : PageTest
{
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            RecordVideoDir = "videos",
            RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
        };
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        await Context.Tracing.StartAsync(new()
        {
            Title = $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}",
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        await Context.Tracing.StopAsync(new()
        {
            Path = Path.Combine(
                Environment.CurrentDirectory,
                "playwright-traces",
                $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}.zip"
            )
        });
        await Context.CloseAsync();
    }

    [TestMethod]
    public async Task HasTitle()
    {
        await Page.GotoAsync("https://www.upt.edu.pe");

        await Expect(Page).ToHaveTitleAsync(new Regex("Universidad"));
    }

    [TestMethod]
    public async Task GetSchoolDirectorName()
    {
        string schoolDirectorName = "Ing. Martha Judith Paredes Vignola";
    await Page.GotoAsync("https://www.upt.edu.pe");
    await CloseLandingModalAsync();

    await Page.GotoAsync("https://www.upt.edu.pe/upt/web/facultad/index/188");
    await Page.GetByRole(AriaRole.Link, new() { Name = "Escuela Profesional de" }).ClickAsync(new() { Force = true });
    await Page.GetByRole(AriaRole.Link, new() { Name = "Plana Docente" }).ClickAsync(new() { Force = true });

        await Expect(Page.GetByText("Ing. Martha Judith Paredes")).ToContainTextAsync(schoolDirectorName);
    }

    [TestMethod]
    public async Task SearchStudentInDirectoryPage()
    {
        string studentName = "AYMA CHOQUE, ERICK YOEL";
        string studentSearch = studentName.Split(" ")[0];
    await Page.GotoAsync("https://www.upt.edu.pe");
    await CloseLandingModalAsync();

    await Page.GotoAsync("https://www.upt.edu.pe/upt/web/facultad/index/188");
    await Page.GetByRole(AriaRole.Link, new() { Name = "Estudiantes" }).ClickAsync(new() { Force = true });
        var directoryFrame = Page.FrameLocator("iframe");
        await directoryFrame.GetByRole(AriaRole.Textbox).ClickAsync();
        await directoryFrame.GetByRole(AriaRole.Textbox).FillAsync(studentSearch);
        await directoryFrame.GetByRole(AriaRole.Button, new() { Name = "Buscar" }).ClickAsync();
        await directoryFrame.GetByRole(AriaRole.Link, new() { Name = "CICLO - VII", Exact = true }).ClickAsync();

        await Expect(directoryFrame.GetByRole(AriaRole.Table)).ToContainTextAsync("ALUMNO");
    }

    [TestMethod]
    public async Task VerifyAdmissionsInformation()
    {
        await Page.GotoAsync("https://www.upt.edu.pe");

        await CloseLandingModalAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Admisión", Exact = true }).First.ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { NameRegex = new Regex("Admisión", RegexOptions.IgnoreCase) })).ToBeVisibleAsync();
        StringAssert.Contains(Page.Url, "/contenido/102");
    }

    [TestMethod]
    public async Task VerifyFooterContactInfo()
    {
        await Page.GotoAsync("https://www.upt.edu.pe");

        await CloseLandingModalAsync();
        await Page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
        var pageText = await Page.EvaluateAsync<string>("() => document.body.innerText");

        Assert.IsTrue(pageText.Contains("Tacna", StringComparison.OrdinalIgnoreCase));
    }

    private async Task CloseLandingModalAsync()
    {
        var closeButton = Page.GetByRole(AriaRole.Button, new() { Name = "×" });
        if (await closeButton.CountAsync() > 0 && await closeButton.IsVisibleAsync())
        {
            await closeButton.ClickAsync();
        }
    }
}
