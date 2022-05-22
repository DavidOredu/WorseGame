using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Score system for all game modes found in Lit!
public class ScoreSystem
{
   
    public static class GameScore
    {
        //Scores contained in a domination game
        public static int currentScoreMultiplier = 1;
        public static int maxMultiplier = 2;

        public static int score;
        public static int kills;

        public static void ResetScore()
        {
            score = 0;
            kills = 0;
            currentScoreMultiplier = 1;
        }
    }
}
