using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    public Player player;
    public Button endlessModeButton;

    private bool isEndless;

    // Use this for initialization
    void Awake()
    {
        player.DemonstrationMode = true;
        player.HasJetpack = true;
    }

    public void OnEndlessMode()
    {
        isEndless = !isEndless;
        endlessModeButton.GetComponentInChildren<Text>().text = isEndless ? "X" : " ";
    }

    public void OnEnterLevel(LevelSettings levelSettings)
    {
        LevelManager.Instance.GameMode = levelSettings.gameMode;
        LevelManager.Instance.Parameters = levelSettings.parameters;
        LevelManager.Instance.IsEndless = isEndless;

        SceneManager.LoadScene("Game");
    }
}

