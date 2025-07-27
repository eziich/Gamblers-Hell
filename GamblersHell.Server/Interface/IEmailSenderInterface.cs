namespace GamblersHell.Server.Interface
{
    public interface IEmailSenderInterface
    {
        public Task SendEmailAsync(string to, string subject, string plainTextBody, string htmlBody);

        public Task SendWelcomeMailAsync(string to, string username);

        public Task SendResetPasswordMailAsync(string to, string token);

        public Task RequestVerificationToken(string to, string token);
    }
}
