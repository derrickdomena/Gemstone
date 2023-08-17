using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cameraControls : MonoBehaviour
{
    [Header("----- Component -----")]
    // Mouse Sensitivity
    [SerializeField] public int sensitivity;
    Transform player;

    // Lock Vertical Mouse Movement
    // Purpose: Avoid tilting camera to far forward or behind the Player.
    [SerializeField] int lockVerMin;
    [SerializeField] int lockVerMax;

    // Invert Mouse Y
    [SerializeField] public bool invertY;

    float xRotation;

    private void Awake()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
        {
            player = null; 
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // No need for cursor, using UI reticle.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
       
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(player != null)
        {
            transform.forward = player.transform.forward;
        }
        
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0) || !gameManager.instance.dontMove)
        {
            // Get Input
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

            // Define xRotation
            if (invertY)
                xRotation += mouseY;
            else
                xRotation -= mouseY;

            // Clamp camera rotation on the X-axis
            xRotation = Mathf.Clamp(xRotation, lockVerMin, lockVerMax);

            // Rotate the camera on the X-axis
            transform.rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y, 0);

            // Rotate the player on the Y-axis\
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                player.transform.Rotate(Vector3.up * mouseX);
            }
        }
    }
}
