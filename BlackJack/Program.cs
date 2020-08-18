using Casino;
using Casino.BlackJack;
using System;
using System.Globalization;
using System.IO;

namespace BlackJack
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string casinoName = "Grand Hotel & Casino"; // Great if used several times or more and you want it to stay consistent

            Console.Write($"Welcome to {casinoName}. Let's start by telling me your name.\n>>>: ");
            string inputName = Console.ReadLine();

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
                    catch (FraudException)
                    {
                        Console.WriteLine("Security, kick this person out!");
                        Console.ReadLine();
                        return;
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine($"An error occurred- {err.Message}! Please see your system administrator.");
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
    }
}