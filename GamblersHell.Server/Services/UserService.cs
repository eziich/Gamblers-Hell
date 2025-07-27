using Microsoft.Data.Sqlite;
using GamblersHell.Shared;
using GamblersHell.Server.Interface;
using System.Text;


namespace GamblersHell.Server.Services
{
    public class UserService
    {
        private readonly string _connectionString;
        private readonly IEmailSenderInterface _emailSender;

        public UserService(string connectionString, IEmailSenderInterface emailSender)
        {
            _connectionString = connectionString;
            _emailSender = emailSender;
        }

        // Get a user by ID using raw ADO.NET with a 'using' block
        public async Task<UserDTO> GetUserByID(int id)
        {
            // SQL query to retrieve user by ID
            var sql = "SELECT User_ID, User_Username, User_FirstName, User_LastName, User_Email, User_Level, User_Balance, User_BalanceAfterWin, User_Eye, User_LastLogin, User_PokerGameControl, User_PandoraGameControl, User_Verified, User_RegistrationTime FROM User WHERE User_ID = @id";

            // Use 'using' to ensure the connection is properly opened and disposed of
            using (var connection = new SqliteConnection(_connectionString))
            {
                // Open the connection
                await connection.OpenAsync();

                // Create the command with parameterized SQL
                using (var command = new SqliteCommand(sql, connection))
                {
                    // Add the parameter to the command
                    command.Parameters.AddWithValue("@id", id);

                    // Execute the command and read the result
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())  // If data is found
                        {
                            // Map the result to the DTO
                            return new UserDTO
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
                                LastLogin = reader.IsDBNull(reader.GetOrdinal("User_LastLogin")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("User_LastLogin")),
                                PokerGameControl = reader.IsDBNull(reader.GetOrdinal("User_PokerGameControl")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_PokerGameControl")),
                                PandoraGameControl = reader.IsDBNull(reader.GetOrdinal("User_PandoraGameControl")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_PandoraGameControl")),
                                UserVerified = reader.IsDBNull(reader.GetOrdinal("User_Verified")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_Verified")),
                                UserRegistrationTime = reader.IsDBNull(reader.GetOrdinal("User_RegistrationTime")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("User_RegistrationTime")),
                            };
                        }
                    }
                }
            }
            return null; // If no user found
        }

        private async Task<bool> IsUsernameOrEmailTaken(string username, string email)
        {
            var sql = "SELECT COUNT(1) FROM User WHERE User_Username = @Username OR User_Email = @Email";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqliteCommand(sql, connection))
                {
                    // Add parameters to avoid SQL injection
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Email", email);

                    // Execute the query and check if any row was returned
                    var result = await command.ExecuteScalarAsync();

                    // If the count is greater than 0, the username or email is taken
                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        public async Task<bool> RegisterUser(UserDTO user)
        {

            if (await IsUsernameOrEmailTaken(user.Username, user.Email))
            {
                return false;
            }

            // Hash the password before storing it
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // SQL query to insert a new user into the database
            var sql = "INSERT INTO User (User_Username, User_FirstName, User_LastName, User_Email, User_Password, User_Level, User_Balance, User_BalanceAfterWin, User_Eye, User_LastLogin, User_PokerGameControl, User_PandoraGameControl, User_Verified, User_RegistrationTime) " +
                      "VALUES (@Username, @FirstName, @LastName, @Email, @Password, 1, 2000, 0, 0, datetime('now', 'localtime'), 5, 3, 0, datetime('now')) ";

            using (var connection = new SqliteConnection(_connectionString))
            {
                // Make sure the connection is open before starting the transaction
                await connection.OpenAsync();

                // Start the transaction after the connection is open
                var transaction = await connection.BeginTransactionAsync();

                try
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        // Associate the command with the transaction
                        command.Transaction = (SqliteTransaction?)transaction;

                        // Add parameters to the query
                        command.Parameters.AddWithValue("@Username", user.Username);
                        command.Parameters.AddWithValue("@FirstName", user.FirstName);
                        command.Parameters.AddWithValue("@LastName", user.LastName);
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@Password", hashedPassword);

                        // Execute the query
                        await command.ExecuteNonQueryAsync();
                    }

                    try
                    {
                        await transaction.CommitAsync();
                        await _emailSender.SendWelcomeMailAsync(user.Email, user.Username);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    // If an error occurs, rollback the transaction
                    await transaction.RollbackAsync();

                    // Log the error (you can use a logging system or debugging tool in a real production environment)
                    Console.WriteLine($"Error during user registration: {ex.Message}");

                    // Return false if something went wrong
                    return false;
                }
            }
        }

