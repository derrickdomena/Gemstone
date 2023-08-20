using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    Vector3 swayPos;

    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    Vector3 swayEulerRotation;

    [Header("Bobbing")]
    public float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    Vector3 bobPosition;

    public float bobIntensity;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    Vector3 bobEulerRotation;

    // Store inputs
    Vector2 walkInput;
    Vector2 lookInput;

    float smoothing = 10f;
    float smoothRotation = 12f;

    bool isGrounded;

    private void Update()
    {
        // Get's the player's grounded state
        isGrounded = gameManager.instance.playerScript.controller.isGrounded;

        // Updates the bobIntensity when left shift is pressed
        // Gives a feeling of the player running
        //if (Input.GetKey(KeyCode.LeftShift))
        //{
        //    bobIntensity = 60;
        //}
        //else
        //{
        //    bobIntensity = 35;
        //}

        // Get player's input
        GetInput();

        // Get movement and rotation
        Sway();
        SwayRotation();
        // Used to generate the sin and cos waves
        speedCurve += Time.deltaTime * (isGrounded ? (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical")) * bobIntensity : 1f) + 0.01f;
        Bob();
        BobRotation();

        // Set's the position and rotation
        SetPositionRotation();
    }

    // Gets all inputs (Player & Camera)
    void GetInput()
    {
        // Player movement
        walkInput.x = Input.GetAxis("Horizontal");
        walkInput.y = Input.GetAxis("Vertical");
        walkInput = walkInput.normalized;

        // Camera movement
        lookInput.x = Input.GetAxis("Mouse X");
        lookInput.y = Input.GetAxis("Mouse Y");
    }

    // Position change in relation to mouse movement
    void Sway()
    {
        // Reverses the vectors direction and makes the vector smaller
        Vector3 invertLook = lookInput * -step;
        // Clamp the x and y values to the max step distance
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook; // Stores the value of invertLook
    }

    // Rotation change in relation to mouse movement
    void SwayRotation()
    {
        // Reverses the vectors direction and makes the vector smaller
        Vector2 invertLook = lookInput * -rotationStep;
        // Clamp the x and y values to the max step distance
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);

        swayEulerRotation = new Vector3(invertLook.y, invertLook.x, invertLook.x); // Stores the pitch, yaw, and roll axis
    }

    // Position change as a result of walking
    void Bob()
    {
        bobPosition.x = (curveCos * bobLimit.x * (isGrounded ? 1 : 0)) - (walkInput.x * travelLimit.x);

        bobPosition.y = (curveSin * bobLimit.y) - (Input.GetAxis("Vertical") * travelLimit.y);

        bobPosition.z = - (walkInput.y * travelLimit.z);
    }

    // Rotation change as a result of walking
    void BobRotation()
    {
        bobEulerRotation.x = (walkInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : 
                                                          multiplier.x * (Mathf.Sin(2 * speedCurve) / 2)); // pitch

        bobEulerRotation.y = (walkInput != Vector2.zero ? multiplier.y * curveCos : 0); // yaw

        bobEulerRotation.z = (walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0); // roll
    }

    void SetPositionRotation()
    {
        // Position
        transform.localPosition = 
            Vector3.Lerp(transform.localPosition, 
            swayPos + bobPosition, 
            Time.deltaTime * smoothing);

        // Rotation
        transform.localRotation = 
            Quaternion.Slerp(transform.localRotation, 
            Quaternion.Euler(swayEulerRotation) * Quaternion.Euler(bobEulerRotation), 
            Time.deltaTime * smoothRotation);
    }


}
