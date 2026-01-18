using UnityEngine;
using TMPro;

public class HighscoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _highscoreText;

    private void Start()
    {
        if (_highscoreText == null)
        {
            _highscoreText = GetComponent<TextMeshProUGUI>();
        }
        UpdateHighscoreDisplay();
    }

    private void Update()
    {
        UpdateHighscoreDisplay();
    }

    private void UpdateHighscoreDisplay()
    {
        if (PresentCounter.Instance != null)
        {
            int highscore = PresentCounter.Instance.GetHighscore();
            _highscoreText.text = $"Highscore: {highscore}";
        }
        else
        {
            _highscoreText.text = "Highscore: 0";
        }
    }
}