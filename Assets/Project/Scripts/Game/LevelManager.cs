using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{

    private static LevelManager instance;
    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LevelManager();
            }
            return instance;
        }
    }

    private GameController.GameMode gameMode;
    public GameController.GameMode GameMode { get { return gameMode; } set { gameMode = value; } }

    private LevelParameter[] parameters;
    public LevelParameter[] Parameters { get { return parameters; } set { parameters = value; } }

    private bool isEndless;
    public bool IsEndless { get { return isEndless; } set { isEndless = value; } }

    public LevelManager()
    {
        parameters = new LevelParameter[0];
    }
}

