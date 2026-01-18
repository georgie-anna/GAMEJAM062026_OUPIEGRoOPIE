using UnityEngine;

/// <summary>
/// Interface für alle sammelbaren Objekte
/// </summary>
public interface ICollectible
{
    // Großgeschrieben: Collect statt collect
    void Collect(GameObject collector);
}