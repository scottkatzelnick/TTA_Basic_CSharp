using System;
using System.Collections.Generic;

namespace Casino
{
    public abstract class Game // Abstract Class - Never instantiated, just inherited
    {
        // Props
        private List<Player> _players = new List<Player>();

        private Dictionary<Player, int> _bets = new Dictionary<Player, int>();

        public List<Player> Players { get { return _players; } set { _players = value; } }

        public Dictionary<Player, int> Bets { get { return _bets; } set { _bets = value; } }

        public string Name { get; set; }

        public abstract void Play(); // Abstract Method - Needs to be implemented in inheriting class, never here

        public virtual void ListPlayers() // Virtual Method - Can have implementation here, but can be overriden
        {
            foreach (Player player in Players)
            {
                Console.WriteLine(player.Name);
            }
        }
    }
}