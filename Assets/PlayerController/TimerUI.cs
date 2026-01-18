using UnityEngine;
using TMPro;

/// <summary>
/// Countdown Timer UI - Zeigt die verbleibende Zeit oben in der Mitte an
/// Startet NICHT automatisch, sondern wird vom StartPresent gestartet
/// </summary>
public class TimerUI : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Startzeit in Sekunden")]
    [SerializeField] private float _startTime = 60f;
    [Tooltip("Was passiert wenn Timer abläuft?")]
    [SerializeField] private bool _pauseGameOnTimeout = false;

    [Header("UI References")]
    [Tooltip("TextMeshPro Text Component für Timer-Anzeige")]
    [SerializeField] private TextMeshProUGUI _timerText;

    [Header("Display Settings")]
    [Tooltip("Textfarbe")]
    [SerializeField] private Color _normalColor = Color.white;
    [Tooltip("Warnfarbe wenn weniger als 10 Sekunden übrig")]
    [SerializeField] private Color _warningColor = Color.red;
    [Tooltip("Textgröße")]
    [SerializeField] private float _fontSize = 48f;

    [Header("Start Behavior")]
    [Tooltip("Timer verstecken bis er startet?")]
    [SerializeField] private bool _hideUntilStart = true;

    private float _currentTime;
    private bool _isRunning = false;

    private void Start()
    {
        Debug.Log($"TimerUI Start: _startTime = {_startTime}, _currentTime wird gesetzt auf {_startTime}");
        _currentTime = _startTime;
        _isRunning = false;

        if (_timerText != null)
        {
            _timerText.fontSize = _fontSize;
            _timerText.color = _normalColor;

            if (_hideUntilStart)
            {
                _timerText.gameObject.SetActive(false);
            }
            else
            {
                UpdateTimerDisplay();
            }
        }
        else
        {
            Debug.LogError("TimerUI: Kein TextMeshProUGUI zugewiesen!");
        }
    }

    private void Update()
    {
        if (!_isRunning || _timerText == null)
            return;

        _currentTime -= Time.deltaTime;

        if (_currentTime <= 0f)
        {
            Debug.Log($"Timer erreicht 0! _currentTime = {_currentTime}, rufe OnTimerEnd auf");
            _currentTime = 0f;
            _isRunning = false;
            OnTimerEnd();
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(_currentTime / 60f);
        int seconds = Mathf.FloorToInt(_currentTime % 60f);
        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (_currentTime <= 10f && _currentTime > 0f)
        {
            _timerText.color = _warningColor;
        }
        else
        {
            _timerText.color = _normalColor;
        }
    }

    private void OnTimerEnd()
    {
        Debug.Log("=== OnTimerEnd aufgerufen! ===");

        if (EndTeleporter.Instance != null)
        {
            Debug.Log("EndTeleporter.Instance gefunden - rufe TeleportPlayerToEnd auf");
            EndTeleporter.Instance.TeleportPlayerToEnd();
        }
        else
        {
            Debug.LogError("TimerUI: EndTeleporter.Instance ist NULL!");
        }

        if (_pauseGameOnTimeout)
        {
            Time.timeScale = 0f;
            Debug.Log("Spiel pausiert");
        }
    }

    public void StartTimer()
    {
        Debug.Log($"StartTimer() aufgerufen! _startTime = {_startTime}");
        _currentTime = _startTime;
        _isRunning = true;

        if (_timerText != null)
        {
            _timerText.gameObject.SetActive(true);
            UpdateTimerDisplay();
        }

        Debug.Log("Timer gestartet!");
    }

    public void AddTime(float seconds)
    {
        _currentTime += seconds;
    }

    public void PauseTimer()
    {
        _isRunning = false;
    }

    public void ResumeTimer()
    {
        _isRunning = true;
    }

    public void ResetTimer()
    {
        _currentTime = _startTime;
        _isRunning = false;
        UpdateTimerDisplay();
    }

    public float GetCurrentTime()
    {
        return _currentTime;
    }

    public bool IsRunning()
    {
        return _isRunning;
    }
}