using MD.CreadorDeReportes.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace MD.CreadorDeReportes.Servicios
{
    public class Email : IEmail
    {
        private IConfiguration _configuration { get; }
        private SmtpClient _cliente { get; }

        public Email(IConfiguration configuration)
        {
            _configuration = configuration;
            _cliente = new SmtpClient()
            {
                Host = _configuration["EmailOptions:Host"].ToString(),
                Port = Convert.ToInt32(_configuration["EmailOptions:Port"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_configuration["EmailOptions:Email"], _configuration["EmailOptions:Password"]),
                EnableSsl = Convert.ToBoolean(_configuration["EmailOptions:EnableSsl"])
            };
        }

        public void EnviarCorreo(string correo, string asunto, string mensaje)
        {
            var mailMessage = new MailMessage(_configuration["EmailOptions:Email"], correo, asunto, mensaje);
            mailMessage.IsBodyHtml = true;
            _cliente.Send(mailMessage);
        }
    }
}
