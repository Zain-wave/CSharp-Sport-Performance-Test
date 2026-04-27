using Microsoft.AspNetCore.Mvc;
using SportsSebastianVargas.Models;
using SportsSebastianVargas.Services;
using SportsSebastianVargas.ViewModels;

namespace SportsSebastianVargas.Controllers;

public class EspacioDeportivoController : Controller
{
    private readonly EspacioDeportivoService _espacioService;

    public EspacioDeportivoController(EspacioDeportivoService espacioService) { _espacioService = espacioService; }

    public async Task<IActionResult> Index(string? tipo = null)
    {
        var response = await _espacioService.GetEspacios(tipo);
        return View(new EspacioDeportivoViewModel { EspacioList = response.Data ?? new List<EspacioDeportivo>() });
    }

    [HttpPost]
    public async Task<IActionResult> Create(EspacioDeportivo espacio)
    {
        var response = await _espacioService.CreateEspacio(espacio);
        if (!response.Success) TempData["Mensaje"] = response.Message;
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EspacioDeportivo espacio)
    {
        var response = await _espacioService.UpdateEspacio(espacio);
        if (!response.Success) TempData["Mensaje"] = response.Message;
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _espacioService.DeleteEspacio(id);
        if (!response.Success) TempData["Mensaje"] = response.Message;
        return RedirectToAction("Index");
    }
}