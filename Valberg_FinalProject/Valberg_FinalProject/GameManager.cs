using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valberg_FinalProject
{
    class GameManager
    {
        public static int score = 0;
        public static double timer = 30;
        public static int currenthighscore = 0;
        public static List<int> highscores = new List<int>();

        public static void GameOver()
        {
            if (score != 0)
            {
                highscores.Add(score);
                currenthighscore = score;
                score = 0;
            }

            highscores.Sort();
            highscores.Reverse();
            if (highscores.Count > 10)
                highscores.RemoveRange(10, (highscores.Count - 10));
        }

        public static string ListToText(List<int> list)
        {
            string result = "High Scores: \n";
            foreach (var listMember in list)
                result += "        " + listMember.ToString() + "\n";
            return result;
        }
    }
}
