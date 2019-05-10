using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStats
{
    public static int LIVES = 3;
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
        }
    }

    public static bool isGameOver
    {
        get
        {
            return life == 0;
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
