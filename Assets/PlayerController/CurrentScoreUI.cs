using UnityEngine;
using TMPro;

public class CurrentScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentScoreText;

    private void Start()
    {
        if (_currentScoreText == null)
        {
            _currentScoreText = GetComponent<TextMeshProUGUI>();
        }
        UpdateCurrentScoreDisplay();
    }

    private void Update()
    {
        UpdateCurrentScoreDisplay();
    }

    private void UpdateCurrentScoreDisplay()
    {
        if (PresentCounter.Instance != null)
        {
            int currentScore = PresentCounter.Instance.GetCurrentCount();
            _currentScoreText.text = $"Geschenke: {currentScore}";
        }
        else
        {
            _currentScoreText.text = "Geschenke: 0";
        }
    }
}