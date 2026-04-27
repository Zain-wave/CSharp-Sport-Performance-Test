using Microsoft.EntityFrameworkCore;
using SportsSebastianVargas.Data;
using SportsSebastianVargas.Models;
using SportsSebastianVargas.Models.Enums;
using SportsSebastianVargas.Response;

namespace SportsSebastianVargas.Services;

public class ReservaService
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public ReservaService(AppDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<ResponseService<List<Reserva>>> GetReservas()
    {
        var reservas = await _context.Reserva
            .Include(r => r.Usuario)
            .Include(r => r.EspacioDeportivo)
            .Where(r => r.Usuario.Activo && r.EspacioDeportivo.Activo)
            .ToListAsync();

        return new ResponseService<List<Reserva>>(
            reservas,
            reservas.Count > 0 ? "Reservas cargadas" : "No hay reservas",
            reservas.Count > 0);
    }

    public async Task<ResponseService<Reserva>> GetReserva(int id)
    {
        var reserva = await _context.Reserva
            .Include(r => r.Usuario)
            .Include(r => r.EspacioDeportivo)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reserva == null)
        {
            return new ResponseService<Reserva>(null, "Reserva no encontrada", false);
        }
        return new ResponseService<Reserva>(reserva, "Reserva encontrada", true);
    }

    public async Task<ResponseService<List<Reserva>>> GetReservasPorUsuario(int usuarioId)
    {
        var reservas = await _context.Reserva
            .Include(r => r.Usuario)
            .Include(r => r.EspacioDeportivo)
            .Where(r => r.UsuarioId == usuarioId && r.Usuario.Activo && r.EspacioDeportivo.Activo)
            .ToListAsync();

        return new ResponseService<List<Reserva>>(
            reservas,
            reservas.Count > 0 ? "Reservas cargadas" : "No hay reservas",
            reservas.Count > 0);
    }

    public async Task<ResponseService<List<Reserva>>> GetReservasPorEspacio(int espacioId)
    {
        var reservas = await _context.Reserva
            .Include(r => r.Usuario)
            .Include(r => r.EspacioDeportivo)
            .Where(r => r.EspacioDeportivoId == espacioId && r.Usuario.Activo && r.EspacioDeportivo.Activo)
            .ToListAsync();

        return new ResponseService<List<Reserva>>(
            reservas,
            reservas.Count > 0 ? "Reservas cargadas" : "No hay reservas",
            reservas.Count > 0);
    }

    public async Task<ResponseService<Reserva>> CreateReserva(Reserva reserva)
    {
        if (reserva.HoraFin <= reserva.HoraInicio)
        {
            return new ResponseService<Reserva>(null, "Hora fin debe ser mayor a hora inicio", false);
        }

        var fechaActual = DateTime.Today;
        var horaActual = DateTime.Now.TimeOfDay;
        if (reserva.Fecha.Date < fechaActual || 
            (reserva.Fecha.Date == fechaActual && reserva.HoraInicio < horaActual))
        {
            return new ResponseService<Reserva>(null, "No se puede crear reserva en fecha/hora pasada", false);
        }

        var usuario = await _context.Usuario.FindAsync(reserva.UsuarioId);
        if (usuario == null || !usuario.Activo)
        {
            return new ResponseService<Reserva>(null, "Usuario no encontrado", false);
        }

        var espacio = await _context.EspacioDeportivo.FindAsync(reserva.EspacioDeportivoId);
        if (espacio == null || !espacio.Activo)
        {
            return new ResponseService<Reserva>(null, "Espacio no encontrado", false);
        }

        var reservasEspacio = await _context.Reserva
            .Where(r => r.EspacioDeportivoId == reserva.EspacioDeportivoId
                && r.Fecha.Date == reserva.Fecha.Date
                && r.Estado == EstadoReserva.Activa
                && !(r.HoraFin <= reserva.HoraInicio || r.HoraInicio >= reserva.HoraFin))
            .ToListAsync();

        if (reservasEspacio.Any())
        {
            return new ResponseService<Reserva>(null, "Espacio ocupado en ese horario", false);
        }

        var reservasUsuario = await _context.Reserva
            .Where(r => r.UsuarioId == reserva.UsuarioId
                && r.Fecha.Date == reserva.Fecha.Date
                && r.Estado == EstadoReserva.Activa
                && !(r.HoraFin <= reserva.HoraInicio || r.HoraInicio >= reserva.HoraFin))
            .ToListAsync();

        if (reservasUsuario.Any())
        {
            return new ResponseService<Reserva>(null, "Usuario tiene otra reserva en ese horario", false);
        }

        reserva.Estado = EstadoReserva.Activa;
        await _context.Reserva.AddAsync(reserva);
        await _context.SaveChangesAsync();

        await _emailService.EnviarCorreoReserva(usuario.CorreoElectronico, usuario.Nombre, espacio.Nombre, reserva.Fecha, reserva.HoraInicio, reserva.HoraFin);

        return new ResponseService<Reserva>(reserva, "Reserva creada", true);
    }

    public async Task<ResponseService<Reserva>> CancelarReserva(int id)
    {
        var reserva = await _context.Reserva.FindAsync(id);
        if (reserva == null)
        {
            return new ResponseService<Reserva>(null, "Reserva no encontrada", false);
        }

        if (reserva.Estado == EstadoReserva.Cancelada)
        {
            return new ResponseService<Reserva>(null, "Reserva ya cancelada", false);
        }

        reserva.Estado = EstadoReserva.Cancelada;
        await _context.SaveChangesAsync();
        return new ResponseService<Reserva>(reserva, "Reserva cancelada", true);
    }

    public async Task<ResponseService<Reserva>> FinalizarReserva(int id)
    {
        var reserva = await _context.Reserva.FindAsync(id);
        if (reserva == null)
        {
            return new ResponseService<Reserva>(null, "Reserva no encontrada", false);
        }

        if (reserva.Estado != EstadoReserva.Activa)
        {
            return new ResponseService<Reserva>(null, "Reserva no está activa", false);
        }

        reserva.Estado = EstadoReserva.Finalizada;
        await _context.SaveChangesAsync();
        return new ResponseService<Reserva>(reserva, "Reserva finalizada", true);
    }
}