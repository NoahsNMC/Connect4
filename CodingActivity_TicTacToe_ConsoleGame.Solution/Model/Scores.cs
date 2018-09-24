using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingActivity_TicTacToe_ConsoleGame
{
    public class Scores
    {
        public struct Scoreboard
        {
            public DateTime gameTime;
            public string[] playerNames;
            public int[] playerScores;
        }

        public Scoreboard score;
    }
}
