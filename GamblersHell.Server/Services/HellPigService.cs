using System.Data;
using System.Text.Json;
using GamblersHell.Shared;
using Microsoft.Data.Sqlite;


namespace GamblersHell.Server.Services
{
    public class HellPigService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _connectionString;

        public HellPigService(IWebHostEnvironment webHostEnvironment, string connectionString)
        {
            _webHostEnvironment = webHostEnvironment;
            _connectionString = connectionString;
        }

        public async Task<List<HellPigDTO>> GetPigSigns()
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "HellPigJson", "HellPigRewards.json");
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var slots = await JsonSerializer.DeserializeAsync<List<HellPigDTO>>(fileStream);
            return slots;
        }

        public async Task<HellPigDTO> GetRandomPigSlot()
        {
            var slots = await GetPigSigns();
            var random = new Random();
            int randomIndex = random.Next(0, slots.Count);
            return slots[randomIndex];
        }

        public async Task<bool> DailyReward(int id)
        {
            var sql = "SELECT User_ID, User_LastLogin FROM User WHERE User_ID = @id";

            UserLastLoginDTO user = new();

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
                            user.ID = reader.GetInt32(reader.GetOrdinal("User_ID"));
                            user.LastLoginDate = reader.IsDBNull("User_LastLogin") ? null : reader.GetDateTime("User_LastLogin");
                        }
                    }
                }
            }

            if (user != null)
            {
                if (user.LastLoginDate.Value.Date != DateTime.Today)
                {
                    Console.WriteLine("You can collect your daily reward");
                    return true;
                }
                else
                {
                    Console.WriteLine("You collected your daily reward");
                    return false;
                }
            }
            Console.WriteLine("No user found!");
            return false;
        }

        public async Task<bool> DailyRewardPrice(int id, int priceValue)
        {
            var sql = "UPDATE User SET User_Balance = User_Balance + @priceValue, User_LastLogin = datetime('now', 'localtime') WHERE User_ID = @id";
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
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
    }
}
