using Microsoft.EntityFrameworkCore;
using SportsSebastianVargas.Data;
using SportsSebastianVargas.Models;
using SportsSebastianVargas.Response;

namespace SportsSebastianVargas.Services;

public class EspacioDeportivoService
{
    private readonly AppDbContext _context;

    public EspacioDeportivoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseService<List<EspacioDeportivo>>> GetEspacios(string? tipo = null)
    {
        var query = _context.EspacioDeportivo.Where(e => e.Activo);

        if (!string.IsNullOrEmpty(tipo))
        {
            query = query.Where(e => e.Tipo.ToLower() == tipo.ToLower());
        }

        var espacios = await query.ToListAsync();
        return new ResponseService<List<EspacioDeportivo>>(
            espacios,
            espacios.Count > 0 ? "Espacios cargados" : "No hay espacios",
            espacios.Count > 0);
    }

    public async Task<ResponseService<EspacioDeportivo>> GetEspacio(int id)
    {
        var espacio = await _context.EspacioDeportivo.FindAsync(id);
        if (espacio == null || !espacio.Activo)
        {
            return new ResponseService<EspacioDeportivo>(null, "Espacio no encontrado", false);
        }
        return new ResponseService<EspacioDeportivo>(espacio, "Espacio encontrado", true);
    }

    public async Task<ResponseService<EspacioDeportivo>> CreateEspacio(EspacioDeportivo espacio)
    {
        var existeNombre = await _context.EspacioDeportivo
            .AnyAsync(e => e.Nombre.ToLower() == espacio.Nombre.ToLower());
        if (existeNombre)
        {
            return new ResponseService<EspacioDeportivo>(null, "Espacio ya registrado", false);
        }

        await _context.EspacioDeportivo.AddAsync(espacio);
        await _context.SaveChangesAsync();
        return new ResponseService<EspacioDeportivo>(espacio, "Espacio creado", true);
    }

    public async Task<ResponseService<EspacioDeportivo>> UpdateEspacio(EspacioDeportivo espacio)
    {
        var oldEspacio = await _context.EspacioDeportivo.FirstOrDefaultAsync(e => e.Id == espacio.Id);
        if (oldEspacio == null)
        {
            return new ResponseService<EspacioDeportivo>(null, "Espacio no encontrado", false);
        }

        _context.Entry(oldEspacio).CurrentValues.SetValues(espacio);
        await _context.SaveChangesAsync();
        return new ResponseService<EspacioDeportivo>(espacio, "Espacio actualizado", true);
    }

    public async Task<ResponseService<EspacioDeportivo>> DeleteEspacio(int id)
    {
        var espacio = await _context.EspacioDeportivo.FindAsync(id);
        if (espacio == null)
        {
            return new ResponseService<EspacioDeportivo>(null, "Espacio no encontrado", false);
        }

        espacio.Activo = false;
        await _context.SaveChangesAsync();
        return new ResponseService<EspacioDeportivo>(espacio, "Espacio eliminado", true);
    }
}