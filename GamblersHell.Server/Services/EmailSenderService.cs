using GamblersHell.Server.Interface;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.ComponentModel.DataAnnotations;

namespace GamblersHell.Server.Services
{
    public class EmailSenderService : IEmailSenderInterface
    {
        private readonly IConfiguration _configuration;
        [Required] private readonly string _smtpServer;
        [Required] private readonly int _smtPort;
        [Required] private readonly string _smtpUser;
        [Required] private readonly string _smtpPass;
        [Required] private readonly string _smtpMailFrom;
        private readonly ILogger<EmailSenderService> _logger;

        public EmailSenderService(IConfiguration configuration, ILogger<EmailSenderService> logger)
        {
            _configuration = configuration;
            _smtpServer = _configuration["MailSender:SmtpServer"];
            _smtPort = int.Parse(_configuration["MailSender:SmtpPort"]);
            _smtpUser = _configuration["MailSender:SmtpUser"];
            _smtpPass = _configuration["MailSender:SmtpPass"];
            _smtpMailFrom = _configuration["MailSender:SmtpMailFrom"];
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string plainTextBody, string htmlBody)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("GamblersHell", _smtpMailFrom));
            email.To.Add(new MailboxAddress("", to));
            email.Subject = subject;

            // Add Message-ID header to prevent deduplication
            email.MessageId = $"<{Guid.NewGuid()}@gamblershell.com>";

            // Add a unique header to prevent caching
            email.Headers.Add("X-Entity-Ref-ID", Guid.NewGuid().ToString());

            // Create the message body
            var multipart = new MultipartAlternative();

            // Add the plain text version
            var textPart = new TextPart("plain")
            {
                Text = plainTextBody
            };
            multipart.Add(textPart);

            // Add the HTML version
            var htmlPart = new TextPart("html")
            {
                Text = htmlBody
            };
            multipart.Add(htmlPart);

            email.Body = multipart;


            using (var client = new SmtpClient())
            {
                try
                {
                    // Connect to the SMTP server with TLS
                    _logger.LogInformation($"Connecting to SMTP server {_smtpServer}:{_smtPort} with TLS...");
                    await client.ConnectAsync(_smtpServer, _smtPort, SecureSocketOptions.StartTls);

                    // Check if the server supports authentication
                    if (client.AuthenticationMechanisms.Count > 0)
                    {
                        _logger.LogInformation("SMTP server supports authentication. Attempting to authenticate...");
                        await client.AuthenticateAsync(_smtpUser, _smtpPass);
                    }
                    else
                    {
                        _logger.LogWarning("SMTP server does not support authentication. This might cause issues with Gmail.");
                    }

                    // Send the email
                    await client.SendAsync(email);
                    _logger.LogInformation($"Email sent successfully to {to} with Message-ID: {email.MessageId}");
                }
                catch (SslHandshakeException ex)
                {
                    _logger.LogError($"SSL/TLS handshake failed: {ex.Message}");
                    throw new Exception("Failed to establish secure connection with the SMTP server. Please check your SSL/TLS settings.", ex);
                }
                catch (AuthenticationException ex)
                {
                    _logger.LogError($"Authentication failed: {ex.Message}");
                    throw new Exception("Failed to authenticate with the SMTP server. Please check your credentials.", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to send email to {to}. Error: {ex.Message}");
                    throw new Exception($"Failed to send email: {ex.Message}", ex);
                }
                finally
                {
                    if (client.IsConnected)
                    {
                        await client.DisconnectAsync(true);
                    }
                }
            }
        }

