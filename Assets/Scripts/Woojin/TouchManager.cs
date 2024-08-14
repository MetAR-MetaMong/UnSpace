using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
    private Camera mainCamera;
    public InputActionProperty positionProperty, touchProperty;

    void Start()
    {
        // Cache the main camera
        mainCamera = Camera.main;
    }

    void OnEnable() 
    {
        positionProperty.action.Enable();
        touchProperty.action.Enable();

        touchProperty.action.started += OnTouch;
        touchProperty.action.performed += OnTouch;

    }

    void OnDisable() 
    {
        touchProperty.action.started -= OnTouch;
        touchProperty.action.performed -= OnTouch;

        positionProperty.action.Disable();
        touchProperty.action.Disable();
    }

    void OnTouch(InputAction.CallbackContext context) {
        if(context.ReadValue<float>() < 0.5f) return;
        // Get the touch position
        Vector2 touchPos = positionProperty.action.ReadValue<Vector2>();

        // Convert touch position to a ray
        Ray ray = mainCamera.ScreenPointToRay(touchPos);
        Debug.DrawRay(ray.origin, ray.direction, Color.white);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the ray hit a collider
            if (hit.collider.CompareTag("Rentable")) // Optional: Use tags to filter specific colliders
            {
                ReactManager.Set(hit.collider.gameObject);
                hit.collider.GetComponent<CheckBox>().ToggleColor();
            }
        }
    }

}
