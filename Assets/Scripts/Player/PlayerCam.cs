using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotaion;
    float yRotaion;

    private void Awake()
    {
        
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotaion += mouseX;

        xRotaion -= mouseY;
        xRotaion = Mathf.Clamp(xRotaion, -90f, 90f);

        // Rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotaion, yRotaion, 0);
        orientation.rotation = Quaternion.Euler(0, yRotaion, 0);
    }
}
