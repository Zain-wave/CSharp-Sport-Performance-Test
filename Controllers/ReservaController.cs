using Microsoft.AspNetCore.Mvc;
using SportsSebastianVargas.Models;
using SportsSebastianVargas.Services;
using SportsSebastianVargas.ViewModels;

namespace SportsSebastianVargas.Controllers;

public class ReservaController : Controller
{
    private readonly ReservaService _reservaService;
    private readonly UsuarioService _usuarioService;

    public ReservaController(ReservaService reservaService, UsuarioService usuarioService)
    {
        _reservaService = reservaService;
        _usuarioService = usuarioService;
    }

    public async Task<IActionResult> Index()
    {
        var reservas = await _reservaService.GetReservas();
        var usuarios = await _usuarioService.GetUsuarios();
        return View(new ReservaViewModel
        {
            ReservaList = reservas.Data ?? new List<Reserva>(),
            UsuarioList = usuarios.Data ?? new List<Usuario>()
        });
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