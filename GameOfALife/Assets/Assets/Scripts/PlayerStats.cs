using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStats
{
    public static int LIVES = 3;
    private static string GAME_OVER = "GAME_OVER";
    private static string WIN = "WIN";
    private static string state = "WIN";
    private static int life = LIVES;
    private static string currentCharacter = "Anais";
    private static int collectedCoins = 0;

    public static int CollectedCoins
    {
        get
        {
            return collectedCoins;
        }

        set
        {
            collectedCoins = value;
            //PlayerPrefs.SetInt(SCORE_KEY, value);
        }
    }

    public static int Life
    {
        get
        {
            return life;
        }

        set
        {
            life = value;
            if (life == 0)
            {
                GameOver();
            }
            //PlayerPrefs.SetInt(SCORE_KEY, value);
        }
    }

    public static void GameOver()
    {
        state = GAME_OVER;
    }

    public static void Win()
    {
        state = WIN;
    }

    public static bool isGameOver
    {
        get
        {
            return state == GAME_OVER;
        }
    }

    public static string CurrentCharacter
    {
        get
        {
            return currentCharacter;
        }
        set
        {
            currentCharacter = value;
        }
    }
}
