using UnityEngine;

/// <summary>
/// Geschenk das den Spieler beim Einsammeln teleportiert
/// </summary>
public class TeleportGift : MonoBehaviour, ICollectible
{
    [Header("Teleport Settings")]
    [Tooltip("Wohin soll der Spieler teleportiert werden?")]
    [SerializeField] private Transform _targetSpawnPoint;
    [Tooltip("Oder stelle die Position manuell ein (wenn kein Transform zugewiesen)")]
    [SerializeField] private Vector3 _targetPosition = new Vector3(0, 1, 0);

    [Header("Optional Effects")]
    [Tooltip("Sound beim Einsammeln (optional)")]
    [SerializeField] private AudioClip _collectSound;
    [Tooltip("Particle Effect beim Einsammeln (optional)")]
    [SerializeField] private GameObject _collectEffect;

    public void Collect(GameObject collector)
    {
        // Optional: Sound abspielen
        if (_collectSound != null)
        {
            AudioSource.PlayClipAtPoint(_collectSound, transform.position);
        }

        // Optional: Particle Effect spawnen
        if (_collectEffect != null)
        {
            Instantiate(_collectEffect, transform.position, Quaternion.identity);
        }

        // Spieler teleportieren
        Vector3 targetPos = _targetSpawnPoint != null ? _targetSpawnPoint.position : _targetPosition;
        collector.transform.position = targetPos;

        // Optional: Rotation setzen
        if (_targetSpawnPoint != null)
        {
            collector.transform.rotation = _targetSpawnPoint.rotation;
        }

        Debug.Log($"Spieler teleportiert zu: {targetPos}");

    }

    // Zeigt Teleport-Ziel im Editor an
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 targetPos = _targetSpawnPoint != null ? _targetSpawnPoint.position : _targetPosition;
        Gizmos.DrawWireSphere(targetPos, 0.5f);
        Gizmos.DrawLine(transform.position, targetPos);
    }
}