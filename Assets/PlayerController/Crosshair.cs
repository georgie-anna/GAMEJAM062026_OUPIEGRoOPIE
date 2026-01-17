/*
 * crosshair
 * modifiable: color / lenght / thickness / gap size
 */



using UnityEngine;


/// <summary>
/// crosshair & modifiable
/// </summary>
public class Crosshair : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [SerializeField] private Color crosshairColor = Color.white;
    [SerializeField] private float lineLength = 10f;
    [SerializeField] private float lineThickness = 2f;
    [SerializeField] private float gapSize = 5f;

    private void OnGUI()
    {
        float centerX = Screen.width / 2f;
        float centerY = Screen.height / 2f;

        // texture for crosshair lines
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, crosshairColor);
        texture.Apply();

        // size / thikness
        GUI.DrawTexture(new Rect(centerX - lineThickness / 2f, centerY - gapSize - lineLength, lineThickness, lineLength), texture);


        GUI.DrawTexture(new Rect(centerX - lineThickness / 2f, centerY + gapSize, lineThickness, lineLength), texture);


        GUI.DrawTexture(new Rect(centerX - gapSize - lineLength, centerY - lineThickness / 2f, lineLength, lineThickness), texture);

        
        GUI.DrawTexture(new Rect(centerX + gapSize, centerY - lineThickness / 2f, lineLength, lineThickness), texture);
    }
}