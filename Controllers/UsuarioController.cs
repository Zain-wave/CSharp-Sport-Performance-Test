using Microsoft.AspNetCore.Mvc;
using SportsSebastianVargas.Models;
using SportsSebastianVargas.Services;
using SportsSebastianVargas.ViewModels;

namespace SportsSebastianVargas.Controllers;

public class UsuarioController : Controller
{
    private readonly UsuarioService _usuarioService;

    public UsuarioController(UsuarioService usuarioService) { _usuarioService = usuarioService; }

    public async Task<IActionResult> Index(string? estado = null)
    {
        var response = await _usuarioService.GetUsuarios(estado);
        return View(new UsuarioViewModel { UsuarioList = response.Data ?? new List<Usuario>() });
    }

    [HttpPost]
    public async Task<IActionResult> Create(Usuario usuario)
    {
        var response = await _usuarioService.CreateUsuario(usuario);
        if (!response.Success) TempData["Mensaje"] = response.Message;
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Usuario usuario)
    {
        var response = await _usuarioService.UpdateUsuario(usuario);
        if (!response.Success) TempData["Mensaje"] = response.Message;
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _usuarioService.DeleteUsuario(id);
        if (!response.Success) TempData["Mensaje"] = response.Message;
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Toggle(int id)
    {
        var response = await _usuarioService.ToggleEstado(id);
        if (!response.Success) TempData["Mensaje"] = response.Message;
        return RedirectToAction("Index");
    }
}