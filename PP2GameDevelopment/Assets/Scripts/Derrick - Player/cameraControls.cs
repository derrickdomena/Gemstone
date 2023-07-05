using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControls : MonoBehaviour
{
    [Header("----- Component -----")]
    // Mouse Sensitivity
    [SerializeField] int sensitivity;

    // Lock Vertical Mouse Movement
    // Purpose: Avoid tilting camera to far forward or behind the Player.
    [SerializeField] int lockVerMin;
    [SerializeField] int lockVerMax;

    // Invert Mouse Y
    [SerializeField] bool invertY;

    float xRotation;

    // Start is called before the first frame update
    void Start()
    {
        // No need for cursor, using UI reticle.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Get Input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

        // Define xRotation
        if(invertY)
            xRotation += mouseY;
        else 
            xRotation -= mouseY;

        // Clamp camera rotation on the X-axis
        xRotation = Mathf.Clamp(xRotation, lockVerMin, lockVerMax);

        // Rotate the camera on the X-axis
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // Rotate the player on the Y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