        public async Task<List<TopFiveRulersDTO>> GetTopFiveRulers()
        {
            var result = new List<TopFiveRulersDTO>();

            var sql = "SELECT User_Username, User_FirstName, User_LastName, User_BalanceAfterWin " +
                      "FROM User " +
                      "WHERE User_BalanceAfterWin > 0 " +
                      "ORDER BY User_BalanceAfterWin DESC " +
                      "LIMIT 5";

            // Use 'using' to ensure the connection is properly opened and disposed of
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqliteCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var user = new TopFiveRulersDTO
                            {
                                Username = reader.IsDBNull(reader.GetOrdinal("User_Username")) ? null : reader.GetString(reader.GetOrdinal("User_Username")),
                                FirstName = reader.IsDBNull(reader.GetOrdinal("User_FirstName")) ? null : reader.GetString(reader.GetOrdinal("User_FirstName")),
                                LastName = reader.IsDBNull(reader.GetOrdinal("User_LastName")) ? null : reader.GetString(reader.GetOrdinal("User_LastName")),
                                BalanceAfterWin = reader.IsDBNull(reader.GetOrdinal("User_BalanceAfterWin")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_BalanceAfterWin")),
                            };

                            result.Add(user);
                        }
                    }
                }
            }
            return result;
        }

        public async Task<List<TopFiveGentlemenDTO>> GetTopFiveGentlemen()
        {
            var result = new List<TopFiveGentlemenDTO>();

            var sql = "SELECT User_Username, User_FirstName, User_LastName, User_Balance, User_Level " +
                      "FROM User " +
                      "ORDER BY User_Balance DESC " +
                      "LIMIT 5";

            // Use 'using' to ensure the connection is properly opened and disposed of
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqliteCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var user = new TopFiveGentlemenDTO
                            {
                                Username = reader.IsDBNull(reader.GetOrdinal("User_Username")) ? null : reader.GetString(reader.GetOrdinal("User_Username")),
                                FirstName = reader.IsDBNull(reader.GetOrdinal("User_FirstName")) ? null : reader.GetString(reader.GetOrdinal("User_FirstName")),
                                LastName = reader.IsDBNull(reader.GetOrdinal("User_LastName")) ? null : reader.GetString(reader.GetOrdinal("User_LastName")),
                                Balance = reader.IsDBNull(reader.GetOrdinal("User_Balance")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_Balance")),
                                Level = reader.IsDBNull(reader.GetOrdinal("User_Level")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_Level")),
                            };

                            result.Add(user);
                        }
                    }
                }
            }
            return result;
        }

        public async Task<List<TopFifteenRulersDTO>> GetTopFifteenRulers()
        {
            var result = new List<TopFifteenRulersDTO>();

            var sql = "SELECT User_Username, User_FirstName, User_LastName, User_Email, User_Balance, User_BalanceAfterWin " +
                      "FROM User " +
                      "WHERE User_BalanceAfterWin > 0 " +
                      "ORDER BY User_BalanceAfterWin DESC " +
                      "LIMIT 14 OFFSET 1";

            // Use 'using' to ensure the connection is properly opened and disposed of
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqliteCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var user = new TopFifteenRulersDTO
                            {
                                Username = reader.IsDBNull(reader.GetOrdinal("User_Username")) ? null : reader.GetString(reader.GetOrdinal("User_Username")),
                                FirstName = reader.IsDBNull(reader.GetOrdinal("User_FirstName")) ? null : reader.GetString(reader.GetOrdinal("User_FirstName")),
                                LastName = reader.IsDBNull(reader.GetOrdinal("User_LastName")) ? null : reader.GetString(reader.GetOrdinal("User_LastName")),
                                Email = reader.IsDBNull(reader.GetOrdinal("User_Email")) ? null : reader.GetString(reader.GetOrdinal("User_Email")),
                                Balance = reader.IsDBNull(reader.GetOrdinal("User_Balance")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_Balance")),
                                BalanceAfterWin = reader.IsDBNull(reader.GetOrdinal("User_BalanceAfterWin")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_BalanceAfterWin")),
                            };

                            result.Add(user);
                        }
                    }
                }
            }
            return result;
        }


        public async Task<UserDTO> UltimateRuler()
        {
            UserDTO result = null;  // To store the top user

            var sql = "SELECT User_Username, User_FirstName, User_LastName, User_Email, User_Balance, User_BalanceAfterWin " +
                      "FROM User " +
                      "WHERE User_BalanceAfterWin > 0 " +
                      "ORDER BY User_BalanceAfterWin DESC " +
                      "LIMIT 1";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqliteCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            result = new UserDTO
                            {
                                Username = reader.IsDBNull(reader.GetOrdinal("User_Username")) ? null : reader.GetString(reader.GetOrdinal("User_Username")),
                                FirstName = reader.IsDBNull(reader.GetOrdinal("User_FirstName")) ? null : reader.GetString(reader.GetOrdinal("User_FirstName")),
                                LastName = reader.IsDBNull(reader.GetOrdinal("User_LastName")) ? null : reader.GetString(reader.GetOrdinal("User_LastName")),
                                Email = reader.IsDBNull(reader.GetOrdinal("User_Email")) ? null : reader.GetString(reader.GetOrdinal("User_Email")),
                                Balance = reader.IsDBNull(reader.GetOrdinal("User_Balance")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_Balance")),
                                BalanceAfterWin = reader.IsDBNull(reader.GetOrdinal("User_BalanceAfterWin")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_BalanceAfterWin")),
                            };
                        }
                    }
                }
            }
            return result;  // Return the top user (or null if no result)
        }

        public async Task<bool> UserChangePassword(int id, string currentPassword, string newPassword)
        {
            UserCurrentPasswordDTO currentPasswordModel = null;

            var sql = "SELECT User_ID, User_Password " +
                      "FROM User " +
                      "WHERE User_ID = @id";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            currentPasswordModel = new UserCurrentPasswordDTO
                            {
                                id = reader.IsDBNull(reader.GetOrdinal("User_ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("User_ID")),
                                currentPassword = reader.IsDBNull(reader.GetOrdinal("User_Password")) ? null : reader.GetString(reader.GetOrdinal("User_Password")),
                            };
                        }
                    }
                }
            }

            if (currentPasswordModel.id != 0 && currentPasswordModel.currentPassword != null)
            {
                if (BCrypt.Net.BCrypt.Verify(currentPassword, currentPasswordModel.currentPassword))
                {
                    var sqlUpdatePassword = "UPDATE User " +
                          "SET User_Password = @newPassword " +
                          "WHERE User_ID = @id";

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                    using (var connection = new SqliteConnection(_connectionString))
                    {
                        // Make sure the connection is open before starting the transaction
                        await connection.OpenAsync();

                        // Start the transaction after the connection is open
                        var transaction = await connection.BeginTransactionAsync();

                        try
                        {
                            using (var command = new SqliteCommand(sqlUpdatePassword, connection))
                            {
                                // Associate the command with the transaction
                                command.Transaction = (SqliteTransaction?)transaction;

                                // Add parameters to the query
                                command.Parameters.AddWithValue("@id", id);
                                command.Parameters.AddWithValue("@newPassword", hashedPassword);

                                // Execute the query
                                await command.ExecuteNonQueryAsync();
                                await transaction.CommitAsync();
                            }

                            return true;
                        }
                        catch (Exception ex)
                        {
                            // If an error occurs, rollback the transaction
                            await transaction.RollbackAsync();

                            // Log the error (you can use a logging system or debugging tool in a real production environment)
                            Console.WriteLine($"Error during user registration: {ex.Message}");

                            // Return false if something went wrong
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        public async Task<bool> RequestTokenForgottenPassword(string email)
        {
            UserCurrentPasswordDTO currentPasswordModel = null;

            var tokenCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%&?";

            var random = new Random();
            var token = new StringBuilder(8);

            for (int i = 0; i < 8; i++)
            {
                token.Append(tokenCharacters[random.Next(tokenCharacters.Length)]);
            }

            var sql = "UPDATE User " +
                          "SET User_PasswordToken = @token " +
                          "WHERE User_Email = @email";

            using (var connection = new SqliteConnection(_connectionString))
            {
                // Make sure the connection is open before starting the transaction
                await connection.OpenAsync();

                // Start the transaction after the connection is open
                var transaction = await connection.BeginTransactionAsync();

                try
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        // Associate the command with the transaction
                        command.Transaction = (SqliteTransaction?)transaction;

                        // Add parameters to the query
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@token", token.ToString());

                        // Execute the query
                        await command.ExecuteNonQueryAsync();
                        await transaction.CommitAsync();
                        await _emailSender.SendResetPasswordMailAsync(email, token.ToString());
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    // If an error occurs, rollback the transaction
                    await transaction.RollbackAsync();

                    // Log the error (you can use a logging system or debugging tool in a real production environment)
                    Console.WriteLine($"Error during user registration: {ex.Message}");

                    // Return false if something went wrong
                    return false;
                }
            }
        }


        public async Task<bool> ChangeForgottenPassword(string token, string newPassword)
        {
            // First, check if the token exists and is valid
            string checkTokenSql = "SELECT COUNT(*) FROM User WHERE User_PasswordToken = @token";

            // SQL to update password
            string updatePasswordSql = "UPDATE User " +
                                      "SET User_Password = @newPassword " +
                                      "WHERE User_PasswordToken = @token";

            // SQL to clear token after successful update
            string clearTokenSql = "UPDATE User " +
                                  "SET User_PasswordToken = NULL " +
                                  "WHERE User_PasswordToken = @token";

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                // First check if the token exists
                using (var checkCommand = new SqliteCommand(checkTokenSql, connection))
                {
                    checkCommand.Parameters.AddWithValue("@token", token);
                    int tokenCount = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                    if (tokenCount == 0)
                    {
                        Console.WriteLine("Password change failed: Invalid token or no matching user found.");
                        return false;
                    }
                }

                // Start transaction for the password update
                using var transaction = await connection.BeginTransactionAsync();
                try
                {
                    // Step 1: Update the password
                    using (var updateCommand = new SqliteCommand(updatePasswordSql, connection))
                    {
                        updateCommand.Transaction = transaction as SqliteTransaction;
                        updateCommand.Parameters.AddWithValue("@token", token);
                        updateCommand.Parameters.AddWithValue("@newPassword", hashedPassword);

                        int rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            // This shouldn't happen if our token check above is working,
                            // but it's good to have as a safety measure
                            await transaction.RollbackAsync();
                            Console.WriteLine("Password change failed: No rows updated.");
                            return false;
                        }
                    }

                    // Step 2: Clear the token after successful password update
                    using (var clearCommand = new SqliteCommand(clearTokenSql, connection))
                    {
                        clearCommand.Transaction = transaction as SqliteTransaction;
                        clearCommand.Parameters.AddWithValue("@token", token);
                        await clearCommand.ExecuteNonQueryAsync();
                    }

                    // Commit transaction if everything succeeded
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error during password change: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<bool> RequestVerificationToken(string email)
        {
            // First check if the email exists
            string checkEmailSql = "SELECT COUNT(*) FROM User WHERE User_Email = @email";
            
            var tokenCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%&?";
            var random = new Random();
            var token = new StringBuilder(8);

            for (int i = 0; i < 8; i++)
            {
                token.Append(tokenCharacters[random.Next(tokenCharacters.Length)]);
            }

            var updateSql = "UPDATE User " +
                          "SET User_VerificationToken = @token " +
                          "WHERE User_Email = @email ";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                // First check if the email exists
                using (var checkCommand = new SqliteCommand(checkEmailSql, connection))
                {
                    checkCommand.Parameters.AddWithValue("@email", email);
                    int emailCount = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                    if (emailCount == 0)
                    {
                        Console.WriteLine($"Verification token request failed: Email {email} not found.");
                        return false;
                    }
                }

                // Start the transaction after confirming email exists
                var transaction = await connection.BeginTransactionAsync();

                try
                {
                    using (var command = new SqliteCommand(updateSql, connection))
                    {
                        command.Transaction = (SqliteTransaction?)transaction;
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@token", token.ToString());

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        
                        if (rowsAffected == 0)
                        {
                            await transaction.RollbackAsync();
                            Console.WriteLine($"Failed to update verification token for email: {email}");
                            return false;
                        }

                        await transaction.CommitAsync();
                        
                        try
                        {
                            await _emailSender.RequestVerificationToken(email, token.ToString());
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to send verification email: {ex.Message}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error during verification token request: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<bool> VerifyUser(string token)
        {
            // First, check if the token exists and is valid
            string checkTokenSql = "SELECT COUNT(*) FROM User WHERE User_VerificationToken = @token";

            // SQL to update password
            string updatePasswordSql = "UPDATE User " +
                                      "SET User_Verified = 1, User_VerificationToken = NULL " +
                                      "WHERE User_VerificationToken = @token";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                // First check if the token exists
                using (var checkCommand = new SqliteCommand(checkTokenSql, connection))
                {
                    checkCommand.Parameters.AddWithValue("@token", token);
                    int tokenCount = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                    if (tokenCount == 0)
                    {
                        Console.WriteLine("Verification failed: Invalid token or no matching user found.");
                        return false;
                    }
                }

                // Start transaction for the password update
                using var transaction = await connection.BeginTransactionAsync();
                try
                {
                    // Step 1: Update the password
                    using (var updateCommand = new SqliteCommand(updatePasswordSql, connection))
                    {
                        updateCommand.Transaction = transaction as SqliteTransaction;
                        updateCommand.Parameters.AddWithValue("@token", token);

                        int rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            // This shouldn't happen if our token check above is working,
                            // but it's good to have as a safety measure
                            await transaction.RollbackAsync();
                            Console.WriteLine("Verification failed: No rows updated.");
                            return false;
                        }
                    }

                    // Commit transaction if everything succeeded
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error during verification: {ex.Message}");
                    return false;
                }
            }
        }
    }
}


