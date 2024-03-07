using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;
    public float airDrag;
    public float gravity;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Dash")]
    public float dashForce;
    public float dashCooldown;
    bool readyToDash;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool isGrounded;

    public Transform orientation;

    [Header("UI")]
    public TextMeshProUGUI speedometer;

    [Header("Input")]
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Start()
    {
        readyToJump = true;
        readyToDash = true;
    }

    void Update()
    {
        isGrounded = Grounded();

        MyInput();
        //SpeedControl();

        // Handle drag
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }

        speedometer.text = rb.velocity.magnitude.ToString();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private bool Grounded()
    {
        Vector3 castOrigin = new Vector3(transform.position.x, transform.position.y - (playerHeight * 0.5f) + 0.2f, transform.position.z);
        return Physics.SphereCast(transform.position, 0.4f, Vector3.down, out RaycastHit hitInfo, whatIsGround);
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        // Jump
        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Dash
        if (Input.GetKey(dashKey) && readyToDash)
        {
            readyToDash = false;
            Dash();

            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    private void Move()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (isGrounded) // On Ground
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10, ForceMode.Force);
        }
        else if (!isGrounded) // In Air
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10 * airMultiplier, ForceMode.Force);

        }
        // apply gravity
        rb.AddForce(Vector3.down * gravity, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z); ;
        }
    }

    #region Jump
    public void Jump()
    {
        // Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
    #endregion

    #region Dash
    public void Dash()
    {
        // Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(moveDirection * dashForce, ForceMode.Impulse);
    }

    public void ResetDash()
    {
        readyToDash = true;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y - (playerHeight * 0.5f) + 0.2f, transform.position.z), 0.4f);
    }
}
