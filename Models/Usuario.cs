namespace SportsSebastianVargas.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string DocumentoIdentidad { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string CorreoElectronico { get; set; } = "";
    public bool Activo { get; set; } = true;
}