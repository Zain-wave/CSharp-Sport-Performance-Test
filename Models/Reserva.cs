using SportsSebastianVargas.Models.Enums;

namespace SportsSebastianVargas.Models;

public class Reserva
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public int EspacioDeportivoId { get; set; }
    public EspacioDeportivo? EspacioDeportivo { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public EstadoReserva Estado { get; set; } = EstadoReserva.Activa;
}