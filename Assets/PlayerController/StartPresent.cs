using UnityEngine;
using System.Collections;

public class StartPresent : MonoBehaviour, ICollectible
{
    [Header("Teleport Settings")]
    [Tooltip("Position auf der Hauptinsel wo der Spieler spawnen soll")]
    [SerializeField] private Transform _targetSpawnPoint;
    [Tooltip("Oder stelle die Position manuell ein (wenn kein Transform zugewiesen)")]
    [SerializeField] private Vector3 _targetPosition = new Vector3(0, 1, 0);

    [Header("Timer Settings")]
    [Tooltip("Soll der Timer beim Einsammeln starten?")]
    [SerializeField] private bool _startTimer = true;

    [Header("Respawn Settings")]
    [Tooltip("Wie lange soll das Geschenk unsichtbar sein nach dem Einsammeln?")]
    [SerializeField] private float _respawnDelay = 1f;

    [Header("Optional Effects")]
    [Tooltip("Sound beim Einsammeln (optional)")]
    [SerializeField] private AudioClip _collectSound;
    [Tooltip("Particle Effect beim Einsammeln (optional)")]
    [SerializeField] private GameObject _collectEffect;

    private bool _isCollecting = false;

    public void Collect(GameObject collector)
    {
        if (_isCollecting) return; // Verhindert mehrfaches Einsammeln

        _isCollecting = true;

        if (_collectSound != null)
        {
            AudioSource.PlayClipAtPoint(_collectSound, transform.position);
        }

        if (_collectEffect != null)
        {
            Instantiate(_collectEffect, transform.position, Quaternion.identity);
        }

        // ALLE Geschenke zurücksetzen
        if (GiftManager.Instance != null)
        {
            GiftManager.Instance.ResetAllGifts();
        }
        else
        {
            Debug.LogWarning("StartPresent: Kein GiftManager gefunden!");
        }

        // Present Counter zurücksetzen
        if (PresentCounter.Instance != null)
        {
            PresentCounter.Instance.ResetCounter();
            Debug.Log("Present Counter zurückgesetzt!");
        }

        TeleportPlayer(collector);

        if (_startTimer)
        {
            StartGameTimer();
        }

        Debug.Log("Start-Geschenk eingesammelt! Alle Geschenke zurückgesetzt! Counter zurückgesetzt! Timer gestartet!");

        // Kurz ausblenden und wieder einblenden
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        // Geschenk unsichtbar machen
        Renderer renderer = GetComponent<Renderer>();
        Collider collider = GetComponent<Collider>();

        if (renderer != null) renderer.enabled = false;
        if (collider != null) collider.enabled = false;

        yield return new WaitForSeconds(_respawnDelay);

        // Geschenk wieder sichtbar machen
        if (renderer != null) renderer.enabled = true;
        if (collider != null) collider.enabled = true;

        _isCollecting = false;

        Debug.Log("Start-Geschenk wieder verfügbar!");
    }

    private void TeleportPlayer(GameObject player)
    {
        Vector3 targetPos = _targetSpawnPoint != null ? _targetSpawnPoint.position : _targetPosition;
        player.transform.position = targetPos;

        if (_targetSpawnPoint != null)
        {
            player.transform.rotation = _targetSpawnPoint.rotation;
        }

        Debug.Log($"Spieler teleportiert zu: {targetPos}");
    }

    private void StartGameTimer()
    {
        TimerUI timer = FindFirstObjectByType<TimerUI>();
        if (timer != null)
        {
            timer.ResetTimer();
            timer.ResumeTimer();
            Debug.Log("Timer gestartet!");
        }
        else
        {
            Debug.LogWarning("StartPresent: Kein TimerUI gefunden!");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 targetPos = _targetSpawnPoint != null ? _targetSpawnPoint.position : _targetPosition;
        Gizmos.DrawWireSphere(targetPos, 0.5f);
        Gizmos.DrawLine(transform.position, targetPos);
    }
}