using UnityEngine;

public class EndPresent : MonoBehaviour
{
    [Header("Optional Effects")]
    [Tooltip("Sound beim Klicken (optional)")]
    [SerializeField] private AudioClip _clickSound;
    [Tooltip("Particle Effect beim Klicken (optional)")]
    [SerializeField] private GameObject _clickEffect;

    private void OnMouseDown()
    {
        // Sound abspielen
        if (_clickSound != null)
        {
            AudioSource.PlayClipAtPoint(_clickSound, transform.position);
        }

        // Effekt spawnen
        if (_clickEffect != null)
        {
            Instantiate(_clickEffect, transform.position, Quaternion.identity);
        }

        Debug.Log("End-Geschenk angeklickt! Spiel wird beendet!");

        // Spiel beenden
        QuitGame();
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}