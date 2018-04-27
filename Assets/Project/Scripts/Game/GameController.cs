using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    [Header("Gameplay")]
    public Player player;
    public GameMode gameMode;
    public float duration = 15f;
    public float depthRange = 4f;
    public float horizontalRange;

    [Header("Endless gameplay")]
    public bool isEndless;
    public float timeToIncreaseDifficulty = 10f;

    [Header("Visuals")]
    public Camera gameCamera;
    public Text infoText;
    public GameObject upperWall;
    public GameObject lowerWall;

    [Header("Game: jumpers")]
    public GameObject jumpingEnemyPrefab;

    [Header("Game: rollers")]
    public GameObject rollingEnemyPrefab;

    [Header("Game: bouncers")]
    public GameObject bouncingEnemyPrefab;
    public int bouncingEnemiesAmount = 1;

    [Header("Game: crawlers")]
    public GameObject crawlingEnemyPrefab;
    public int crawlerMaximumDifficulty = 5;
    public float crawlerSpawnIntervalMaximum;
    public float crawlerSpawnIntervalMinimum;

    [Header("Game: turrets")]
    public GameObject turretEnemyPrefab;
    public int turretEnemiesAmount = 2;
    public float turretWaitingDuration;
    public float turretMovingDuration;
    public float turretAimingDuration;

    [Header("Game: targets")]
    public GameObject targetEnemyPrefab;
    public int targetMaximumDifficulty = 10;
    public float targetSpawnIntervalMaximum;
    public float targetSpawnIntervalMininum;
    public float targetsPlayerDepth = -5f;
    public float[] targetPositions;
    public float targetDepth = 4f;
    public float targetShootingDuration = 3f;

    [Header("Game: jetpack")]
    public GameObject flyingEnemyPrefab;
    public int jetpackMaximumDifficulty = 5;
    public float flyingEnemySpawnIntervalMaximum;
    public float flyingEnemySpawnIntervalMinimum;
    public float[] flyingEnemyVerticalPositions;

    private float timer;
    private bool gameOver;
    private bool win;
    private int difficulty = 1;
    private float resetTimer = 3f;

    private float crawlerSpawnTimer;
    private float targetSpawnTimer;
    private float flyingEnemySpawnTimer;

    public float Highscore
    {
        get
        {
            string key = "Highscore" + gameMode.ToString();
            return PlayerPrefs.GetFloat(key);
        }
        set
        {
            string key = "Highscore" + gameMode.ToString();
            PlayerPrefs.SetFloat(key, value);
        }
    }

    public enum GameMode
    {
        None,
        Jumpers,
        Rollers,
        Bouncers,
        Crawlers,
        Turrets,
        Targets,
        Jetpack
    }

    // Use this for initialization
    void Awake()
    {
        // Load data.
        if (LevelManager.Instance.GameMode != GameMode.None)
        {
            gameMode = LevelManager.Instance.GameMode;
        }

        isEndless = LevelManager.Instance.IsEndless;

        // Get level parameters from the level manager.
        LevelParameter[] parameters = LevelManager.Instance.Parameters;

        // Change the game duration
        if (GetParameterValue(parameters, "duration") != 0)
        {
            duration = GetParameterValue(parameters, "duration");
        }

        // Set the horizontal range.
        float baseAspect = 9f / 16f;
        float aspectVariation = gameCamera.aspect / baseAspect;
        horizontalRange = (aspectVariation * gameCamera.orthographicSize) / 2f;

        // Set variables.
        player.DepthRange = depthRange;
        player.HorizontalRange = horizontalRange;
        player.onKill = OnPlayerKilled;

        // Set the bounds' positions.
        upperWall.transform.position = new Vector3(upperWall.transform.position.x, upperWall.transform.position.y, depthRange + 1.25f);
        lowerWall.transform.position = new Vector3(lowerWall.transform.position.x, lowerWall.transform.position.y, -depthRange - 1.25f);

        // Set the game timer.
        timer = isEndless ? 0 : duration;

        // Set game mode.
        switch (gameMode)
        {
            case GameMode.Jumpers:
                player.LockZ = true;
                SpawnEnemy();
                break;
            case GameMode.Rollers:
                player.LockZ = false;
                SpawnEnemy();
                break;
            case GameMode.Bouncers:
                if (GetParameterValue(parameters, "enemiesAmount") != 0)
                {
                    bouncingEnemiesAmount = (int)GetParameterValue(parameters, "enemiesAmount");
                }

                player.LockZ = true;
                if (isEndless == false)
                {
                    for (int i = 0; i < bouncingEnemiesAmount; i++)
                    {
                        SpawnEnemy();
                        difficulty++;
                    }
                }
                else
                {
                    SpawnEnemy();
                }
                break;
            case GameMode.Crawlers:
                if (GetParameterValue(parameters, "maximumDifficulty") != 0)
                {
                    crawlerMaximumDifficulty = (int)GetParameterValue(parameters, "maximumDifficulty");
                }
                if (GetParameterValue(parameters, "spawnIntervalMaximum") != 0)
                {
                    crawlerSpawnIntervalMaximum = GetParameterValue(parameters, "spawnIntervalMaximum");
                }
                if (GetParameterValue(parameters, "spawnIntervalMinimum") != 0)
                {
                    crawlerSpawnIntervalMinimum = GetParameterValue(parameters, "spawnIntervalMinimum");
                }

                player.LockZ = true;
                player.CanJump = true;
                crawlerSpawnTimer = crawlerSpawnIntervalMaximum;
                break;
            case GameMode.Turrets:
                if (GetParameterValue(parameters, "enemiesAmount") != 0)
                {
                    turretEnemiesAmount = (int)GetParameterValue(parameters, "enemiesAmount");
                }
                if (GetParameterValue(parameters, "waitingDuration") != 0)
                {
                    turretWaitingDuration = GetParameterValue(parameters, "waitingDuration");
                }
                if (GetParameterValue(parameters, "movingDuration") != 0)
                {
                    turretMovingDuration = GetParameterValue(parameters, "movingDuration");
                }
                if (GetParameterValue(parameters, "aimingDuration") != 0)
                {
                    turretAimingDuration = GetParameterValue(parameters, "aimingDuration");
                }

                player.LockZ = true;
                if (isEndless == false)
                {
                    for (int i = 0; i < turretEnemiesAmount; i++)
                    {
                        SpawnEnemy();
                        difficulty++;
                    }
                }
                else
                {
                    SpawnEnemy();
                }
                break;
            case GameMode.Targets:
                if (GetParameterValue(parameters, "maximumDifficulty") != 0)
                {
                    targetMaximumDifficulty = (int)GetParameterValue(parameters, "maximumDifficulty");
                }
                if (GetParameterValue(parameters, "spawnIntervalMaximum") != 0)
                {
                    targetSpawnIntervalMaximum = GetParameterValue(parameters, "spawnIntervalMaximum");
                }
                if (GetParameterValue(parameters, "spawnIntervalMinimum") != 0)
                {
                    targetSpawnIntervalMininum = GetParameterValue(parameters, "spawnIntervalMinimum");
                }
                if (GetParameterValue(parameters, "shootingDuration") != 0)
                {
                    targetShootingDuration = GetParameterValue(parameters, "shootingDuration");
                }

                player.CanAttack = true;
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, targetsPlayerDepth);
                targetSpawnTimer = targetSpawnIntervalMaximum;
                break;
            case GameMode.Jetpack:
                player.HasJetpack = true;
                flyingEnemySpawnTimer = flyingEnemySpawnIntervalMaximum;
                break;
            default:
                Debug.LogWarning("This game mode wasn't implemented!");
                break;
        }
    }

    void SpawnEnemy()
    {
        switch (gameMode)
        {
            case GameMode.Jumpers:
                GameObject jumpingEnemyObject = Instantiate(jumpingEnemyPrefab);
                jumpingEnemyObject.transform.SetParent(transform);
                jumpingEnemyObject.GetComponent<JumpingEnemy>().HorizontalRange = horizontalRange;
                break;
            case GameMode.Rollers:
                GameObject rollingEnemyObject = Instantiate(rollingEnemyPrefab);
                rollingEnemyObject.transform.SetParent(transform);
                rollingEnemyObject.GetComponent<RollingEnemy>().DepthRange = depthRange;
                rollingEnemyObject.GetComponent<RollingEnemy>().HorizontalRange = horizontalRange;
                break;
            case GameMode.Bouncers:
                GameObject bouncingEnemyObject = Instantiate(bouncingEnemyPrefab);
                bouncingEnemyObject.transform.SetParent(transform);
                bouncingEnemyObject.transform.position = new Vector3(
                    (difficulty % 2 == 0) ? horizontalRange : -horizontalRange,
                    bouncingEnemyObject.transform.position.y,
                    bouncingEnemyObject.transform.position.z
                );
                bouncingEnemyObject.GetComponent<BouncingEnemy>().DepthRange = depthRange;
                bouncingEnemyObject.GetComponent<BouncingEnemy>().HorizontalRange = horizontalRange;
                break;
            case GameMode.Turrets:
                GameObject turretEnemyObject = Instantiate(turretEnemyPrefab);
                turretEnemyObject.transform.SetParent(transform);
                turretEnemyObject.transform.position = new Vector3(
                    (difficulty % 2 == 0) ? horizontalRange * 0.9f : -horizontalRange * 0.9f,
                    turretEnemyObject.transform.position.y,
                    turretEnemyObject.transform.position.z
                );

                TurretEnemy turret = turretEnemyObject.GetComponent<TurretEnemy>();

                turret.HorizontalRange = horizontalRange;

                turret.WaitingDuration = turretWaitingDuration;
                turret.MovingDuration = turretMovingDuration;
                turret.AimingDuration = turretAimingDuration;

                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is dead.
        if (player == null)
        {
            gameOver = true;
            win = false;
        }

        // Check if the game is finished in the regular mode.
        if (isEndless == false)
        {
            timer -= Time.deltaTime;
            if (timer > 0f)
            {
                infoText.text = "Time: " + Mathf.Floor(timer);
            }
            else
            {
                if (player != null)
                {
                    gameOver = true;
                    win = true;
                    player.Invincible = true;
                }
            }

            // Send the game over message.
            if (gameOver == true)
            {
                if (win)
                {
                    infoText.text = "You win!";
                }
                else
                {
                    infoText.text = "You lose!";
                }
            }
        }
        else
        {
            if (player != null)
            {
                // Make the timer count up.
                timer += Time.deltaTime;

                // Update the label.
                infoText.text = "Time: " + Mathf.Floor(timer);

                // Increase difficulty.
                if (timer > timeToIncreaseDifficulty * difficulty)
                {
                    difficulty++;
                    SpawnEnemy();
                }
            }
            else
            {
                // Send game over message.
                infoText.text = "Game over! Your time: " + Mathf.Floor(timer) + "\n";
                infoText.text += "Highscore: " + Mathf.Floor(Highscore);
            }
        }

        if (gameMode == GameMode.Crawlers)
        {
            // Crawler game logic.

            crawlerSpawnTimer -= Time.deltaTime;
            if (crawlerSpawnTimer <= 0f)
            {
                if (isEndless == false)
                {
                    crawlerSpawnTimer = crawlerSpawnIntervalMaximum;
                }
                else
                {
                    float progress = (float)difficulty / crawlerMaximumDifficulty;
                    if (progress > 1)
                    {
                        progress = 1;
                    }

                    crawlerSpawnTimer = crawlerSpawnIntervalMaximum - progress * (crawlerSpawnIntervalMaximum - crawlerSpawnIntervalMinimum);
                }

                GameObject crawlingEnemyObject = Instantiate(crawlingEnemyPrefab);
                crawlingEnemyObject.transform.SetParent(transform);
                crawlingEnemyObject.transform.position = new Vector3(
                    (Random.value > 0.5f) ? (horizontalRange + 0.8f) : (-horizontalRange - 0.8f),
                    crawlingEnemyObject.transform.position.y,
                    crawlingEnemyObject.transform.position.z
                );
            }
        }
        else if (gameMode == GameMode.Targets)
        {
            // Targets game logic.

            // Throw player's weapons.
            if (Input.GetMouseButtonDown(0))
            {
                Ray clickRay = gameCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(clickRay, out hit))
                {
                    if (hit.point.z < player.transform.position.z)
                    {
                        player.ThrowWeapon(hit.point);
                    }
                }
            }

            // Spawn targets.
            targetSpawnTimer -= Time.deltaTime;
            if (targetSpawnTimer <= 0 && player != null && gameOver == false)
            {
                if (isEndless == false)
                {
                    targetSpawnTimer = targetSpawnIntervalMaximum;
                }
                else
                {
                    float progress = (float)difficulty / targetMaximumDifficulty;
                    if (progress > 1)
                    {
                        progress = 1;
                    }

                    targetSpawnTimer = targetSpawnIntervalMaximum - progress * (targetSpawnIntervalMaximum - targetSpawnIntervalMininum);
                }

                GameObject targetObject = Instantiate(targetEnemyPrefab);
                targetObject.transform.position = new Vector3(
                    targetPositions[Random.Range(0, targetPositions.Length)],
                    targetObject.transform.position.y,
                    targetDepth
                );
                targetObject.GetComponent<TargetEnemy>().ShootingDuration = targetShootingDuration;
            }
        }
        else if (gameMode == GameMode.Jetpack)
        {
            // Spawn flying enemies.
            flyingEnemySpawnTimer -= Time.deltaTime;
            if (flyingEnemySpawnTimer <= 0)
            {
                if (isEndless == false)
                {
                    flyingEnemySpawnTimer = flyingEnemySpawnIntervalMaximum;
                }
                else
                {
                    float progress = (float)difficulty / jetpackMaximumDifficulty;
                    if (progress > 1)
                    {
                        progress = 1;
                    }

                    flyingEnemySpawnTimer = flyingEnemySpawnIntervalMaximum - progress * (flyingEnemySpawnIntervalMaximum - flyingEnemySpawnIntervalMinimum);
                }

                GameObject flyingEnemyObject = Instantiate(flyingEnemyPrefab);
                flyingEnemyObject.transform.position = new Vector3(
                    Random.value > 0.5f ? horizontalRange + 1 : -horizontalRange - 1,
                    flyingEnemyVerticalPositions[Random.Range(0, flyingEnemyVerticalPositions.Length)],
                    0
                );
            }
        }

        if (gameOver == true)
        {
            resetTimer -= Time.deltaTime;
            if (resetTimer <= 0)
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }

    void OnPlayerKilled()
    {
        if (timer > Highscore)
        {
            Highscore = timer;
        }
    }


    private float GetParameterValue(LevelParameter[] parameters, string key)
    {
        foreach (LevelParameter parameter in parameters)
        {
            if (parameter.key == key)
            {
                return parameter.value;
            }
        }

        return 0;
    }
}
