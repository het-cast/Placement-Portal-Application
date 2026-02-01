using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;

namespace PlacementPortal.BLL.Helper;

public class EmailHelper
{
    private readonly IConfiguration _configuration;

    public EmailHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            IConfigurationSection emailSettings = _configuration.GetSection("EmailSettings");

            MimeMessage emailToSend = new();
            emailToSend.From.Add(MailboxAddress.Parse(emailSettings["SenderEmail"]));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            using SmtpClient emailClient = new();
            await emailClient.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]!), MailKit.Security.SecureSocketOptions.StartTls);
            await emailClient.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["SenderPassword"]);
            await emailClient.SendAsync(emailToSend);
            await emailClient.DisconnectAsync(true);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void SendEmailSync(string email, string subject, string htmlMessage)
    {
        SendEmailAsync(email, subject, htmlMessage).GetAwaiter().GetResult(); ;
    }
}
