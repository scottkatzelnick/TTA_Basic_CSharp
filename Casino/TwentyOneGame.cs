using Casino.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Casino.BlackJack
{
    public class TwentyOneGame : Game, IWalkAway
    {
        public TwentyOneDealer Dealer { get; set; }

        public override void Play()
        {
            Dealer = new TwentyOneDealer();
            foreach (Player player in Players)
            {
                player.Hand = new List<Card>();
                player.Stay = false;
            }
            Dealer.Hand = new List<Card>();
            Dealer.Stay = false;
            Dealer.Deck = new Deck();
            Dealer.Deck.Shuffle();

            foreach (Player player in Players)
            {
                bool validAnswer = false;
                int bet = 0;
                while (!validAnswer)
                {
                    Console.Write("\nPlace your bet!\n>>>: ");
                    validAnswer = int.TryParse(Console.ReadLine(), out bet);
                    if (!validAnswer) Console.WriteLine("Please enter digits only, no decimals.");
                }

                if (bet < 0)
                {
                    throw new FraudException("Kicked out for fraudulent betting!");
                }

                bool successfullyBet = player.Bet(bet);
                if (!successfullyBet)
                {
                    return;
                }
                Bets[player] = bet;
            }
            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine("\nDealing. . .");
                foreach (Player player in Players)
                {
                    Console.Write($"{player.Name}: ");
                    Dealer.Deal(player.Hand);
                    if (i == 1)
                    {
                        bool blackJack = TwentyOneRules.CheckForBlackJack(player.Hand);
                        if (blackJack)
                        {
                            player.Balance += Convert.ToInt32((Bets[player] * 1.5) + Bets[player]);
                            Console.WriteLine($"BlackJack! {player.Name} wins {Bets[player]}!\nYour new balance is now {player.Balance}!");
                            return;
                        }
                    }
                }
                Console.Write("Dealer: ");
                Dealer.Deal(Dealer.Hand);
                if (i == 1)
                {
                    bool blackJack = TwentyOneRules.CheckForBlackJack(Dealer.Hand);
                    if (blackJack)
                    {
                        Console.WriteLine("\nDealer has BlackJack! Everyone loses!");
                        foreach (KeyValuePair<Player, int> entry in Bets)
                        {
                            Dealer.Balance += entry.Value;
                        }
                        return;
                    }
                }
            }
            foreach (Player player in Players)
            {
                while (!player.Stay)
                {
                    Console.WriteLine("Your cards are:  ");
                    foreach (Card card in player.Hand)
                    {
                        Console.WriteLine($"{card.ToString()}");
                    }
                    Console.Write("\n\nHit or Stay?\n>>>: ");
                    string answer = Console.ReadLine().ToLower();
                    if (answer == "stay")
                    {
                        player.Stay = true;
                        break;
                    }
                    else if (answer == "hit")
                    {
                        Dealer.Deal(player.Hand);
                    }
                    bool busted = TwentyOneRules.isBusted(player.Hand);
                    if (busted)
                    {
                        Dealer.Balance += Bets[player];
                        Console.WriteLine($"{player.Name} Busted! You lost your bet, {Bets[player]}! Your new balance is {player.Balance}.");
                        Console.Write("\n\nDo you want to play again?\n>>>: ");
                        answer = Console.ReadLine().ToLower();
                        if (answer == "yes" || answer == "yeah" || answer == "yea" || answer == "yup" || answer == "ya" || answer == "y")
                        {
                            player.isActivelyPlaying = true;
                            return;
                        }
                        else
                        {
                            player.isActivelyPlaying = false;
                            return;
                        }
                    }
                }
            }
            Dealer.isBusted = TwentyOneRules.isBusted(Dealer.Hand);
            Dealer.Stay = TwentyOneRules.ShouldDealerStay(Dealer.Hand);
            while (!Dealer.Stay && !Dealer.isBusted)
            {
                Console.WriteLine("\nDealer is hitting. . .");
                Dealer.Deal(Dealer.Hand);
                Dealer.isBusted = TwentyOneRules.isBusted(Dealer.Hand);
                Dealer.Stay = TwentyOneRules.ShouldDealerStay(Dealer.Hand);
            }
            if (Dealer.Stay)
            {
                Console.WriteLine("\nDealer is staying!");
            }
            if (Dealer.isBusted)
            {
                Console.WriteLine("\nDealer Busted!");
                foreach (KeyValuePair<Player, int> entry in Bets)
                {
                    Players.Where(x => x.Name == entry.Key.Name).First().Balance += (entry.Value * 2);
                    Dealer.Balance -= entry.Value;
                    Console.WriteLine($"{entry.Key.Name} won {entry.Value}! Your new balance is {entry.Key.Balance}!");
                }
                return;
            }
            foreach (Player player in Players)
            {
                bool? playerWon = TwentyOneRules.CompareHands(player.Hand, Dealer.Hand);
                if (playerWon == null)
                {
                    Console.WriteLine("Push! No one wins.");
                    player.Balance += Bets[player];
                }
                else if (playerWon == true)
                {
                    player.Balance += (Bets[player] * 2);
                    Dealer.Balance -= Bets[player];
                    Console.WriteLine($"\n{player.Name} won {Bets[player]}! Your new balance is {player.Balance}!");
                }
                else
                {
                    Console.WriteLine($"\nDealer Wins {Bets[player]}!");
                    Dealer.Balance += Bets[player];
                    Console.WriteLine($"Your new balance is now {player.Balance}.");
                }
                Console.Write("\n\nPlay again?\n>>>: ");
                string answer = Console.ReadLine().ToLower();
                if (answer == "yes" || answer == "yeah" || answer == "yea" || answer == "yup" || answer == "ya" || answer == "y")
                {
                    player.isActivelyPlaying = true;
                    return;
                }
                else
                {
                    player.isActivelyPlaying = false;
                    return;
                }
            }
        }

        public override void ListPlayers()
        {
            Console.WriteLine("21 Players:");
            base.ListPlayers();
        }

        public void WalkAway(Player player)
        {
            throw new NotImplementedException();
        }
    }
}