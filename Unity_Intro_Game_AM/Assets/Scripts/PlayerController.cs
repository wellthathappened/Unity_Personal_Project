using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody myRB;
    Camera playerCam;

    Vector2 camRotation;

    [Header("Movement Stats")]
    public bool sprinting = false;
    public float speed = 10f;
    public float sprintMult = 1.5f;
    public float jumpHeight = 5f;
    public float groundDetection = 1f;

    [Header("User Settings")]
    public bool sprintToggle = false;
    public float mouseSensitivity = 2.0f;
    public float Xsensitivity = 2.0f;
    public float Ysensitivity = 2.0f;
    public float camRotationLimit = 90f;

    // Start is called before the first frame update
    void Start()
    {
        // Initialized components
        myRB = GetComponent<Rigidbody>();
        playerCam = transform.GetChild(0).GetComponent<Camera>();

        // Camera setup
        camRotation = Vector2.zero;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        // FPS Camera Rotation
        camRotation.x += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        camRotation.y += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        // Limit vertical rotation
        camRotation.y = Mathf.Clamp(camRotation.y, -camRotationLimit, camRotationLimit);

        // Set camera rotation on the vertical axis | Set player rotation on horizontal axis
        playerCam.transform.localRotation = Quaternion.AngleAxis(camRotation.y, Vector3.left);
        transform.localRotation = Quaternion.AngleAxis(camRotation.x, Vector3.up);


        // Sprint turn on for toggle & not toggle
        if ((!sprinting) && ((!sprintToggle && Input.GetKey(KeyCode.LeftShift)) || (sprintToggle && Input.GetKey(KeyCode.LeftShift) && (Input.GetAxisRaw("Vertical") > 0))))
            sprinting = true;


        // Movement math calculation velocity measured by input * speed
        Vector3 temp = myRB.velocity;

        temp.x = Input.GetAxisRaw("Horizontal") * speed;
        temp.z = Input.GetAxisRaw("Vertical") * speed;

        // If sprinting, check to see if disable condition flags (also amplify speed if sprinting)
        if (sprinting)
        {
            temp.z *= sprintMult;

            if ((sprintToggle && (Input.GetAxisRaw("Vertical") <= 0)) || (!sprintToggle && Input.GetKeyUp(KeyCode.LeftShift)))
                sprinting = false;
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, -transform.up, groundDetection))
            temp.y = jumpHeight;

        // Give calculated velocity back to rigidbody
        myRB.velocity = (transform.forward * temp.z) + (transform.right * temp.x) + (transform.up * temp.y);
    }
}
