using GamblersHell.Shared.Interface;
using System.Data;
using GamblersHell.Server.Controllers;
using GamblersHell.Shared.Interface;
using GamblersHell.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;
using BC = BCrypt.Net.BCrypt;
using Microsoft.Data.Sqlite;

namespace GamblersHell.Server.Services
{
    public class AuthService : IAuthRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly string _connectionString;

        public AuthService(IConfiguration configuration, ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string is missing");
        }

        public async Task<UserDTO?> GetUserAuthAsync(LoginDTO loginModel)
        {
            var user = new UserDTO();
            string fetchedPassword = string.Empty;

            var sql = "SELECT User_ID, User_Username, User_FirstName, User_LastName, User_Email, User_Level, User_Balance, User_BalanceAfterWin, User_Eye, User_LastLogin, User_Password, User_Verified FROM User WHERE User_Username = @username";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", loginModel.Username);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync()) 
                        {
                            user = new UserDTO
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("User_ID")),
                                Username = reader.IsDBNull(reader.GetOrdinal("User_Username")) ? null : reader.GetString(reader.GetOrdinal("User_Username")),
                                FirstName = reader.IsDBNull(reader.GetOrdinal("User_FirstName")) ? null : reader.GetString(reader.GetOrdinal("User_FirstName")),
                                LastName = reader.IsDBNull(reader.GetOrdinal("User_LastName")) ? null : reader.GetString(reader.GetOrdinal("User_LastName")),
                                Email = reader.IsDBNull(reader.GetOrdinal("User_Email")) ? null : reader.GetString(reader.GetOrdinal("User_Email")),
                                Level = reader.IsDBNull(reader.GetOrdinal("User_Level")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_Level")),
                                Balance = reader.IsDBNull(reader.GetOrdinal("User_Balance")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_Balance")),
                                BalanceAfterWin = reader.IsDBNull(reader.GetOrdinal("User_BalanceAfterWin")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_BalanceAfterWin")),
                                Eye = reader.IsDBNull(reader.GetOrdinal("User_Eye")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_Eye")),
                            };

                            // Check if the "User_LastLogin" value is DBNull or empty string before trying to parse it
                            var lastLogin = reader.IsDBNull(reader.GetOrdinal("User_LastLogin"))
                                            ? (DateTime?)null
                                            : (string.IsNullOrEmpty(reader.GetString(reader.GetOrdinal("User_LastLogin")))
                                                ? (DateTime?)null
                                                : DateTime.Parse(reader.GetString(reader.GetOrdinal("User_LastLogin"))));

                            user.LastLogin = lastLogin ?? DateTime.MinValue;  // Set default if null

                            fetchedPassword = reader.IsDBNull(reader.GetOrdinal("User_Password")) ? string.Empty : reader.GetString(reader.GetOrdinal("User_Password"));

                            user.UserVerified = reader.IsDBNull(reader.GetOrdinal("User_Verified")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_Verified"));
                        }
                    }

                    // Ensure that the password is hashed correctly using BCrypt
                    if (string.IsNullOrEmpty(fetchedPassword) || !BCrypt.Net.BCrypt.Verify(loginModel.Password, fetchedPassword))
                        return null;

                    return user;
                }
            }
        }
    }
}

