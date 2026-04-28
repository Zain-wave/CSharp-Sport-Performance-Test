using SportsSebastianVargas.Data;
using SportsSebastianVargas.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<EspacioDeportivoService>();
builder.Services.AddScoped<ReservaService>();
builder.Services.AddScoped<EmailService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Usuario.Any())
    {
        db.Usuario.AddRange(
            new SportsSebastianVargas.Models.Usuario { Nombre = "Carlos Mendez", DocumentoIdentidad = "1012345678", Telefono = "3001234567", CorreoElectronico = "carlos.mendez@mail.com", Activo = true },
            new SportsSebastianVargas.Models.Usuario { Nombre = "Laura Torres", DocumentoIdentidad = "1023456789", Telefono = "3002345678", CorreoElectronico = "laura.torres@mail.com", Activo = true },
            new SportsSebastianVargas.Models.Usuario { Nombre = "Andres Ruiz", DocumentoIdentidad = "1034567890", Telefono = "3003456789", CorreoElectronico = "andres.ruiz@mail.com", Activo = true }
        );
        db.EspacioDeportivo.AddRange(
            new SportsSebastianVargas.Models.EspacioDeportivo { Nombre = "Cancha Futbol 1", Tipo = "Futbol", Capacidad = 22, Activo = true, ImagenUrl = "https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=800&q=80" },
            new SportsSebastianVargas.Models.EspacioDeportivo { Nombre = "Cancha Tenis A", Tipo = "Tenis", Capacidad = 4, Activo = true, ImagenUrl = "https://images.unsplash.com/photo-1554068865-24cecd4e34b8?w=800&q=80" },
            new SportsSebastianVargas.Models.EspacioDeportivo { Nombre = "Polideportivo Central", Tipo = "Multiuso", Capacidad = 50, Activo = true, ImagenUrl = "https://images.unsplash.com/photo-1546519638-68e109498ffc?w=800&q=80" }
        );
        db.SaveChanges();
        var usuarios = db.Usuario.ToList();
        var espacios = db.EspacioDeportivo.ToList();
        db.Reserva.AddRange(
            new SportsSebastianVargas.Models.Reserva { UsuarioId = usuarios[0].Id, EspacioDeportivoId = espacios[0].Id, Fecha = DateTime.Today.AddDays(1).Date, HoraInicio = new TimeSpan(9, 0, 0), HoraFin = new TimeSpan(11, 0, 0), Estado = SportsSebastianVargas.Models.Enums.EstadoReserva.Activa },
            new SportsSebastianVargas.Models.Reserva { UsuarioId = usuarios[1].Id, EspacioDeportivoId = espacios[1].Id, Fecha = DateTime.Today.AddDays(2).Date, HoraInicio = new TimeSpan(14, 0, 0), HoraFin = new TimeSpan(16, 0, 0), Estado = SportsSebastianVargas.Models.Enums.EstadoReserva.Activa },
            new SportsSebastianVargas.Models.Reserva { UsuarioId = usuarios[2].Id, EspacioDeportivoId = espacios[2].Id, Fecha = DateTime.Today.AddDays(1).Date, HoraInicio = new TimeSpan(17, 0, 0), HoraFin = new TimeSpan(19, 0, 0), Estado = SportsSebastianVargas.Models.Enums.EstadoReserva.Finalizada }
        );
        db.SaveChanges();
    }
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();