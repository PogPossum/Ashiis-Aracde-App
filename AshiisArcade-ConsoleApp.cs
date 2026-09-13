using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;

namespace AshiisArcadeConsole
{
    internal class Program
    {
        static string connString = @"Server=x.x.x.x,1433;Database=ArcadeDB;User Id=User;Password=Password1;TrustServerCertificate=True;";
        static void Main(string[] args)
        {
            MainMenu();
        }
        // menus
        static void MainMenu()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.Write("\x1b[3J\x1b[H\x1b[2J");
                Console.Clear();
                Console.WriteLine("                 ___         __    _ _ _      \r\n                /   |  _____/ /_  (_|_| )_____\r\n               / /| | / ___/ __ \\/ / /|// ___/\r\n              / ___ |(__  ) / / / / /  (__  ) \r\n           __/_/  |_/____/_/ /_/_/_/  /____/  \r\n          /   |  ______________ _____/ /__    \r\n         / /| | / ___/ ___/ __ `/ __  / _ \\   \r\n        / ___ |/ /  / /__/ /_/ / /_/ /  __/   \r\n       /_/  |_/_/   \\___/\\__,_/\\__,_/\\___/    \r\n                                              ");
                Console.WriteLine(" ");
                Console.WriteLine($"                  --- MAIN MENU ---");
                Console.WriteLine("======================================================");
                Console.WriteLine($"SYSTEM TIME: {DateTime.Now:dd/MM/yyyy HH:mm} | STATUS: OPERATIONAL");
                Console.WriteLine("======================================================");
                Console.WriteLine(" [1] All entries           [4] Games counter");
                Console.WriteLine(" [2] Search for game       [5] Oldest/Newest releases");
                Console.WriteLine(" [3] Search for console    [6] Timespan search");
                Console.WriteLine(" ");
                Console.WriteLine(" [9] Creation Menu         [0] Exit App");
                Console.WriteLine("======================================================");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AllEntries();
                        break;
                    case "2":
                        SearchForGame();
                        break;
                    case "3":
                        SearchForConsole();
                        break;
                    case "4":
                        GamesCounter();
                        break;
                    case "5":
                        OldestNewest();
                        break;
                    case "6":
                        PeriodSearch();
                        break;
                    case "9": // goes to Creation Menu
                        CreationMenu();
                        break;
                    case "0": // exits app
                        running = false;
                        Console.WriteLine("Shutting down...");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again..");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void CreationMenu()
        {
            bool running = true;

            while (running)
            {

                Console.Clear();
                Console.Write("\x1b[3J\x1b[H\x1b[2J");
                Console.Clear();
                Console.WriteLine("                 ___         __    _ _ _      \r\n                /   |  _____/ /_  (_|_| )_____\r\n               / /| | / ___/ __ \\/ / /|// ___/\r\n              / ___ |(__  ) / / / / /  (__  ) \r\n           __/_/  |_/____/_/ /_/_/_/  /____/  \r\n          /   |  ______________ _____/ /__    \r\n         / /| | / ___/ ___/ __ `/ __  / _ \\   \r\n        / ___ |/ /  / /__/ /_/ / /_/ /  __/   \r\n       /_/  |_/_/   \\___/\\__,_/\\__,_/\\___/    \r\n                                              ");
                Console.WriteLine(" ");
                Console.WriteLine($"                --- CREATION MENU ---");
                Console.WriteLine("======================================================");
                Console.WriteLine($"SYSTEM TIME: {DateTime.Now:dd/MM/yyyy HH:mm} | STATUS: OPERATIONAL");
                Console.WriteLine("======================================================");
                Console.WriteLine(" [1] Add game              [5] Add games in bulk");
                Console.WriteLine(" [2] Add console           [6] Delete Games");
                Console.WriteLine(" [3] Select * from Games   [7] Delete Consoles");
                Console.WriteLine(" [4] Select * from Console [8] Console IDs");
                Console.WriteLine(" ");
                Console.WriteLine(" [9] Return to previous    [0] Exit App");
                Console.WriteLine("======================================================");

                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddNewGame();
                        break;
                    case "2":
                        AddNewConsole();
                        break;
                    case "3":
                        ShowGameEntries();
                        break;
                    case "4":
                        ShowConsoleEntries();
                        break;
                    case "5":
                        AddGamesBulk();
                        break;
                    case "6":
                        DeleteGames();
                        break;
                    case "7":
                        DeleteConsoles();
                        break;
                    case "8":
                        ConsoleIDs();
                        break;
                    case "9":
                        return; // returns to Main Menu
                    case "0":
                        Environment.Exit(0); // exits app
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again..");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // main menu functions
        static void AllEntries()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("                --- All Entries ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" ");
            Console.WriteLine("Title                                         | Console         | Release");
            Console.WriteLine("-------------------------------------------------------------------------");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    // 1. Added "order by" so that the database groups the consoles together for us
                    string sql = @"
                select Game.Title, Console.Console, Game.Release
                from [ArcadeBlockade].[dbo].[Game] join [ArcadeBlockade].[dbo].[Console]
                on Game.ConID = Console.ConID
                order by Console.ConID, Game.Title asc;";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // 2. Track the active console name
                        string currentConsole = "";

                        while (reader.Read())
                        {
                            string consoleName = reader["Console"].ToString().Trim();

                            // 3. If the console changes, drop in a separator line
                            if (!string.IsNullOrEmpty(currentConsole) && currentConsole != consoleName)
                            {
                                Console.WriteLine("-------------------------------------------------------------------------");
                            }

                            currentConsole = consoleName;

                            Console.WriteLine($"{reader["Title"].ToString().Trim().PadRight(45)} | {consoleName.PadRight(15)} | {reader["Release"]}");
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            }
            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }
        static void SearchForGame()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("                --- Game Search ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" search Games with only a part of the name needed");
            Console.WriteLine(" Example: 'Petshop' for Littlest Petshop");
            Console.WriteLine("----------------------------------------------------");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                        select Game.Title, Console.Console, Game.Release
                        from [ArcadeBlockade].[dbo].[Game] join [ArcadeBlockade].[dbo].[Console]
                        on Game.ConID = Console.ConID
                        where Game.Title like '%' + @search + '%'
                        order by Release asc";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        Console.Write("Enter game title to search: ");
                        string searchTerm = Console.ReadLine();
                        cmd.Parameters.AddWithValue("@search", searchTerm);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader["Title"].ToString().Trim().PadRight(45)} | {reader["Console"].ToString().Trim().PadRight(15)} | {reader["Release"]}");
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            }
            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }
        static void SearchForConsole()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("              --- Console Search ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" search games by Consoles! ");
            Console.WriteLine(" Example: 'Gameboy' to get all Gameboy consoles");
            Console.WriteLine("----------------------------------------------------");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                        select g.Title, c.Console, g.Release
                        from [ArcadeBlockade].[dbo].[Game] g
                        join [ArcadeBlockade].[dbo].[Console] c on g.ConID = c.ConID
                        where c.Console like '%' + @search + '%'
                        order by Release asc";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        Console.Write("Enter console name to search: ");
                        string searchTerm = Console.ReadLine();
                        cmd.Parameters.AddWithValue("@search", searchTerm);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader["Title"].ToString().Trim().PadRight(45)} | {reader["Console"].ToString().Trim().PadRight(15)} | {reader["Release"]}");
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            }
            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }
        static void GamesCounter()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("                --- Games Counter ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" With this we can see how many games we have");
            Console.WriteLine(" Pr Console");
            Console.WriteLine("----------------------------------------------------");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                select
                    c.Console, 
                    count(g.Title) AS TotalGames
                from [ArcadeBlockade].[dbo].[Console] c
                left join [ArcadeBlockade].[dbo].[Game] g on c.ConID = g.ConID
                group by c.Console
                order by TotalGames desc;";

                    // 1. Initialize a counter variable
                    int totalGamesSum = 0;

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // 2. Safely parse the current console's count and add it to the sum
                            int currentConsoleCount = Convert.ToInt32(reader["TotalGames"]);
                            totalGamesSum += currentConsoleCount;

                            Console.WriteLine($"{reader["Console"].ToString().Trim().PadRight(30)} | {currentConsoleCount}");
                        }
                    }

                    // 3. Print the grand total right after the loop finishes
                    Console.WriteLine("----------------------------------------------------");
                    Console.WriteLine($"{"Games in Total".PadRight(30)} | {totalGamesSum}");

                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            }

            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }

        static void OldestNewest()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("           --- Oldest & Newest Games ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" Here we see oldest & newest releases!");
            Console.WriteLine("----------------------------------------------------");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                with RankedGames as (
                    select
                        Game.Title, 
                        Console.Console, 
                        Game.Release,
                        row_number() over(partition by Console.ConID order by Game.Release asc) as oldest_rank,
                        row_number() over(partition by Console.ConID order by Game.Release desc) as newest_rank
                    from [ArcadeBlockade].[dbo].[Game]
                    join [ArcadeBlockade].[dbo].[Console] on Game.ConID = Console.ConID
                )
                select 
                    Console, 
                    Title, 
                    Release,
                    case 
                        when oldest_rank = 1 then 'Oldest' 
                        when newest_rank = 1 then 'Newest' 
                    end as status 
                from RankedGames
                where oldest_rank = 1 or newest_rank = 1
                order by Console, Release;";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Creates a variable to keep track of last console
                        string currentConsole = "";

                        while (reader.Read())
                        {
                            string consoleName = reader["Console"].ToString().Trim();

                            // 2. If not first console, AND it's different from the last one,
                            // print the divider before printing the new group.
                            if (!string.IsNullOrEmpty(currentConsole) && currentConsole != consoleName)
                            {
                                Console.WriteLine("------------ ------------ ------------ ------------ ------------ ------------");
                            }

                            // 3. Update our tracker variable to the current console name
                            currentConsole = consoleName;

                            Console.WriteLine($"{consoleName.PadRight(30)} | {reader["Title"].ToString().Trim().PadRight(45)} | {reader["Release"]} | {reader["status"]}");
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            }
            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }
        static void PeriodSearch()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("               --- Period Search ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" With this we can search for games over a timespan");
            Console.WriteLine(" Example: 1990 to 2000 will show games released in the 90s");
            Console.WriteLine("----------------------------------------------------");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                        select Game.Title, Console.Console, Game.Release
                        from [ArcadeBlockade].[dbo].[Game] join [ArcadeBlockade].[dbo].[Console]
                        on Game.ConID = Console.ConID
                        where Game.Release between @start and @end
                        order by Release asc";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        Console.Write("Enter start year: ");
                        int startYear = int.Parse(Console.ReadLine());
                        Console.Write("Enter end year: ");
                        int endYear = int.Parse(Console.ReadLine());
                        cmd.Parameters.AddWithValue("@start", startYear);
                        cmd.Parameters.AddWithValue("@end", endYear);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader["Title"].ToString().Trim().PadRight(45)} | {reader["Console"].ToString().Trim().PadRight(15)} | {reader["Release"]}");
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            }
            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }

        // creation menu functions
        static void AddNewGame()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("               --- Add New Game ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" Enter the game details below:");
            Console.WriteLine("----------------------------------------------------");

            // 1. Gather plain text inputs from the user
            Console.Write("Enter Game Title: ");
            string title = Console.ReadLine().Trim();

            Console.Write("Enter Console Name: ");
            string consoleInput = Console.ReadLine().Trim();

            Console.Write("Enter Release Year: ");
            string releaseInput = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(consoleInput) || string.IsNullOrEmpty(releaseInput))
            {
                Console.WriteLine("\nError: All fields are required!");
                FinishConsolePrompt();
                return;
            }

            if (!int.TryParse(releaseInput, out int releaseYear))
            {
                Console.WriteLine("\nError: Release year must be a valid number!");
                FinishConsolePrompt();
                return;
            }

            // 2. Connect to the database to fetch the ConID and perform the insertion
            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    // Step A: Look up the Console ID based on the text string typed by the user
                    int? conID = null;
                    string lookupSql = "select ConID from [Console] where Console = @ConsoleName";

                    using (SqlCommand lookupCmd = new SqlCommand(lookupSql, connection))
                    {
                        lookupCmd.Parameters.AddWithValue("@ConsoleName", consoleInput);
                        object result = lookupCmd.ExecuteScalar(); // Safely pulls back just one single value

                        if (result != null && result != DBNull.Value)
                        {
                            conID = Convert.ToInt32(result);
                        }
                    }

                    // Step B: If the console wasn't found, stop and notify the user
                    if (conID == null)
                    {
                        Console.WriteLine($"\nError: Console '{consoleInput}' could not be found in the database!");
                        FinishGamePrompt();
                        return;
                    }

                    // Step C: Insert the game using the resolved ConID
                    string insertSql = "insert into Game (Title, ConID, Release) values (@Title, @ConID, @Release)";
                    using (SqlCommand insertCmd = new SqlCommand(insertSql, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@Title", title);
                        insertCmd.Parameters.AddWithValue("@ConID", conID.Value);
                        insertCmd.Parameters.AddWithValue("@Release", releaseYear);

                        insertCmd.ExecuteNonQuery();
                        Console.WriteLine($"\nSuccess! '{title}' has been added to your collection under ID {conID.Value}.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nDatabase Error: " + ex.Message);
                }
            }

            FinishGamePrompt();
        }

        // keeps the bottom clean
        static void FinishGamePrompt()
        {
            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }
        // inserts the matched entries into the database
        static void AddNewConsole()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.WriteLine("             --- Add New Console ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" Enter the console details below:");
            Console.WriteLine("----------------------------------------------------");

            // 1. Collect inputs step-by-step (Added Release Year prompt)
            Console.Write("Enter Console ID (e.g., 402): ");
            string idInput = Console.ReadLine().Trim();

            Console.Write("Enter Console Name (e.g., Playstation 2): ");
            string consoleName = Console.ReadLine().Trim();

            Console.Write("Enter Company (e.g., Sony): ");
            string company = Console.ReadLine().Trim();

            Console.Write("Enter Console Release Year (e.g., 2000): ");
            string releaseInput = Console.ReadLine().Trim();

            // 2. Validate inputs
            if (string.IsNullOrEmpty(idInput) || string.IsNullOrEmpty(consoleName) || string.IsNullOrEmpty(company) || string.IsNullOrEmpty(releaseInput))
            {
                Console.WriteLine("\nError: All fields are required!");
                FinishConsolePrompt();
                return;
            }

            if (!int.TryParse(idInput, out int conID))
            {
                Console.WriteLine("\nError: Console ID must be a valid number!");
                FinishConsolePrompt();
                return;
            }

            if (!int.TryParse(releaseInput, out int releaseYear))
            {
                Console.WriteLine("\nError: Release year must be a valid number!");
                FinishConsolePrompt();
                return;
            }

            // 3. Insert into the database
            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    // Updated SQL statement to include the Release column
                    string sql = "insert into Console (ConID, Console, Company, Release) values (@ConID, @Console, @Company, @Release)";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@ConID", conID);
                        cmd.Parameters.AddWithValue("@Console", consoleName);
                        cmd.Parameters.AddWithValue("@Company", company);
                        cmd.Parameters.AddWithValue("@Release", releaseYear); // Added parameter mapping

                        cmd.ExecuteNonQuery();
                        Console.WriteLine($"\nSuccess! '{consoleName}' ({releaseYear}) has been added to your database under ID {conID}.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nDatabase Error: " + ex.Message);
                }
            }

            FinishConsolePrompt();
        }

        // keeps the bottom clean
        static void FinishConsolePrompt()
        {
            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }
        static void ShowConsoleEntries()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("           --- Console Details ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" This shows all console details!");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("CID | Console                   | Company");
            Console.WriteLine("----------------------------------------------------");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                        select *
                        from [ArcadeBlockade].[dbo].[Console] ";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["ConID"].ToString().Trim()} | {reader["Console"].ToString().Trim().PadRight(25)} | {reader["Company"].ToString().Trim().PadRight(55)}");
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            }
            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }
        static void ShowGameEntries()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("             --- Game Details ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" This shows all game details!");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("ID  | Title                                         | CID | Release");
            Console.WriteLine("-------------------------------------------------------------------");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                        select *
                        from [ArcadeBlockade].[dbo].[Game] ";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["ID"]} | {reader["Title"].ToString().Trim().PadRight(45)} | {reader["ConID"].ToString().Trim()} | {reader["Release"]}");
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            }
            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }
        static void AddGamesBulk()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.WriteLine("             --- Bulk Add New Games ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" --- Paste SQL-Style Values (Multiple Lines) ---");
            Console.WriteLine(" Format: ('Title', Console Name, Release Year),");
            Console.WriteLine(" Example: ('Sonic', Sega Genesis, 1991),");
            Console.WriteLine(" Add games and press ENTER on a blank line to finish:");
            Console.WriteLine("----------------------------------------------------");

            StringBuilder sb = new StringBuilder();
            string line;

            // Grab lines until hitting an empty line
            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
            {
                sb.AppendLine(line);
            }

            string input = sb.ToString();

            // Regular expression updated to capture the Console NAME as text instead of a strict digit
            string pattern = @"\(\s*'(?<Title>.+?)'\s*,\s*(?<ConsoleName>[^,]+)\s*,\s*(?<Release>\d+)\s*\)";
            MatchCollection matches = Regex.Matches(input, pattern, RegexOptions.Singleline);

            if (matches.Count == 0)
            {
                Console.WriteLine("\nNo valid entries found. Check your formatting!");
                FinishBulkPrompt();
                return;
            }

            // Process entries and map Console Names to real Database IDs
            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    int successCount = 0;
                    int failureCount = 0;

                    foreach (Match match in matches)
                    {
                        string title = match.Groups["Title"].Value.Trim();
                        string consoleName = match.Groups["ConsoleName"].Value.Trim();
                        string releaseStr = match.Groups["Release"].Value.Trim();

                        // 1. Quick lookup to turn "Sega Genesis" (or whatever was typed) into a ConID
                        int? conID = null;
                        string lookupSql = "select ConID from [Console] where Console = @ConsoleName";

                        using (SqlCommand lookupCmd = new SqlCommand(lookupSql, connection))
                        {
                            lookupCmd.Parameters.AddWithValue("@ConsoleName", consoleName);
                            object result = lookupCmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                conID = Convert.ToInt32(result);
                            }
                        }

                        // 2. If the console exists, insert the game
                        if (conID != null)
                        {
                            string insertSql = "insert into Game (Title, ConID, Release) values (@Title, @ConID, @Release)";
                            using (SqlCommand insertCmd = new SqlCommand(insertSql, connection))
                            {
                                insertCmd.Parameters.AddWithValue("@Title", title);
                                insertCmd.Parameters.AddWithValue("@ConID", conID.Value);
                                insertCmd.Parameters.AddWithValue("@Release", int.Parse(releaseStr));

                                insertCmd.ExecuteNonQuery();
                                successCount++;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Skipped: '{title}' -> Console '{consoleName}' not found in DB.");
                            failureCount++;
                        }
                    }

                    Console.WriteLine($"\nProcessing Complete!");
                    Console.WriteLine($"Successfully added: {successCount} games.");
                    if (failureCount > 0)
                    {
                        Console.WriteLine($"Skipped/Failed: {failureCount} games due to missing console names.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nDatabase Error: " + ex.Message);
                }
            }

            FinishBulkPrompt();
        }
        static void FinishBulkPrompt()
        {
            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }
        static void DeleteGames()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("           --- Delete Games by Title ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" Enter titles separated by commas or new lines.");
            Console.WriteLine(" Example: 'Pac-Man', 'Digimon World'");
            Console.WriteLine(" Press ENTER on a blank line to finish:");
            Console.WriteLine("----------------------------------------------------");

            StringBuilder sb = new StringBuilder();
            string line;
            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
            {
                sb.AppendLine(line);
            }

            // Clean up the input into a list of titles
            var titlesToDelete = sb.ToString()
                .Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().Trim('\'')) // Removes spaces and wrapping quotes
                .ToList();

            if (titlesToDelete.Count == 0) return;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    // --- STEP 1: THE SAFEGUARD (PREVIEW) ---
                    Console.WriteLine("\nReviewing database for matches...");
                    List<string> foundGames = new List<string>();

                    foreach (var title in titlesToDelete)
                    {
                        string checkSql = "SELECT Title, Release FROM Game WHERE RTRIM(Title) = @T";
                        using (SqlCommand checkCmd = new SqlCommand(checkSql, connection))
                        {
                            checkCmd.Parameters.AddWithValue("@T", title);
                            using (SqlDataReader reader = checkCmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    foundGames.Add($"{reader["Title"].ToString().Trim()} ({reader["Release"]})");
                                }
                            }
                        }
                    }

                    if (foundGames.Count == 0)
                    {
                        Console.WriteLine("No matching games found in the database.");
                        return;
                    }

                    Console.WriteLine("\nTHE FOLLOWING ENTRIES WILL BE DELETED:");
                    foreach (var game in foundGames) Console.WriteLine($"- {game}");

                    // --- STEP 2: THE CONFIRMATION ---
                    Console.Write("\nAre you absolutely sure? (Type 'YES' to confirm): ");
                    string confirm = Console.ReadLine().ToUpper();

                    if (confirm == "YES")
                    {
                        int totalDeleted = 0;
                        foreach (var title in titlesToDelete)
                        {
                            string deleteSql = "delete from Game where rtrim(Title) = @T";
                            using (SqlCommand delCmd = new SqlCommand(deleteSql, connection))
                            {
                                delCmd.Parameters.AddWithValue("@T", title);
                                totalDeleted += delCmd.ExecuteNonQuery();
                            }
                        }
                        Console.WriteLine($"\nSuccess! {totalDeleted} games removed.");
                    }
                    else
                    {
                        Console.WriteLine("\nOperation cancelled. No data was harmed.");
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            }
            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }
        static void DeleteConsoles()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("          --- Delete Consoles by Name ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" Enter names separated by commas or new lines.");
            Console.WriteLine(" Example: 'Playstation 2', 'Xbox One'");
            Console.WriteLine(" Press ENTER on a blank line to finish:");
            Console.WriteLine("----------------------------------------------------");

            StringBuilder sb = new StringBuilder();
            string line;
            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
            {
                sb.AppendLine(line);
            }

            // Clean up the input into a list of names
            var namesToDelete = sb.ToString()
                .Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(n => n.Trim().Trim('\''))
                .ToList();

            if (namesToDelete.Count == 0) return;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    // --- STEP 1: THE PREVIEW ---
                    Console.WriteLine("\nSearching for matching consoles...");
                    List<string> foundConsoles = new List<string>();

                    foreach (var name in namesToDelete)
                    {
                        // We use RTRIM because of the nchar(100) padding
                        string checkSql = "SELECT ConID, Console FROM Console WHERE RTRIM(Console) = @Name";
                        using (SqlCommand checkCmd = new SqlCommand(checkSql, connection))
                        {
                            checkCmd.Parameters.AddWithValue("@Name", name);
                            using (SqlDataReader reader = checkCmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    foundConsoles.Add($"ID: {reader["ConID"]} - {reader["Console"].ToString().Trim()}");
                                }
                            }
                        }
                    }

                    if (foundConsoles.Count == 0)
                    {
                        Console.WriteLine("No matching consoles found.");
                        return;
                    }

                    Console.WriteLine("\nTHE FOLLOWING CONSOLES WILL BE DELETED:");
                    foreach (var c in foundConsoles) Console.WriteLine($"- {c}");

                    // --- STEP 2: THE CONFIRMATION ---
                    Console.WriteLine("\nWARNING: Deleting a console will fail if games are still linked to it.");
                    Console.Write("Type 'YES' to confirm deletion: ");
                    string confirm = Console.ReadLine().ToUpper();

                    if (confirm == "YES")
                    {
                        int totalDeleted = 0;
                        int failedCount = 0;

                        foreach (var name in namesToDelete)
                        {
                            try
                            {
                                string deleteSql = "DELETE FROM Console WHERE RTRIM(Console) = @Name";
                                using (SqlCommand delCmd = new SqlCommand(deleteSql, connection))
                                {
                                    delCmd.Parameters.AddWithValue("@Name", name);
                                    int rows = delCmd.ExecuteNonQuery();
                                    totalDeleted += rows;
                                }
                            }
                            catch (SqlException ex) when (ex.Number == 547)
                            {
                                Console.WriteLine($"[Error] Could not delete '{name}': Games are still linked to it.");
                                failedCount++;
                            }
                        }
                        Console.WriteLine($"\nProcess finished. Deleted: {totalDeleted} | Blocked: {failedCount}");
                    }
                    else
                    {
                        Console.WriteLine("\nOperation cancelled.");
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            }
            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }
        static void ConsoleIDs()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("               --- Console IDs ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" ");
            Console.WriteLine("------------------------ ------------------------");
            Console.WriteLine("    -- Nintendo 10x --        -- Atari 20x --");
            Console.WriteLine("------------------------ ------------------------");
            Console.WriteLine("--NES                101  --Atari 2600        201");
            Console.WriteLine("--SNES               102  ------------------------");
            Console.WriteLine("--N64                103     -- Commodore 30x --");
            Console.WriteLine("--GameCube           104  ------------------------");
            Console.WriteLine("--Gameboy            105  --Commodore 64      301");
            Console.WriteLine("--Gameboy Colour     106  --Amiga 500         302");
            Console.WriteLine("--Gameboy Advance    107  ------------------------");
            Console.WriteLine("--Gameboy Advance SP 108       -- Sega 60x --");
            Console.WriteLine("--Nintendo DS        109  ------------------------");
            Console.WriteLine("--Nintendo DS Lite   110  --Master System II  606");
            Console.WriteLine("--Nintendo DS XL     111");
            Console.WriteLine("--Nintendo Wii       112");
            Console.WriteLine("--Nintendo Switch    113");
            Console.WriteLine(" ");
            Console.WriteLine("------------------------ ------------------------");
            Console.WriteLine("     -- Sony 40x --         -- Microsoft 50x --");
            Console.WriteLine("------------------------ ------------------------");
            Console.WriteLine("--Playstation 1      401  --Xbox              501");
            Console.WriteLine("--Playstation 2      402  --Xbox 360          502");
            Console.WriteLine("--Playstation 3      403  --Xbox One          503");
            Console.WriteLine(" ");
            Console.WriteLine("------------------------");
            Console.WriteLine("     -- PC 90x --");
            Console.WriteLine("------------------------");
            Console.WriteLine("--PC                 901 ");
            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }
    }
}
