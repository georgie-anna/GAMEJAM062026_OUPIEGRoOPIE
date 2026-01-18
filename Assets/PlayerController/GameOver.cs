using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Game Over Screen mit Statistiken und Buttons
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Das komplette Game Over Panel")]
    [SerializeField] private GameObject _gameOverPanel;

    [Tooltip("Text für die Anzahl gesammelter Geschenke")]
    [SerializeField] private TextMeshProUGUI _presentsCollectedText;

    [Tooltip("Optional: Text für die verstrichene Zeit")]
    [SerializeField] private TextMeshProUGUI _timeElapsedText;

    [Header("Teleport Settings")]
    [Tooltip("Ziel-Position auf der End-Insel")]
    [SerializeField] private Transform _endIslandSpawnPoint;

    [Tooltip("Oder stelle die Position manuell ein")]
    [SerializeField] private Vector3 _endIslandPosition = new Vector3(0, 1, 0);

    private static GameOverUI _instance;

    public static GameOverUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameOverUI>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Panel am Anfang verstecken
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("GameOverUI: Game Over Panel nicht zugewiesen!");
        }
    }

    /// <summary>
    /// Zeigt den Game Over Screen an
    /// </summary>
    public void ShowGameOver(int presentsCollected, float timeElapsed = 0f)
    {
        if (_gameOverPanel == null)
        {
            Debug.LogError("GameOverUI: Game Over Panel nicht zugewiesen!");
            return;
        }

        // Spieler zur End-Insel teleportieren
        TeleportPlayerToEndIsland();

        // Panel anzeigen
        _gameOverPanel.SetActive(true);

        // Statistiken anzeigen
        if (_presentsCollectedText != null)
        {
            _presentsCollectedText.text = $"Geschenke gesammelt: {presentsCollected}";
        }

        if (_timeElapsedText != null && timeElapsed > 0f)
        {
            int minutes = Mathf.FloorToInt(timeElapsed / 60f);
            int seconds = Mathf.FloorToInt(timeElapsed % 60f);
            _timeElapsedText.text = $"Zeit: {minutes:00}:{seconds:00}";
        }

        // Maus sichtbar machen und freigeben
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log($"Game Over! Geschenke: {presentsCollected}");
    }

    private void TeleportPlayerToEndIsland()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("GameOverUI: Spieler nicht gefunden! Stelle sicher dass der Spieler den Tag 'Player' hat.");
            return;
        }

        Vector3 targetPos = _endIslandSpawnPoint != null ? _endIslandSpawnPoint.position : _endIslandPosition;
        player.transform.position = targetPos;

        if (_endIslandSpawnPoint != null)
        {
            player.transform.rotation = _endIslandSpawnPoint.rotation;
        }

        Debug.Log($"Spieler zur End-Insel teleportiert: {targetPos}");
    }

    /// <summary>
    /// Startet das Spiel neu (lädt die aktuelle Szene neu)
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f; // Falls das Spiel pausiert war
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Spiel wird neu gestartet...");
    }

    /// <summary>
    /// Beendet das Spiel
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Spiel wird beendet...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}