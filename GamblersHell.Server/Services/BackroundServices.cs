using GamblersHell.Client.Services;
using Microsoft.Data.Sqlite;


public class TimedHostedService : BackgroundService
{
    private readonly ILogger<TimedHostedService> _logger;
    private readonly string _connectionString;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);

    public TimedHostedService(ILogger<TimedHostedService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupUnverifiedUsers();
                _logger.LogInformation("Next check for unverified users: {time}", DateTimeOffset.Now);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while cleaning up unverified users");
                await Task.Delay(_interval, stoppingToken);
            }
        }

        _logger.LogInformation("Timed Hosted Service is stopping.");
    }

    public async Task<int> CleanupUnverifiedUsers()
    {
        var sql = @"DELETE FROM User 
        WHERE User_Verified = 0 
        AND User_RegistrationTime IS NOT NULL
        AND datetime(User_RegistrationTime, '+10 minutes') < datetime('now')";

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqliteCommand(sql, connection);
        int rowsAffected = await command.ExecuteNonQueryAsync();

        _logger.LogInformation("Deleted {count} unverified users at {time}.", rowsAffected, DateTimeOffset.Now);

        

        return rowsAffected;
    }


}
