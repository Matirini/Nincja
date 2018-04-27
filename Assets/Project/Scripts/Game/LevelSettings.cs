using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LevelParameter
{
    public string key;
    public float value;
}
public class LevelSettings : MonoBehaviour {

    public int id;
    public GameController.GameMode gameMode;
    public LevelParameter[] parameters;

}
