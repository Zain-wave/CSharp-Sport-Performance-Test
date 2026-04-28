using Microsoft.EntityFrameworkCore;
using SportsSebastianVargas.Data;
using SportsSebastianVargas.Models;
using SportsSebastianVargas.Response;

namespace SportsSebastianVargas.Services;

public class UsuarioService
{
    private readonly AppDbContext _context;

    public UsuarioService(AppDbContext context) { _context = context; }

    public async Task<ResponseService<List<Usuario>>> GetUsuarios(string? estado = null)
    {
        var query = _context.Usuario.AsQueryable();
        
        if (!string.IsNullOrEmpty(estado))
        {
            var esActivo = estado.ToLower() == "activo";
            query = query.Where(u => u.Activo == esActivo);
        }
        else
        {
            query = query.Where(u => u.Activo);
        }
        
        var usuarios = await query.ToListAsync();
        return new ResponseService<List<Usuario>>(usuarios, usuarios.Count > 0 ? "Usuarios cargados" : "No hay usuarios", usuarios.Count > 0);
    }

    public async Task<ResponseService<List<Usuario>>> GetAllUsuarios()
    {
        var usuarios = await _context.Usuario.ToListAsync();
        return new ResponseService<List<Usuario>>(usuarios, usuarios.Count > 0 ? "Usuarios cargados" : "No hay usuarios", usuarios.Count > 0);
    }

    public async Task<ResponseService<Usuario>> CreateUsuario(Usuario usuario)
    {
        var existeDoc = await _context.Usuario.AnyAsync(u => u.DocumentoIdentidad == usuario.DocumentoIdentidad);
        if (existeDoc) return new ResponseService<Usuario>(null, "Documento ya registrado", false);

        var existeCorreo = await _context.Usuario.AnyAsync(u => u.CorreoElectronico == usuario.CorreoElectronico);
        if (existeCorreo) return new ResponseService<Usuario>(null, "Correo ya registrado", false);

        await _context.Usuario.AddAsync(usuario);
        await _context.SaveChangesAsync();
        return new ResponseService<Usuario>(usuario, "Usuario creado", true);
    }

    public async Task<ResponseService<Usuario>> UpdateUsuario(Usuario usuario)
    {
        var oldUsuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Id == usuario.Id);
        if (oldUsuario == null) return new ResponseService<Usuario>(null, "Usuario no encontrado", false);

        _context.Entry(oldUsuario).CurrentValues.SetValues(usuario);
        await _context.SaveChangesAsync();
        return new ResponseService<Usuario>(usuario, "Usuario actualizado", true);
    }

    public async Task<ResponseService<Usuario>> DeleteUsuario(int id)
    {
        var usuario = await _context.Usuario.FindAsync(id);
        if (usuario == null) return new ResponseService<Usuario>(null, "Usuario no encontrado", false);

        usuario.Activo = false;
        await _context.SaveChangesAsync();
        return new ResponseService<Usuario>(usuario, "Usuario eliminado", true);
    }

    public async Task<ResponseService<Usuario>> ToggleEstado(int id)
    {
        var usuario = await _context.Usuario.FindAsync(id);
        if (usuario == null) return new ResponseService<Usuario>(null, "Usuario no encontrado", false);

        usuario.Activo = !usuario.Activo;
        await _context.SaveChangesAsync();
        return new ResponseService<Usuario>(usuario, usuario.Activo ? "Usuario activado" : "Usuario desactivado", true);
    }
}