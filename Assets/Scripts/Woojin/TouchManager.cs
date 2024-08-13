using UnityEngine;

public class TouchManager : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Cache the main camera
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Check if there is at least one touch
        if (Input.touchCount > 0)
        {
            // Get the first touch
            Touch touch = Input.GetTouch(0);

            // Convert touch position to a ray
            Ray ray = mainCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the ray hit a collider
                if (hit.collider.CompareTag("Rentable")) // Optional: Use tags to filter specific colliders
                {
                    Debug.Log("Touched object with BoxCollider!");
                    ReactManager.Set(hit.collider.gameObject);
                }
            }
        }
    }
}