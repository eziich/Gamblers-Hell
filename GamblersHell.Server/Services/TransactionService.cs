using GamblersHell.Shared;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

namespace GamblersHell.Server.Services
{
    public class TransactionService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _connectionString;
        private readonly UserService _userService;

        public TransactionService(IWebHostEnvironment webHostEnvironment, string connectionString, UserService userService)
        {
            _webHostEnvironment = webHostEnvironment;
            _connectionString = connectionString;
            _userService = userService;
        }

        private string GenerateGameSessionToken(int userId, string gameType)
        {
            using (var sha256 = SHA256.Create())
            {
                var timestamp = DateTime.UtcNow.Ticks;
                var input = $"{userId}:{gameType}:{timestamp}";
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hash);
            }
        }

        public async Task<string> CreateGameSession(int userId, string gameType)
        {
            var token = GenerateGameSessionToken(userId, gameType);
            var sql = @"
                INSERT INTO GameSession (User_ID, Game_Type, Session_Token, Created_At, Is_Valid)
                VALUES (@userId, @gameType, @token, @createdAt, 1)";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();

                try
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Transaction = (SqliteTransaction?)transaction;
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@gameType", gameType);
                        command.Parameters.AddWithValue("@token", token);
                        command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
                        await command.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                    return token;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    return string.Empty;
                }
            }
        }

        public async Task<bool> ValidateGameSession(int userId, string gameType, string token)
        {
            var sql = @"
                SELECT COUNT(*) FROM GameSession 
                WHERE User_ID = @userId 
                AND Game_Type = @gameType 
                AND Session_Token = @token 
                AND Is_Valid = 1 
                AND Created_At > @expiryTime";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@gameType", gameType);
                    command.Parameters.AddWithValue("@token", token);
                    command.Parameters.AddWithValue("@expiryTime", DateTime.UtcNow.AddMinutes(-30)); // 30 minute expiry

                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        public async Task<string> GameWonTransaction(int id, int priceValue, int level, string gameType, string sessionToken)
        {
            // Validate game session first
            if (!await ValidateGameSession(id, gameType, sessionToken))
            {
                return "Invalid or expired game session!";
            }

            string returnMessage = string.Empty;
            var selectedUser = await _userService.GetUserByID(id);

            if (selectedUser == null)
            {
                return "User not found!";
            }

            var sql = string.Empty;

            if (level == 10)
            {
                sql = "UPDATE User SET User_Balance = User_Balance * 3, User_Level = @level, User_BalanceAfterWin = User_Balance * 3 WHERE User_ID = @id";
            }
            else
            {
                if (selectedUser.Level < level)
                {
                    sql = "UPDATE User SET User_Balance = User_Balance + (2 * @priceValue), User_Level = @level WHERE User_ID = @id";
                }
                else
                {
                    sql = "UPDATE User SET User_Balance = User_Balance + (2 * @priceValue) WHERE User_ID = @id";
                }
            }

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();

                try
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Transaction = (SqliteTransaction?)transaction;
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@priceValue", priceValue);

                        if (selectedUser.Level < level)
                        {
                            command.Parameters.AddWithValue("@level", level);
                        }

                        await command.ExecuteNonQueryAsync();
                    }

                    // Invalidate the game session after successful transaction
                    var invalidateSql = "UPDATE GameSession SET Is_Valid = 0 WHERE User_ID = @id AND Game_Type = @gameType AND Session_Token = @token";
                    using (var command = new SqliteCommand(invalidateSql, connection))
                    {
                        command.Transaction = (SqliteTransaction?)transaction;
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@gameType", gameType);
                        command.Parameters.AddWithValue("@token", sessionToken);
                        await command.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                    return "Successful!";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    return "Unsuccessful!";
                }
            }
        }

        public async Task<string> GameLostBetTransaction(int id, int priceValue)
        {
            string returnMessage = string.Empty;

            var sql = "UPDATE User SET User_Balance = User_Balance - @priceValue WHERE User_ID = @id";
            UserLastLoginDTO user = new();

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();
                try
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        // Dont forget to declare the transaction
                        command.Transaction = (SqliteTransaction?)transaction;

                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@priceValue", priceValue);
                        await command.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                    return returnMessage = "Successful!";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    return returnMessage = "Unsuccessful!";
                }
            }
        }

        public async Task<string> BeastRaceTheEye(int id)
        {
            string returnMessage = "";
            var selectedUser = await _userService.GetUserByID(id);

            if (selectedUser == null)
            {
                return "User not found!";
            }

            string sql = "";

            if (selectedUser != null)
            {
                if (selectedUser.Eye == 0 && selectedUser.BalanceAfterWin == 0)
                {
                    sql = "UPDATE User SET User_Eye = 1 WHERE User_ID = @id";
                }
                else if (selectedUser.BalanceAfterWin > 0)
                {
                    sql = "UPDATE User SET User_Eye = 0 WHERE User_ID = @id";
                }

                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var transaction = await connection.BeginTransactionAsync();
                    try
                    {
                        using (var command = new SqliteCommand(sql, connection))
                        {
                            command.Transaction = (SqliteTransaction?)transaction;
                            command.Parameters.AddWithValue("@id", id);
                            await command.ExecuteNonQueryAsync();
                        }
                        await transaction.CommitAsync();
                        return returnMessage = "Successful!";
                    }
                    catch
                    {
                        transaction.Rollback();
                        return returnMessage = "Unsuccessful!";
                        throw;
                    }
                }
            }
            else
            {
                return returnMessage = "User not found!";
            }
        }

        public async Task<string> ClaimTheEye(int id)
        {
            string returnMessage = "";
            var selectedUser = await _userService.GetUserByID(id);

            if (selectedUser == null)
            {
                return "User not found!";
            }

            string sql = "";

            if (selectedUser != null && selectedUser.Eye == 1)
            {
                sql = "UPDATE User SET User_Eye = 0, User_Balance = User_Balance + 1000 WHERE User_ID = @id";

                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var transaction = await connection.BeginTransactionAsync();
                    try
                    {
                        using (var command = new SqliteCommand(sql, connection))
                        {
                            command.Transaction = (SqliteTransaction?)transaction;
                            command.Parameters.AddWithValue("@id", id);
                            await command.ExecuteNonQueryAsync();
                        }
                        await transaction.CommitAsync();
                        return returnMessage = "Successful!";
                    }
                    catch
                    {
                        transaction.Rollback();
                        return returnMessage = "Unsuccessful!";
                        throw;
                    }
                }
            }
            else
            {
                return returnMessage = "User not found or The Eye prize has been claimed!";
            }
        }

        public async Task<bool> PokerGameControlReset(int id)
        {
            var sql = "UPDATE User Set User_PokerGameControl = 5 WHERE User_ID = @id";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();
                try
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Transaction = (SqliteTransaction?)transaction;
                        command.Parameters.AddWithValue("@id", id);
                        await command.ExecuteNonQueryAsync();
                    }
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> PokerGameControlLost(int id)
        {
            var sql = "UPDATE User Set User_PokerGameControl = User_PokerGameControl - 1 WHERE User_ID = @id";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();
                try
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Transaction = (SqliteTransaction?)transaction;
                        command.Parameters.AddWithValue("@id", id);
                        await command.ExecuteNonQueryAsync();
                    }
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> PokerGameControlWon(int id)
        {
            var sql = "UPDATE User Set User_PokerGameControl = User_PokerGameControl + 1 WHERE User_ID = @id";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();
                try
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Transaction = (SqliteTransaction?)transaction;
                        command.Parameters.AddWithValue("@id", id);
                        await command.ExecuteNonQueryAsync();
                    }
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> PandoraGameControlReset(int id)
        {
            var sql = "UPDATE User Set User_PandoraGameControl = 3 WHERE User_ID = @id";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();
                try
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Transaction = (SqliteTransaction?)transaction;
                        command.Parameters.AddWithValue("@id", id);
                        await command.ExecuteNonQueryAsync();
                    }
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> PandoraGameControlLost(int id)
        {
            var sql = "UPDATE User Set User_PandoraGameControl = User_PandoraGameControl - 1 WHERE User_ID = @id";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();
                try
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Transaction = (SqliteTransaction?)transaction;
                        command.Parameters.AddWithValue("@id", id);
                        await command.ExecuteNonQueryAsync();
                    }
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> PandoraGameControlWon(int id)
        {
            var sql = "UPDATE User Set User_PandoraGameControl = User_PandoraGameControl + 1 WHERE User_ID = @id";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();
                try
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Transaction = (SqliteTransaction?)transaction;
                        command.Parameters.AddWithValue("@id", id);
                        await command.ExecuteNonQueryAsync();
                    }
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> LostToBaal(int id)
        {
            var random = new Random();

            int backToLevel = random.Next(3, 7); 

            var sql = "UPDATE User SET User_Level = @backToLevel WHERE User_ID = @id";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();
                try
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Transaction = (SqliteTransaction?)transaction;
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@backToLevel", backToLevel);
                        await command.ExecuteNonQueryAsync();
                    }
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> LostToLilim(int id)
        {
            var selectedUser = await _userService.GetUserByID(id);
            int decreaseValue = 0;

            if (selectedUser == null)
            {
                return false;
            }

            if (selectedUser.Balance >= 1500 && selectedUser.Balance < 2000)
            {
                decreaseValue = 100;
            }

            else if (selectedUser.Balance >= 2000 && selectedUser.Balance < 4000)
            {
                decreaseValue = 350;
            }

            else if (selectedUser.Balance >= 4000 && selectedUser.Balance < 7000)
            {
                decreaseValue = 550;
            }

            else if (selectedUser.Balance > 7000 && selectedUser.Balance < 10000)
            {
                decreaseValue = 750;
            }

            else if (selectedUser.Balance > 10000)
            {
                decreaseValue = 1000;
            }

            var sql = "UPDATE User SET User_Balance = User_Balance - @decreaseValue WHERE User_ID = @id";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();
                try
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Transaction = (SqliteTransaction?)transaction;
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@decreaseValue", decreaseValue);
                        await command.ExecuteNonQueryAsync();
                    }
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> LostToSatan(int id)
        {
            // First, delete all GameSession rows for this user
            var deleteSessionsSql = "DELETE FROM GameSession WHERE User_ID = @id";
            var deleteUserSql = "DELETE FROM User WHERE User_ID = @id";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();
                try
                {
                    // Delete game sessions
                    using (var command = new SqliteCommand(deleteSessionsSql, connection))
                    {
                        command.Transaction = (SqliteTransaction?)transaction;
                        command.Parameters.AddWithValue("@id", id);
                        await command.ExecuteNonQueryAsync();
                    }

                    // Delete user
                    using (var command = new SqliteCommand(deleteUserSql, connection))
                    {
                        command.Transaction = (SqliteTransaction?)transaction;
                        command.Parameters.AddWithValue("@id", id);
                        await command.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}
