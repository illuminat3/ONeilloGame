using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONeilloApp
{
    internal class GameState
    {
        public string gameName { get; set; }
        public PossibleValues[,] Board { get; set; }
        public PossibleValues CurrentPlayer { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
    }
}
