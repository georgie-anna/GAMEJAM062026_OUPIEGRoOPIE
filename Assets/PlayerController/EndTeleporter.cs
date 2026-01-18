using UnityEngine;

/// <summary>
/// Teleportiert den Spieler zur End-Insel wenn der Timer abläuft
/// </summary>
public class EndTeleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    [Tooltip("Position auf der End-Insel wo der Spieler spawnen soll")]
    [SerializeField] private Transform _targetSpawnPoint;
    [Tooltip("Oder stelle die Position manuell ein (wenn kein Transform zugewiesen)")]
    [SerializeField] private Vector3 _targetPosition = new Vector3(0, 1, 0);

    [Header("Optional Effects")]
    [Tooltip("Sound beim Teleportieren (optional)")]
    [SerializeField] private AudioClip _teleportSound;
    [Tooltip("Particle Effect beim Teleportieren (optional)")]
    [SerializeField] private GameObject _teleportEffect;

    private static EndTeleporter _instance;
    public static EndTeleporter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<EndTeleporter>();
                Debug.Log("EndTeleporter Instance gesucht: " + (_instance != null ? "Gefunden!" : "NICHT GEFUNDEN!"));
            }
            return _instance;
        }
    }

    private void Awake()
    {
        Debug.Log("EndTeleporter Awake aufgerufen");
        if (_instance == null)
        {
            _instance = this;
            Debug.Log("EndTeleporter Instance gesetzt");
        }
        else if (_instance != this)
        {
            Debug.Log("EndTeleporter Duplikat gefunden - wird zerstört");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Teleportiert den Spieler zur End-Insel
    /// </summary>
    public void TeleportPlayerToEnd()
    {
        Debug.Log("=== TeleportPlayerToEnd aufgerufen! ===");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("EndTeleporter: Spieler nicht gefunden! Stelle sicher dass der Spieler den Tag 'Player' hat.");
            return;
        }

        Debug.Log($"Spieler gefunden: {player.name} an Position {player.transform.position}");

        // Optional: Particle Effect an aktueller Position spawnen
        if (_teleportEffect != null)
        {
            Instantiate(_teleportEffect, player.transform.position, Quaternion.identity);
            Debug.Log("Teleport Effect gespawnt");
        }

        // Spieler teleportieren
        TeleportPlayer(player);

        // Optional: Sound abspielen
        if (_teleportSound != null)
        {
            AudioSource.PlayClipAtPoint(_teleportSound, player.transform.position);
            Debug.Log("Teleport Sound abgespielt");
        }

        Debug.Log("=== Spieler zur End-Insel teleportiert! ===");
    }

    private void TeleportPlayer(GameObject player)
    {
        // Teleportiere zur Transform-Position wenn zugewiesen, sonst zur manuellen Position
        Vector3 targetPos = _targetSpawnPoint != null ? _targetSpawnPoint.position : _targetPosition;

        Debug.Log($"Teleportiere von {player.transform.position} nach {targetPos}");
        Debug.Log($"Target Spawn Point: {(_targetSpawnPoint != null ? _targetSpawnPoint.name : "NICHT ZUGEWIESEN - nutze manuelle Position")}");

        player.transform.position = targetPos;

        Debug.Log($"Neue Spieler Position: {player.transform.position}");

        // Optional: Rotation setzen wenn Transform zugewiesen ist
        if (_targetSpawnPoint != null)
        {
            player.transform.rotation = _targetSpawnPoint.rotation;
            Debug.Log($"Rotation gesetzt auf: {_targetSpawnPoint.rotation.eulerAngles}");
        }

        // Optional: Particle Effect an Ziel-Position spawnen
        if (_teleportEffect != null)
        {
            Instantiate(_teleportEffect, targetPos, Quaternion.identity);
        }
    }

    // Gizmo zum Visualisieren der Zielposition im Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 targetPos = _targetSpawnPoint != null ? _targetSpawnPoint.position : _targetPosition;
        Gizmos.DrawWireSphere(targetPos, 0.5f);
        Gizmos.DrawLine(transform.position, targetPos);
    }
}