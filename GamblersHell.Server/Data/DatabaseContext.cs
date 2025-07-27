using Microsoft.Data.Sqlite;


namespace GamblersHell.Server.Data
{
    public static class DatabaseContext
    {
        public static void InitializeDatabase(string connectionString)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Create User table
                var createUserTable = @"
                    CREATE TABLE IF NOT EXISTS User (
                        User_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        User_Username TEXT NOT NULL UNIQUE,
                        User_Password TEXT NOT NULL,
                        User_Email TEXT NOT NULL UNIQUE,
                        User_Balance INTEGER NOT NULL DEFAULT 1000,
                        User_Level INTEGER NOT NULL DEFAULT 1,
                        User_Eye INTEGER NOT NULL DEFAULT 0,
                        User_BalanceAfterWin INTEGER NOT NULL DEFAULT 0,
                        User_PokerGameControl INTEGER NOT NULL DEFAULT 5,
                        User_PandoraGameControl INTEGER NOT NULL DEFAULT 3,
                        User_Verified INTEGER NOT NULL DEFAULT 0
                    );";

                // Create GameSession table
                var createGameSessionTable = @"
                    CREATE TABLE IF NOT EXISTS GameSession (
                        Session_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        User_ID INTEGER NOT NULL,
                        Game_Type TEXT NOT NULL,
                        Session_Token TEXT NOT NULL,
                        Created_At DATETIME NOT NULL,
                        Is_Valid INTEGER NOT NULL DEFAULT 1,
                        FOREIGN KEY (User_ID) REFERENCES User(User_ID)
                    );";

                using (var command = new SqliteCommand(createUserTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SqliteCommand(createGameSessionTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
} 