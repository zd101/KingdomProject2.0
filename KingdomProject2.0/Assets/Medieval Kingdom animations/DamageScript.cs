using System.Collections;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private Color originalColor; // Store the original color of the sprite
    public float flashDuration = 0.5f; // Duration of the flash
    public float redAlpha = 0.8f; // Alpha value for the translucent red color
    public float lightnessFactor = 0.5f; // Factor to lighten the red color

    void Start()
    {
        // Get the SpriteRenderer component from the GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Store the original color of the sprite
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // Press 'F' to flash translucent red
        {
            StartFlashing(); // Call the method in the same script
        }
    }

    public void StartFlashing()
    {
        // Start the Flash coroutine
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        // Define the lighter shade of red
        Color lighterRed = new Color(1f, 1f - lightnessFactor, 1f - lightnessFactor, redAlpha);

        // Change the sprite color to lighter red
        spriteRenderer.color = lighterRed;
        // Wait for the flash duration
        yield return new WaitForSeconds(flashDuration);
        // Change the sprite color back to the original color
        spriteRenderer.color = originalColor;
    }
}