using Microsoft.AspNetCore.Mvc;
using SportsSebastianVargas.Services;
using SportsSebastianVargas.ViewModels;

namespace SportsSebastianVargas.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}