using Microsoft.EntityFrameworkCore;
using SportsSebastianVargas.Data;
using SportsSebastianVargas.Models;
using SportsSebastianVargas.Models.Enums;
using SportsSebastianVargas.Response;

namespace SportsSebastianVargas.Services;

public class UsuarioService
{
    private readonly AppDbContext _context;
    private readonly Dictionary<int, Usuario> _cache = new();

    public UsuarioService(AppDbContext context)
    {
        _context = context;
        CargarCache();
    }

    private void CargarCache()
    {
        try
        {
            _cache.Clear();
            var usuarios = _context.Usuario.ToList();
            foreach (var u in usuarios)
                _cache[u.Id] = u;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando cache de usuarios: {ex.Message}");
        }
    }

    public async Task<ResponseService<List<Usuario>>> GetUsuarios(string? estado = null)
    {
        IQueryable<Usuario> query;
        
        if (string.IsNullOrEmpty(estado))
        {
            query = _context.Usuario;
        }
        else
        {
            var esActivo = estado.ToLower() == "activo";
            query = _context.Usuario.Where(u => u.Activo == esActivo);
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
        try
        {
            if (_cache.Values.Any(u => u.DocumentoIdentidad == usuario.DocumentoIdentidad))
                return new ResponseService<Usuario>(null, "Documento ya registrado", false);

            if (_cache.Values.Any(u => u.CorreoElectronico == usuario.CorreoElectronico))
                return new ResponseService<Usuario>(null, "Correo ya registrado", false);

            await _context.Usuario.AddAsync(usuario);
            await _context.SaveChangesAsync();
            _cache[usuario.Id] = usuario;
            return new ResponseService<Usuario>(usuario, "Usuario creado", true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear usuario: {ex.Message}");
            return new ResponseService<Usuario>(null, "Error al crear usuario", false);
        }
    }

    public async Task<ResponseService<Usuario>> UpdateUsuario(Usuario usuario)
    {
        try
        {
            if (!_cache.ContainsKey(usuario.Id))
                return new ResponseService<Usuario>(null, "Usuario no encontrado", false);

            var oldUsuario = _cache[usuario.Id];
            _context.Entry(oldUsuario).CurrentValues.SetValues(usuario);
            await _context.SaveChangesAsync();
            _cache[usuario.Id] = usuario;
            return new ResponseService<Usuario>(usuario, "Usuario actualizado", true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar usuario: {ex.Message}");
            return new ResponseService<Usuario>(null, "Error al actualizar usuario", false);
        }
    }

    public async Task<ResponseService<Usuario>> DeleteUsuario(int id)
    {
        try
        {
            if (!_cache.ContainsKey(id))
                return new ResponseService<Usuario>(null, "Usuario no encontrado", false);

            var usuario = _cache[id];
            usuario.Activo = false;
            await _context.SaveChangesAsync();
            _cache[id] = usuario;
            return new ResponseService<Usuario>(usuario, "Usuario eliminado", true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar usuario: {ex.Message}");
            return new ResponseService<Usuario>(null, "Error al eliminar usuario", false);
        }
    }

    public async Task<ResponseService<Usuario>> ToggleEstado(int id)
    {
        try
        {
            if (!_cache.ContainsKey(id))
                return new ResponseService<Usuario>(null, "Usuario no encontrado", false);

            var usuario = _cache[id];
            usuario.Activo = !usuario.Activo;
            await _context.SaveChangesAsync();
            _cache[id] = usuario;

            if (!usuario.Activo)
            {
                var reservasActivas = await _context.Reserva
                    .Where(r => r.UsuarioId == id && r.Estado == EstadoReserva.Activa)
                    .ToListAsync();

                foreach (var reserva in reservasActivas)
                    reserva.Estado = EstadoReserva.Cancelada;

                await _context.SaveChangesAsync();
            }

            return new ResponseService<Usuario>(usuario, usuario.Activo ? "Usuario activado" : "Usuario desactivado y reservas canceladas", true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cambiar estado: {ex.Message}");
            return new ResponseService<Usuario>(null, "Error al cambiar estado", false);
        }
    }
}