        public async Task SendWelcomeMailAsync(string to, string username)
        {
            var subject = $"Welcome {username}";
            var plainTextBody = $"Dear {username}, we are so glad to have you on this hellish adventure";
            var baseUrl = _configuration["BaseURL"];
            var logoImgurPath = _configuration["LogoPath"];
            var loginLink = $"{baseUrl}/login";

            var htmlBody = $@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
                        <html xmlns=""http://www.w3.org/1999/xhtml"">
                        <head>
                            <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
                            <title>Welcome to Gambler's Hell</title>
                            <style type=""text/css"">
                                /* Ensure images display properly */
                                img {{ max-width: 100%; display: block; }}
                            </style>
                        </head>
                        <body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #000000;"">
                            <!-- Main Table -->
                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #000000;"">
                                <tr>
                                    <td align=""center"" style=""padding: 20px 0;"">
                                        <!-- Content Table -->
                                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""background-color: #1a0000; border-radius: 8px; border: 1px solid #3d0000; box-shadow: 0 4px 16px rgba(255, 0, 0, 0.2);"">
                                            <!-- Header -->
                                            <tr>
                                                <td align=""center"" style=""padding: 20px 0; background-color: #0a0000; border-radius: 8px 8px 0 0;"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"">
                                                        <tr>
                                                            <td align=""center"">
                                                                <img src=""{logoImgurPath}"" style=""max-width:150px; max-height:150px""/>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <h1 style=""color: #ff0000; font-size: 28px; margin: 10px 0 5px 0;"">Welcome to Gambler's Hell</h1>
                                                    <p style=""color: #ff3333; margin: 0; font-size: 16px;"">Your journey begins</p>
                                                </td>
                                            </tr>
                                            <!-- Body -->
                                            <tr>
                                                <td style=""padding: 30px 20px; background-color: #0e0000;"">
                                                    <p style=""color: #ff3333; font-size: 18px; line-height: 1.6; text-align: center;"">
                                                        Dear <span style=""color: #ff0000; font-weight: bold;"">{username}</span>, welcome, we are glad to have you on this hellish adventure.
                                                    </p>
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin: 20px 0;"">
                                                        <tr>
                                                            <td align=""center"">
                                                                <a href=""{loginLink}"" style=""background-color: #ff0000; color: white; text-decoration: none; padding: 12px 24px; border-radius: 8px; font-weight: bold; display: inline-block;"">Login into Gambler's Hell</a>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin: 20px 0;"">
                                                        <tr>
                                                            <td style=""height: 1px; background-color: rgba(255, 0, 0, 0.3);""></td>
                                                        </tr>
                                                    </table>
                                                    <p style=""color: #ff3333; font-size: 12px; text-align: center;"">Prepare yourself for what lies ahead, may luck be on your side mortal because they await..</p>
                                                </td>
                                            </tr>
                                            <!-- Footer -->
                                            <tr>
                                                <td align=""center"" style=""padding: 20px; background-color: #0a0000; border-radius: 0 0 8px 8px;"">
                                                    <p style=""color: #ff3333; font-size: 12px; margin: 0;"">© 2025 Gambler's Hell. @Eziich. All rights reserved.</p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </body>
                        </html>";

            await SendEmailAsync(to, subject, plainTextBody, htmlBody);
        }

        public async Task SendResetPasswordMailAsync(string to, string token)
        {
            var subject = $"Reset token for {to}";
            var plainTextBody = $"Token for reseting password for {to}";
            var logoImgurPath = _configuration["LogoPath"];
            var baseUrl = _configuration["BaseURL"];
            var forgottenPasswordLink = $"{baseUrl}/forgottenpassword";

            var htmlBody = $@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
                        <html xmlns=""http://www.w3.org/1999/xhtml"">
                        <head>
                            <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
                            <title>Gambler's Hell - Forgotten Password</title>
                            <style type=""text/css"">
                            </style>
                        </head>
                        <body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #000000;"">
                            <!-- Main Table -->
                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #000000;"">
                                <tr>
                                    <td align=""center"" style=""padding: 20px 0;"">
                                        <!-- Content Table -->
                                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""background-color: #1a0000; border-radius: 8px; border: 1px solid #3d0000; box-shadow: 0 4px 16px rgba(255, 0, 0, 0.2);"">
                                            <!-- Header -->
                                            <tr>
                                                <td align=""center"" style=""padding: 20px 0; background-color: #0a0000; border-radius: 8px 8px 0 0;"">
                                                    <img src=""{logoImgurPath}"" style=""max-width:150px; max-height:150px""/>
                                                    <h1 style=""color: #ff0000; font-size: 28px; margin: 10px 0 5px 0;"">Gambler's Hell - Forgotten Password</h1>
                                                    <p style=""color: #ff3333; margin: 0; font-size: 16px;"">Input the token into text field on forgotten password page</p>
                                                </td>
                                            </tr>
                                            <!-- Body -->
                                            <tr>
                                                <td style=""padding: 30px 20px; background-color: #0e0000;"">
                                                    <p style=""color: #ff3333; font-size: 18px; line-height: 1.6; text-align: center;"">
                                                        Password reset token for <span style=""color: #ff0000; font-weight: bold;"">{to}</span> is: 
                                                    </p>
                                                    <br/>
                                                    <p style=""color: #ff3333; font-size: 18px; line-height: 1.6; text-align: center;"">{token}</p>
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin: 20px 0;"">
                                                        <tr>
                                                            <td align=""center"">
                                                                <a href=""{forgottenPasswordLink}"" style=""background-color: #ff0000; color: white; text-decoration: none; padding: 12px 24px; border-radius: 8px; font-weight: bold; display: inline-block;"">Continue to Forgotten Password page</a>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin: 20px 0;"">
                                                        <tr>
                                                            <td style=""height: 1px; background-color: rgba(255, 0, 0, 0.3);""></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <!-- Footer -->
                                            <tr>
                                                <td align=""center"" style=""padding: 20px; background-color: #0a0000; border-radius: 0 0 8px 8px;"">
                                                    <p style=""color: #ff3333; font-size: 12px; margin: 0;"">© 2025 Gambler's Hell. @Eziich. All rights reserved.</p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </body>
                        </html>";

            await SendEmailAsync(to, subject, plainTextBody, htmlBody);
        }

