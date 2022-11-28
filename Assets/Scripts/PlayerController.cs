using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody playerRB;

    [Header("Speed Parametres")]
    public float force = 30f;
    public float groundDrag = 10f;
    public float airMultiplyer;

    [HideInInspector] public bool canMove = true;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    [Header("Jump Parametres")]
    public float jumpForce = 10f;
    public float jumpCooldown;
    public float playerHight;

    public LayerMask whatIsGorund;

    [SerializeField] private bool canJump = true;
    [SerializeField] private bool isGrounded = true;

    [Header("Wall Attatch")]
    public LayerMask whatIsWalls;

    [SerializeField] private bool canAttatch = false;
    [SerializeField] private bool isWalledRight = false;
    [SerializeField] private bool isWalledLeft = false;


    void Start()
    {
        playerRB = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHight * 0.5f + 0.1f, whatIsGorund);

        Debug.DrawRay(transform.position, Vector3.down, Color.green);
        Debug.DrawRay(transform.position, transform.right, Color.blue);
        Debug.DrawRay(transform.position, -transform.right, Color.red);
        
        if (canMove)
        {
            PlayerInput();
            SpeedControl();

            if (isGrounded)
            {
                playerRB.drag = groundDrag;
                canAttatch = true;
            }
            else
            {
                playerRB.drag = 1f;
            }
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space) && canJump && isGrounded)
        {
            canJump = false;

            JumpMechanic();

            Invoke(nameof(JumpReset), jumpCooldown);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump)
            {
                if(isWalledRight || isWalledLeft)
                {
                    canJump = false;
                    canAttatch = false;

                    JumpMechanic();

                    Invoke(nameof(JumpReset), jumpCooldown);
                }
            }
            if (canAttatch)
            {
                canAttatch = false;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            AttatchWallRight();
            AttatchWallLeft();
        }
    }

    void AttatchWallLeft()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.right, out hit, playerHight * 0.25f + 0.2f, whatIsWalls))
        {
            if (canAttatch)
            {
                isWalledLeft = true;

                //transform.position = new Vector3(transform.position.x, hit.transform.position.y, transform.position.z);
                playerRB.useGravity = false;
            }
            else
            {
                isWalledLeft = false;
                playerRB.useGravity = true;
            }
        }
        else
        {
            isWalledLeft = false;
            playerRB.useGravity = true;
        }
    }

    void AttatchWallRight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, playerHight * 0.25f + 0.2f, whatIsWalls))
        {
            if (canAttatch)
            {
                isWalledRight = true;

                //transform.position = new Vector3(transform.position.x, hit.transform.position.y, transform.position.z);
                playerRB.useGravity = false;
            }
            else
            {
                isWalledRight = false;
                playerRB.useGravity = true;
            }
        }
        else
        {
            isWalledRight = false;
            playerRB.useGravity = true;
        }
    }

    void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if (isGrounded)
        {
            playerRB.AddForce(moveDirection.normalized * force * 10f, ForceMode.Force);
        }
        else if (isWalledRight || isWalledLeft)
        {
            playerRB.AddForce(moveDirection.normalized * force * 6f, ForceMode.Force);
        }
        else
        {
            playerRB.AddForce(moveDirection.normalized * force * 10f * airMultiplyer, ForceMode.Force);
        }
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);
        float newForce = force / 2;

        if(flatVel.magnitude > newForce)
        {
            Vector3 limitedVel = flatVel.normalized * newForce;
            playerRB.velocity = new Vector3(limitedVel.x, playerRB.velocity.y, limitedVel.z);
        }
    }

    void JumpMechanic()
    {
        playerRB.velocity = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z);

        playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void JumpReset()
    {
        canJump = true;
        canAttatch = true;
    }
}
