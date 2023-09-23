using GenericClassLocalization.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace GenericClassLocalization.Controllers;

[ApiController]
[Route("[controller]")]
public class LocalizationController : Controller
{
    private readonly IStringLocalizer<MyClass<int, string>> _myClassLocalizer;
    private readonly IStringLocalizer<MyClass<int, double>.MySubClass<string, int>> _mySubClassLocalizer;

    public LocalizationController(IStringLocalizer<MyClass<int, string>> myClassLocalizer,
        IStringLocalizer<MyClass<int, double>.MySubClass<string, int>> mySubClassLocalizer)
    {
        _myClassLocalizer = myClassLocalizer;
        _mySubClassLocalizer = mySubClassLocalizer;
    }

    [HttpGet(nameof(HelloWorld))]
    public IActionResult HelloWorld()
    {
        return Ok(new Messages
        {
            MyClassMessage = _myClassLocalizer["HelloWorld"],
            MySubClassMessage = _mySubClassLocalizer["HelloWorld"]
        });
    }

    private class Messages
    {
        public string MyClassMessage { get; set; } = default!;
        public string MySubClassMessage { get; set; } = default!;
    }
}