using Casino;
using Casino.BlackJack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace BlackJack
{
    class Program
    {
        static void Main(string[] args)
        {
            const string casinoName = "Grand Hotel & Casino"; // Great if used several times or more and you want it to stay consistent

            Console.Write($"Welcome to {casinoName}. Let's start by telling me your name.\n>>>: ");
            string inputName = Console.ReadLine();

            if (inputName.ToLower() == "admin")
            {
                List<ExceptionEntity> Exceptions = ReadExceptions();
                foreach (var exception in Exceptions)
                {
                    Console.Write(exception.Id + " | ");
                    Console.Write(exception.ExceptionType + " | ");
                    Console.Write(exception.ExceptionMessage + " | ");
                    Console.Write(exception.TimeStamp + " | ");
                    Console.WriteLine();
                }
                Console.ReadLine();
                return;
            }

            bool validAnswer = false;
            int bank = 0;
            while (!validAnswer)
            {
                Console.Write("And how much money did you bring today?\n>>>: ");
                validAnswer = int.TryParse(Console.ReadLine(), out bank);
                if (!validAnswer) Console.WriteLine("Please enter digits only, no decimals!");
            }

            TextInfo playerNameTI = new CultureInfo("en-US", false).TextInfo; // Creates a TextInfo based on the "en-US" culture
            string lowerPlayerName = playerNameTI.ToLower(inputName); // First pass normalization to lower-case all letters
            string playerName = playerNameTI.ToTitleCase(lowerPlayerName); // Last pass normalization to capitalize player's name

            Console.Write($"\nHello {playerName}. Would you like to join a game of 21, right now?\n>>>: ");
            string answer = Console.ReadLine().ToLower();
            if (answer == "yes" || answer == "yeah" || answer == "yea" || answer == "yup" || answer == "ya" || answer == "y")
            {
                Player player = new Player(playerName, bank);
                player.Id = Guid.NewGuid();
                using (StreamWriter file = new StreamWriter(@"C:\Users\Scott\Projects\repos\TTA_Basic_CSharp\logs\blackjack_log.txt", true))
                {
                    file.WriteLine(player.Id);
                }
                Game game = new TwentyOneGame();
                game += player;
                player.isActivelyPlaying = true;
                while (player.isActivelyPlaying && player.Balance > 0)
                {
                    try
                    {
                        game.Play(); // Abstract method, contains logic for game itself
                    }
                    catch (FraudException ex)
                    {
                        Console.WriteLine(ex.Message);
                        UpdateDbWithException(ex);
                        Console.ReadLine();
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred- {ex.Message}! Please see your system administrator.");
                        UpdateDbWithException(ex);
                        Console.ReadLine();
                        return;
                    }
                }
                game -= player;
                Console.WriteLine("\nThank you for playing!\n");
            }
            Console.WriteLine("Feel free to look around the casino. Bye for now!");
            Console.Read();
        }

        private static void UpdateDbWithException(Exception ex)
        {
            string connectionString = @"Data Source = (localdb)\ProjectsV13; Initial Catalog = BlackJackGame; Integrated Security = True;
                                        Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite;
                                        MultiSubnetFailover = False";

            string queryString = @"INSERT INTO Exceptions (ExceptionType, ExceptionMessage, TimeStamp)
                                    VALUES (@ExceptionType, @ExceptionMessage, @TimeStamp)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@ExceptionType", SqlDbType.VarChar);
                command.Parameters.Add("@ExceptionMessage", SqlDbType.VarChar);
                command.Parameters.Add("@TimeStamp", SqlDbType.DateTime);

                command.Parameters["@ExceptionType"].Value = ex.GetType().ToString();
                command.Parameters["@ExceptionMessage"].Value = ex.Message;
                command.Parameters["@TimeStamp"].Value = DateTime.Now;

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private static List<ExceptionEntity> ReadExceptions()
        {
            string connectionString = @"Data Source = (localdb)\ProjectsV13; Initial Catalog = BlackJackGame; Integrated Security = True;
                                        Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite;
                                        MultiSubnetFailover = False";

            string quearyString = @"SELECT Id, ExceptionType, ExceptionMessage, TimeStamp FROM Exceptions";

            List<ExceptionEntity> Exceptions = new List<ExceptionEntity>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(quearyString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ExceptionEntity exception = new ExceptionEntity();
                    exception.Id = Convert.ToInt32(reader["Id"]);
                    exception.ExceptionType = reader["ExceptionType"].ToString();
                    exception.ExceptionMessage = reader["ExceptionMessage"].ToString();
                    exception.TimeStamp = Convert.ToDateTime(reader["TimeStamp"]);

                    Exceptions.Add(exception);
                }
                connection.Close();
            }

            return Exceptions;
        }
    }
}