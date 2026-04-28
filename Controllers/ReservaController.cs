using Microsoft.AspNetCore.Mvc;
using SportsSebastianVargas.Models;
using SportsSebastianVargas.Services;
using SportsSebastianVargas.ViewModels;

namespace SportsSebastianVargas.Controllers;

public class ReservaController : Controller
{
    private readonly ReservaService _reservaService;
    private readonly UsuarioService _usuarioService;
    private readonly EspacioDeportivoService _espacioService;

    public ReservaController(ReservaService reservaService, UsuarioService usuarioService, EspacioDeportivoService espacioService)
    {
        _reservaService = reservaService;
        _usuarioService = usuarioService;
        _espacioService = espacioService;
    }

    public async Task<IActionResult> Index(string? usuarioId = null, string? espacioId = null)
    {
        var reservas = await _reservaService.GetReservas(usuarioId, espacioId);
        var usuarios = await _usuarioService.GetUsuarios();
        var espacios = await _espacioService.GetEspacios();
        return View(new ReservaViewModel
        {
            ReservaList = reservas.Data ?? new List<Reserva>(),
            UsuarioList = usuarios.Data ?? new List<Usuario>(),
            EspacioList = espacios.Data ?? new List<EspacioDeportivo>()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReservaCreateViewModel model)
    {
        try
        {
            if (!DateTime.TryParse(model.Fecha, out var fecha))
            {
                TempData["Mensaje"] = "Fecha inválida";
                return RedirectToAction("Index");
            }

            if (!TimeSpan.TryParse(model.HoraInicio, out var horaInicio))
            {
                TempData["Mensaje"] = "Hora de inicio inválida";
                return RedirectToAction("Index");
            }

            if (!TimeSpan.TryParse(model.HoraFin, out var horaFin))
            {
                TempData["Mensaje"] = "Hora de fin inválida";
                return RedirectToAction("Index");
            }

            fecha = DateTime.SpecifyKind(fecha, DateTimeKind.Utc);

            var reserva = new Reserva
            {
                UsuarioId = model.UsuarioId,
                EspacioDeportivoId = model.EspacioDeportivoId,
                Fecha = fecha,
                HoraInicio = horaInicio,
                HoraFin = horaFin
            };

            var response = await _reservaService.CreateReserva(reserva);
            if (!response.Success) TempData["Mensaje"] = response.Message;
        }
        catch (Exception ex)
        {
            TempData["Mensaje"] = $"Error al crear reserva: {ex.Message}";
        }

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