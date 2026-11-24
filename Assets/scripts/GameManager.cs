using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum Objective { CollectCoins, CoinsThenGoal }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Objective")]
    [SerializeField] Objective objective = Objective.CollectCoins;
    [SerializeField] int targetScore = 6;        // never higher than the coins in the level
    [SerializeField] GameObject goal;            // CoinsThenGoal: hidden until targetScore is reached
    [SerializeField] bool finalLevel;

    [Header("Lives")]
    [SerializeField] int lives = 3;
    [SerializeField] float respawnDelay = 0.6f, spawnProtection = 1.5f;
    [SerializeField] Health player;
    [SerializeField] Transform respawnPoint;

    [Header("UI")]
    [SerializeField] TMP_Text scoreText, livesText, bonusText;
    [SerializeField] GameObject winPanel, losePanel, nextButton;

    [Header("Bonus Round (optional, off in the assignment levels)")]
    [SerializeField] bool bonusRound = false;
    [SerializeField] GameObject redCoinPrefab, blueCoinPrefab;
    [SerializeField] int bonusCoinCount = 20;
    [SerializeField] float bonusMinRadius = 1.5f, bonusMaxRadius = 6f, arenaLimit = 9f;

    int score, redTriggerScore, bonusCollected, bonusRemaining, bonusTotal;
    bool redSpawned, redOnField, ended;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        redTriggerScore = Random.Range(1, targetScore + 1);
        if (bonusText != null) bonusText.gameObject.SetActive(false);
        if (objective == Objective.CoinsThenGoal && goal != null) goal.SetActive(false);
        UpdateUI();
    }

    // ---------- score ----------
    public void AddScore(int amount)
    {
        if (ended) return;
        score = Mathf.Min(score + amount, targetScore);
        UpdateUI();
        if (bonusRound && !redSpawned && score >= redTriggerScore) SpawnRedCoin();
        CheckProgress();
    }

    void CheckProgress()
    {
        if (ended || score < targetScore) return;
        if (objective == Objective.CoinsThenGoal)
        {
            if (goal != null) goal.SetActive(true);
        }
        else if (BonusDone) Win();
    }

    public void ReachedGoal()
    {
        if (ended || objective != Objective.CoinsThenGoal || score < targetScore) return;
        Win();
    }

    // ---------- bonus round (Lecture 06 extra) ----------
    bool BonusDone => !bonusRound || (!redOnField && bonusRemaining <= 0);

    bool FreeSpot(Vector3 pos) =>
        !Physics.CheckSphere(pos, 0.3f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

    void SpawnRedCoin()
    {
        redSpawned = true;
        redOnField = true;
        Vector3 pos = Vector3.zero;
        for (int i = 0; i < 40; i++)
        {
            pos = new Vector3(Random.Range(-arenaLimit, arenaLimit), 0.5f,
                              Random.Range(-arenaLimit, arenaLimit));
            if (Vector3.Distance(pos, player.transform.position) > 3f && FreeSpot(pos)) break;
        }
        Instantiate(redCoinPrefab, pos, Quaternion.identity);
    }

    public void CollectRedCoin(Vector3 center)
    {
        redOnField = false;
        bonusCollected = 0;
        bonusRemaining = 0;
        for (int i = 0; i < bonusCoinCount; i++)
        {
            for (int tries = 0; tries < 10; tries++)
            {
                float angle = Random.value * Mathf.PI * 2f;
                float radius = Mathf.Sqrt(Random.Range(
                    bonusMinRadius * bonusMinRadius, bonusMaxRadius * bonusMaxRadius));
                float x = Mathf.Clamp(center.x + Mathf.Cos(angle) * radius, -arenaLimit, arenaLimit);
                float z = Mathf.Clamp(center.z + Mathf.Sin(angle) * radius, -arenaLimit, arenaLimit);
                Vector3 pos = new Vector3(x, 0.5f, z);
                if (!FreeSpot(pos)) continue;
                Instantiate(blueCoinPrefab, pos, Quaternion.identity);
                bonusRemaining++;
                break;
            }
        }
        bonusTotal = bonusRemaining;
        if (bonusText != null) bonusText.gameObject.SetActive(true);
        UpdateBonusUI();
        CheckProgress();
    }

    public void CollectBlueCoin()
    {
        bonusCollected++;
        bonusRemaining--;
        UpdateBonusUI();
        CheckProgress();
    }

    // ---------- UI (a text is only set when its value changes) ----------
    static void Set(TMP_Text t, string s)
    {
        if (t != null && t.text != s) t.text = s;
    }

    public void UpdateUI()
    {
        bool done = objective == Objective.CoinsThenGoal && score >= targetScore;
        Set(scoreText, done ? "Coins done - reach the goal!" : $"Coins: {score} / {targetScore}");
        Set(livesText, $"Lives: {lives}   Health: {Mathf.Max(player.Current, 0)} / {player.Max}");
    }

    void UpdateBonusUI() => Set(bonusText, $"Bonus Coins: {bonusCollected}/{bonusTotal}");

    public void ResetCoins()
    {
        score = 0;
        UpdateUI();
        var coins = FindObjectsByType<Coin>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var c in coins)
        {
            if (c != null) c.ResetCoin();
        }
        if (objective == Objective.CoinsThenGoal && goal != null)
        {
            goal.SetActive(false);
        }
    }

    // ---------- lives, win, lose ----------
    public void PlayerDied()
    {
        if (ended) return;
        lives--;
        ResetCoins();
        UpdateUI();
        if (lives <= 0) Lose(); else StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        if (player != null)
        {
            Rigidbody body = player.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                Vector3 spawnPos = respawnPoint != null ? respawnPoint.position : new Vector3(0f, 0.5f, 0f);
                body.position = spawnPos;
                player.transform.position = spawnPos;
                Physics.SyncTransforms();
            }
            player.Restore(spawnProtection);
            if (player.TryGetComponent(out PlayerController pc))
            {
                pc.OnRespawn();
            }
        }
    }

    public int Lives => lives;

    void Win()
    {
        ended = true;
        var goalComp = FindFirstObjectByType<LevelGoal>();
        if (goalComp != null)
        {
            goalComp.TriggerWin();
        }
        else
        {
            if (winPanel != null) winPanel.SetActive(true);
            if (nextButton != null) nextButton.SetActive(!finalLevel);
            Time.timeScale = 0f;
        }
    }

    void Lose()
    {
        ended = true;
        var goComp = FindFirstObjectByType<GameOver>();
        if (goComp != null)
        {
            goComp.TriggerGameOver();
        }
        else
        {
            if (losePanel != null) losePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;                       // first, or the new level never moves
        var loader = FindFirstObjectByType<LevelLoader>();
        if (loader != null) loader.Reload();
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        var loader = FindFirstObjectByType<LevelLoader>();
        if (loader != null) loader.LoadNext();
    }
}
