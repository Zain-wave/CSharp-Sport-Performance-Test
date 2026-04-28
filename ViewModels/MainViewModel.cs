using SportsSebastianVargas.Models;

namespace SportsSebastianVargas.ViewModels;

public class UsuarioViewModel
{
    public List<Usuario> UsuarioList { get; set; } = new();
    public Usuario Usuario { get; set; } = new();
}

public class EspacioDeportivoViewModel
{
    public List<EspacioDeportivo> EspacioList { get; set; } = new();
    public EspacioDeportivo Espacio { get; set; } = new();
}

public class ReservaViewModel
{
    public List<Reserva> ReservaList { get; set; } = new();
    public Reserva Reserva { get; set; } = new();
    public List<Usuario> UsuarioList { get; set; } = new();
}