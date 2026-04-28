using Microsoft.EntityFrameworkCore;
using SportsSebastianVargas.Data;
using SportsSebastianVargas.Models;
using SportsSebastianVargas.Response;

namespace SportsSebastianVargas.Services;

public class EspacioDeportivoService
{
    private readonly AppDbContext _context;
    private readonly Dictionary<int, EspacioDeportivo> _cache = new();

    public EspacioDeportivoService(AppDbContext context)
    {
        _context = context;
        CargarCache();
    }

    private void CargarCache()
    {
        try
        {
            _cache.Clear();
            var espacios = _context.EspacioDeportivo.ToList();
            foreach (var e in espacios)
                _cache[e.Id] = e;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando cache de espacios: {ex.Message}");
        }
    }

    public async Task<ResponseService<List<EspacioDeportivo>>> GetEspacios(string? tipo = null)
    {
        try
        {
            var query = _cache.Values.Where(e => e.Activo);
            if (!string.IsNullOrEmpty(tipo))
                query = query.Where(e => e.Tipo.ToLower() == tipo.ToLower());

            var espacios = query.ToList();
            return new ResponseService<List<EspacioDeportivo>>(espacios, espacios.Count > 0 ? "Espacios cargados" : "No hay espacios", espacios.Count > 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener espacios: {ex.Message}");
            return new ResponseService<List<EspacioDeportivo>>(new(), "Error al cargar espacios", false);
        }
    }

    public async Task<ResponseService<EspacioDeportivo>> CreateEspacio(EspacioDeportivo espacio)
    {
        try
        {
            if (_cache.Values.Any(e => e.Nombre.ToLower() == espacio.Nombre.ToLower()))
                return new ResponseService<EspacioDeportivo>(null, "Espacio ya registrado", false);

            await _context.EspacioDeportivo.AddAsync(espacio);
            await _context.SaveChangesAsync();
            _cache[espacio.Id] = espacio;
            return new ResponseService<EspacioDeportivo>(espacio, "Espacio creado", true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear espacio: {ex.Message}");
            return new ResponseService<EspacioDeportivo>(null, "Error al crear espacio", false);
        }
    }

    public async Task<ResponseService<EspacioDeportivo>> UpdateEspacio(EspacioDeportivo espacio)
    {
        try
        {
            if (!_cache.ContainsKey(espacio.Id))
                return new ResponseService<EspacioDeportivo>(null, "Espacio no encontrado", false);

            var old = _cache[espacio.Id];
            _context.Entry(old).CurrentValues.SetValues(espacio);
            await _context.SaveChangesAsync();
            _cache[espacio.Id] = espacio;
            return new ResponseService<EspacioDeportivo>(espacio, "Espacio actualizado", true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar espacio: {ex.Message}");
            return new ResponseService<EspacioDeportivo>(null, "Error al actualizar espacio", false);
        }
    }

    public async Task<ResponseService<EspacioDeportivo>> DeleteEspacio(int id)
    {
        try
        {
            if (!_cache.ContainsKey(id))
                return new ResponseService<EspacioDeportivo>(null, "Espacio no encontrado", false);

            var espacio = _cache[id];
            espacio.Activo = false;
            await _context.SaveChangesAsync();
            _cache[id] = espacio;
            return new ResponseService<EspacioDeportivo>(espacio, "Espacio eliminado", true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar espacio: {ex.Message}");
            return new ResponseService<EspacioDeportivo>(null, "Error al eliminar espacio", false);
        }
    }
}