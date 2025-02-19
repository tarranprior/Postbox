using System.Net;
using System.Net.Mail;
using Serilog;
using Postbox.Configuration;

namespace Postbox.Communication;

/// <summary>
/// Handles sending emails with attachments via SMTP.
/// </summary>
public static class SmtpHandler
{
    /// <summary>
    /// Sends an email to a recipient with an optional attachment.
    /// </summary>
    /// <param name="recipientEmail">The recipient's email address.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The body of the email.</param>
    /// <param name="attachmentPath">Optional attachment file path.</param>
    public static void SendEmail(string recipientEmail, string subject, string body, string? attachmentPath = null)
    {
        try
        {
            ConfigManager.LoadConfig();

            string smtpServer = ConfigManager.Get("SMTP_SERVER", "127.0.0.1");
            int smtpPort = ConfigManager.GetInt("SMTP_PORT", 1025);
            string smtpUser = ConfigManager.Get("SMTP_USER");
            string smtpPass = ConfigManager.Get("SMTP_PASS");

            using SmtpClient smtp = new (smtpServer, smtpPort);
            {
                smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                smtp.EnableSsl = true;

                using MailMessage message = new ()
                {
                    From = new MailAddress(smtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };
                message.To.Add(recipientEmail);

                if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                {
                    message.Attachments.Add(new Attachment(attachmentPath));
                }

                Log.Information($"📩 Dispatching key to {recipientEmail} via {smtpServer}:{smtpPort}...");
                smtp.Send(message);
                Log.Information($"Key has been sent to {recipientEmail}.");
            }
        }
        catch (SmtpException smtpEx)
        {
            Log.Error($"SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
    }
}