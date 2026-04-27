using Microsoft.AspNetCore.Mvc;
using SportsSebastianVargas.Models;
using SportsSebastianVargas.Services;
using SportsSebastianVargas.ViewModels;

namespace SportsSebastianVargas.Controllers;

public class ReservaController : Controller
{
    private readonly ReservaService _reservaService;

    public ReservaController(ReservaService reservaService) { _reservaService = reservaService; }

    public async Task<IActionResult> Index()
    {
        var response = await _reservaService.GetReservas();
        return View(new ReservaViewModel { ReservaList = response.Data ?? new List<Reserva>() });
    }

    [HttpPost]
    public async Task<IActionResult> Create(Reserva reserva)
    {
        var response = await _reservaService.CreateReserva(reserva);
        if (!response.Success) TempData["Mensaje"] = response.Message;
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var response = await _reservaService.CancelarReserva(id);
        if (!response.Success) TempData["Mensaje"] = response.Message;
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Finish(int id)
    {
        var response = await _reservaService.FinalizarReserva(id);
        if (!response.Success) TempData["Mensaje"] = response.Message;
        return RedirectToAction("Index");
    }
}