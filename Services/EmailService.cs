using MailKit.Net.Smtp;
using MimeKit;

namespace SportsSebastianVargas.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration) { _configuration = configuration; }

    public async Task EnviarCorreoReserva(string correo, string nombre, string espacio, DateTime fecha, TimeSpan inicio, TimeSpan fin)
    {
        try
        {
            var smtpHost = _configuration["Email:Smtp:Host"] ?? "smtp.gmail.com";
            var smtpPort = _configuration["Email:Smtp:Port"] ?? "587";
            var usuario = _configuration["Email:Smtp:Usuario"] ?? "";
            var password = _configuration["Email:Smtp:Password"] ?? "";
            var remetente = _configuration["Email:Remitente"] ?? usuario;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine($"[Simulación] Correo: Reserva confirmada para {espacio} el {fecha:dd/MM/yyyy}");
                return;
            }

            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Complejo Deportivo", remetente));
            mensaje.To.Add(new MailboxAddress(nombre, correo));
            mensaje.Subject = "Confirmación de Reserva";
            mensaje.Body = new TextPart("html")
            {
                Text = $"<h2>Reserva Confirmada</h2><p>Hola {nombre}, tu reserva está confirmada.</p><p><strong>Espacio:</strong> {espacio}<br><strong>Fecha:</strong> {fecha:dd/MM/yyyy}<br><strong>Hora:</strong> {inicio} - {fin}</p>"
            };

            using var cliente = new SmtpClient();
            await cliente.ConnectAsync(smtpHost, int.Parse(smtpPort), false);
            await cliente.AuthenticateAsync(usuario, password);
            await cliente.SendAsync(mensaje);
            await cliente.DisconnectAsync(true);
        }
        catch (Exception ex) { Console.WriteLine($"Error enviar correo: {ex.Message}"); }
    }
}