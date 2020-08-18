using System;
using System.Collections.Generic;

namespace Casino
{
    public class Deck
    {
        public Deck() // Deck constructor for creating a deck object(List) with 52 uniques card objects
        {
            Cards = new List<Card>(); // Initialize an empty list of cards

            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    /* Calls on the Card class to create a new card object, then initializing the
                    new card object the properties suit and face. Next, it adds the card to the
                    aforementioned empty Card List. */
                    Card card = new Card();
                    card.Face = (Face)i;
                    card.Suit = (Suit)j;
                    Cards.Add(card);
                    // **Total Card objects created equals 52 (4 suits x 13 faces)**
                }
            }
        }

        // This is the Cards "property" of the Deck class.
        public List<Card> Cards { get; set; }

        // Shuffle() method that randomizes the cards order in the deck.Cards List. Optional
        // parameters can be set the parameter equal to an "initial value"
        public void Shuffle(int times = 1)
        {
            for (int i = 0; i < times; i++)
            {
                List<Card> TempList = new List<Card>();
                Random random = new Random();

                while (Cards.Count > 0)
                {
                    int randomIndex = random.Next(0, Cards.Count);
                    TempList.Add(Cards[randomIndex]);
                    Cards.RemoveAt(randomIndex);
                }
                Cards = TempList; // Returns shuffled cards back to the Cards List
            }
        }
    }
}