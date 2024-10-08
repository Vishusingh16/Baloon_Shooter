using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Camera cam; // Assign the main camera in the inspector
    public BalloonSpawner balloonSpawner; // Reference to the BalloonSpawner for releasing balloons
    public AudioClip popSound; // Assign the pop sound effect in the inspector
    private AudioSource audioSource; // Audio source to play sound effects

    private void Start()
    {
        // Set up audio source component
        audioSource = gameObject.AddComponent<AudioSource>();

        if (cam == null)
        {
            Debug.LogError("Camera is not assigned in the inspector!");
        }

        if (balloonSpawner == null)
        {
            Debug.LogError("BalloonSpawner is not assigned in the inspector!");
        }

        if (popSound == null)
        {
            Debug.LogWarning("Pop sound is not assigned in the inspector!");
        }
    }

    void Update()
    {
        // Handle touch for Android and mouse input for desktop
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector3 inputPosition;

            // Use touch input for mobile devices
            if (Input.touchCount > 0)
            {
                inputPosition = Input.GetTouch(0).position;
            }
            else
            {
                inputPosition = Input.mousePosition;
            }

            // Check if inputPosition is within screen bounds
            if (inputPosition.x >= 0 && inputPosition.y >= 0 && inputPosition.x <= Screen.width && inputPosition.y <= Screen.height)
            {
                // Convert the screen position (or touch) into a ray from the camera
                Ray ray = cam.ScreenPointToRay(inputPosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // Check if the raycast hit anything, and if it's tagged as "Balloon"
                if (hit.collider != null && hit.collider.CompareTag("Balloon"))
                {
                    // Call a function to handle popping, play a sound, etc.
                    PopBalloon(hit.collider.gameObject);
                }
            }
        }
    }

    void PopBalloon(GameObject balloon)
    {
        if (balloon == null) return; // Check for null balloon

        // Release the balloon back to the pool instead of destroying it
        balloonSpawner.ReleaseBalloon(balloon);

        // Play balloon pop sound
        if (popSound != null)
        {
            audioSource.PlayOneShot(popSound); // Play the pop sound effect
        }

        // Update score when the balloon is popped
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(10);
        }
        else
        {
            Debug.LogError("ScoreManager is not initialized.");
        }
    }

    // Optional: If you are using OnDrawGizmos, ensure the input is valid here too
    private void OnDrawGizmos()
    {
        if (cam == null)
            return; // Exit if camera reference is missing

        Vector3 inputPosition = Input.mousePosition;

        // Check if the input position is valid before using ScreenToWorldPoint
        if (inputPosition.x >= 0 && inputPosition.y >= 0 && inputPosition.x <= Screen.width && inputPosition.y <= Screen.height)
        {
            Vector3 worldPosition = cam.ScreenToWorldPoint(inputPosition);
            Gizmos.DrawSphere(worldPosition, 0.1f); // Example to show where the touch/click happens
        }
    }
}
