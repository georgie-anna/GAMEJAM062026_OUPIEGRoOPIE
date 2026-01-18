using UnityEngine;
using System.Collections.Generic;

public class GiftManager : MonoBehaviour
{
    private static GiftManager _instance;

    public static GiftManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GiftManager>();
            }
            return _instance;
        }
    }

    private List<GameObject> _collectedGifts = new List<GameObject>();

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

    public void RegisterCollectedGift(GameObject gift)
    {
        if (!_collectedGifts.Contains(gift))
        {
            _collectedGifts.Add(gift);
            Debug.Log($"Geschenk registriert: {gift.name}");
        }
    }

    public void ResetAllGifts()
    {
        Debug.Log($"Setze {_collectedGifts.Count} Geschenke zurück");

        foreach (GameObject gift in _collectedGifts)
        {
            if (gift != null)
            {
                gift.SetActive(true);
            }
        }

        _collectedGifts.Clear();
    }
}