        public async Task RequestVerificationToken(string to, string token)
        {
            var subject = $"Verification token for {to}";
            var plainTextBody = $"Token for verifying account for {to}";
            var logoImgurPath = _configuration["LogoPath"];
            var baseUrl = _configuration["BaseURL"];
            var forgottenPasswordLink = $"{baseUrl}/forgottenpassword";

            var htmlBody = $@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
                        <html xmlns=""http://www.w3.org/1999/xhtml"">
                        <head>
                            <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
                            <title>Gambler's Hell - Account Verification</title>
                            <style type=""text/css"">
                            </style>
                        </head>
                        <body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #000000;"">
                            <!-- Main Table -->
                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #000000;"">
                                <tr>
                                    <td align=""center"" style=""padding: 20px 0;"">
                                        <!-- Content Table -->
                                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""background-color: #1a0000; border-radius: 8px; border: 1px solid #3d0000; box-shadow: 0 4px 16px rgba(255, 0, 0, 0.2);"">
                                            <!-- Header -->
                                            <tr>
                                                <td align=""center"" style=""padding: 20px 0; background-color: #0a0000; border-radius: 8px 8px 0 0;"">
                                                    <img src=""{logoImgurPath}"" style=""max-width:150px; max-height:150px""/>
                                                    <h1 style=""color: #ff0000; font-size: 28px; margin: 10px 0 5px 0;"">Gambler's Hell - Account Verification</h1>
                                                    <p style=""color: #ff3333; margin: 0; font-size: 16px;"">Input the token into text field on account verification page</p>
                                                </td>
                                            </tr>
                                            <!-- Body -->
                                            <tr>
                                                <td style=""padding: 30px 20px; background-color: #0e0000;"">
                                                    <p style=""color: #ff3333; font-size: 18px; line-height: 1.6; text-align: center;"">
                                                        Account verification token for <span style=""color: #ff0000; font-weight: bold;"">{to}</span> is: 
                                                    </p>
                                                    <br/>
                                                    <p style=""color: #ff3333; font-size: 18px; line-height: 1.6; text-align: center;"">{token}</p>
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin: 20px 0;"">
                                                        <tr>
                                                            <td style=""height: 1px; background-color: rgba(255, 0, 0, 0.3);""></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <!-- Footer -->
                                            <tr>
                                                <td align=""center"" style=""padding: 20px; background-color: #0a0000; border-radius: 0 0 8px 8px;"">
                                                    <p style=""color: #ff3333; font-size: 12px; margin: 0;"">© 2025 Gambler's Hell. @Eziich. All rights reserved.</p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </body>
                        </html>";

            await SendEmailAsync(to, subject, plainTextBody, htmlBody);
        }
    }
}
