using UnityEngine;
using TMPro;

/// <summary>
/// Zählt eingesammelte Geschenke und zeigt sie oben links an
/// </summary>
public class PresentCounter : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("TextMeshPro Text Component für Counter-Anzeige")]
    [SerializeField] private TextMeshProUGUI _counterText;

    [Header("Display Settings")]
    [Tooltip("Textfarbe")]
    [SerializeField] private Color _textColor = Color.white;
    [Tooltip("Textgröße")]
    [SerializeField] private float _fontSize = 36f;
    [Tooltip("Zeige Icon vor der Zahl? (z.B. ' 5')")]
    [SerializeField] private bool _showIcon = true;
    [Tooltip("Icon/Prefix vor der Zahl")]
    [SerializeField] private string _prefix = " ";

    [Header("Optional")]
    [Tooltip("Ziel-Anzahl an Geschenken (0 = unbegrenzt)")]
    [SerializeField] private int _targetAmount = 0;
    [Tooltip("Zeige Ziel-Anzahl? (z.B. '5/10')")]
    [SerializeField] private bool _showTarget = false;

    private int _currentCount = 0;
    private int _highscore = 0;

    private static PresentCounter _instance;

    public static PresentCounter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PresentCounter>();
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

        // Highscore laden
        _highscore = PlayerPrefs.GetInt("PresentHighscore", 0);
        Debug.Log($"Highscore geladen: {_highscore}");
    }

    private void Start()
    {
        if (_counterText != null)
        {
            _counterText.fontSize = _fontSize;
            _counterText.color = _textColor;
            UpdateDisplay();
        }
        else
        {
            Debug.LogError("PresentCounter: Kein TextMeshProUGUI zugewiesen!");
        }
    }

    /// <summary>
    /// Fügt ein eingesammeltes Geschenk hinzu
    /// </summary>
    public void AddPresent()
    {
        _currentCount++;
        UpdateDisplay();
        Debug.Log($"Geschenk eingesammelt! Total: {_currentCount}");

        // Highscore updaten wenn neuer Rekord
        if (_currentCount > _highscore)
        {
            _highscore = _currentCount;
            PlayerPrefs.SetInt("PresentHighscore", _highscore);
            PlayerPrefs.Save();
            Debug.Log($" NEUER HIGHSCORE: {_highscore}!");
        }

        // Prüfe ob Ziel erreicht
        if (_targetAmount > 0 && _currentCount >= _targetAmount)
        {
            OnTargetReached();
        }
    }

    /// <summary>
    /// Entfernt ein Geschenk vom Counter (falls nötig)
    /// </summary>
    public void RemovePresent()
    {
        if (_currentCount > 0)
        {
            _currentCount--;
            UpdateDisplay();
        }
    }

    /// <summary>
    /// Setzt den Counter zurück
    /// </summary>
    public void ResetCounter()
    {
        _currentCount = 0;
        UpdateDisplay();
        Debug.Log("Present Counter zurückgesetzt!");
    }

    /// <summary>
    /// Aktualisiert die Anzeige
    /// </summary>
    private void UpdateDisplay()
    {
        if (_counterText == null)
            return;

        string displayText = "";

        if (_showIcon)
        {
            displayText += _prefix;
        }

        if (_showTarget && _targetAmount > 0)
        {
            displayText += $"{_currentCount}/{_targetAmount}";
        }
        else
        {
            displayText += _currentCount.ToString();
        }

        _counterText.text = displayText;
    }

    /// <summary>
    /// Wird aufgerufen wenn das Ziel erreicht wurde
    /// </summary>
    private void OnTargetReached()
    {
        Debug.Log("Alle Geschenke eingesammelt! ");
    }

    // Public Getter
    public int GetCurrentCount()
    {
        return _currentCount;
    }

    public int GetTargetAmount()
    {
        return _targetAmount;
    }

    public bool IsTargetReached()
    {
        return _targetAmount > 0 && _currentCount >= _targetAmount;
    }

    /// <summary>
    /// Gibt den Highscore zurück
    /// </summary>
    public int GetHighscore()
    {
        return _highscore;
    }
}