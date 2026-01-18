using UnityEngine;

/// <summary>
/// Geschenk das eingesammelt werden kann
/// </summary>
public class Present : MonoBehaviour, ICollectible
{
    [Header("Optional: Timer Bonus")]
    [Tooltip("Soll beim Einsammeln Zeit zum Timer hinzugefügt werden?")]
    [SerializeField] private bool _addTimeOnCollect = false;
    [Tooltip("Wie viele Sekunden werden hinzugefügt?")]
    [SerializeField] private float _bonusTime = 5f;

    public void Collect(GameObject collector)
    {
        // Optional: Zeit zum Timer hinzufügen
        if (_addTimeOnCollect)
        {
            TimerUI timer = FindFirstObjectByType<TimerUI>();
            if (timer != null)
            {
                timer.AddTime(_bonusTime);
                Debug.Log($"Geschenk eingesammelt! +{_bonusTime} Sekunden");
            }
        }

        // Counter erhöhen
        if (PresentCounter.Instance != null)
        {
            PresentCounter.Instance.AddPresent();
        }

        // Beim GiftManager registrieren
        if (GiftManager.Instance != null)
        {
            GiftManager.Instance.RegisterCollectedGift(gameObject);
        }

        // Geschenk verstecken
        gameObject.SetActive(false);
    }
}