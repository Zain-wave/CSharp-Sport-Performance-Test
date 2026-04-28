namespace SportsSebastianVargas.Models;

public class EspacioDeportivo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Tipo { get; set; } = "";
    public int Capacidad { get; set; }
    public bool Activo { get; set; } = true;
    public string? ImagenUrl { get; set; }
}