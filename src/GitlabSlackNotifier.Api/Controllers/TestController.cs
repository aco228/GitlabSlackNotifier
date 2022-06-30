using Microsoft.AspNetCore.Mvc;

namespace GitlabSlackNotifier.Api.Controllers;

public class TestController : ControllerBase
{
    public IActionResult Index()
    {
        var val = Environment.GetEnvironmentVariable("APP_SETTINGS");
        return Content(val ?? "nothing");
    }
}