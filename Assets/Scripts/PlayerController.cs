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

    [SerializeField] private bool canWallJumpRight = false;
    [SerializeField] private bool canWallJumpLeft = false;


    [Header("Wall Attatch")]
    public LayerMask whatIsWalls;

    [SerializeField] private bool canAttatch = false;
    [SerializeField] private bool isWalledRight = false;
    [SerializeField] private bool isWalledLeft = false;

    [Header("Gravity Modifier")]
    [SerializeField] private float wallGrav = -1f;
    [SerializeField] private float normalGrav = -10f;


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
                canWallJumpRight = false;
                canWallJumpLeft = false;
            }
            else
            {
                playerRB.drag = 1f;
                Physics.gravity = new Vector3(0, normalGrav, 0);
            }

            if (isWalledRight || isWalledLeft)
            {
                Physics.gravity = new Vector3(0, wallGrav, 0);
            }
            else if (!isGrounded)
            {
                Physics.gravity = new Vector3(0, normalGrav, 0);
            }

            if (isWalledRight && !canWallJumpRight)
            {
                canWallJumpRight = true;
                canWallJumpLeft = false;
            }
            
            if (isWalledLeft && !canWallJumpLeft)
            {
                canWallJumpLeft = true;
                canWallJumpRight = false;
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

        if (canAttatch)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                AttatchWallRight();
                AttatchWallLeft();
            }
            else
            {
                isWalledRight = false;
                isWalledLeft = false;
                Invoke(nameof(WallJumpReset), jumpCooldown);
            }
        }
        else
        {
            Physics.gravity = new Vector3(0, normalGrav, 0);
            isWalledRight = false;
            isWalledLeft = false;
            canWallJumpLeft = false;
            canWallJumpRight = false;
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (canJump && canAttatch)
            {
                if (canWallJumpRight)
                {
                    canJump = false;
                    canAttatch = false;
                    canWallJumpRight = false;

                    WallJumpMechanic(-transform.right);

                    Invoke(nameof(JumpReset), jumpCooldown);
                }
                
                if (canWallJumpLeft)
                {
                    canJump = false;
                    canAttatch = false;
                    canWallJumpLeft = false;

                    WallJumpMechanic(transform.right);

                    Invoke(nameof(JumpReset), jumpCooldown);
                }
            }
        }
    }

    void AttatchWallLeft()
    {
        isWalledLeft = Physics.Raycast(transform.position, -transform.right, playerHight * 0.25f + 0.2f, whatIsWalls);
    }

    void AttatchWallRight()
    {
        isWalledRight = Physics.Raycast(transform.position, transform.right, playerHight * 0.25f + 0.2f, whatIsWalls);
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
            playerRB.AddForce(moveDirection.normalized * force * 3f, ForceMode.Force);
        }
        else
        {
            playerRB.AddForce(moveDirection.normalized * force * 8f * airMultiplyer, ForceMode.Force);
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

    void WallJumpMechanic(Vector3 direction)
    {
        playerRB.velocity = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z);

        playerRB.AddForce(Vector3.up * jumpForce * 0.7f, ForceMode.Impulse);
        playerRB.AddForce(direction * jumpForce * 0.8f, ForceMode.Impulse);
    }

    void JumpReset()
    {
        canJump = true;
        canAttatch = true;
    }

    void WallJumpReset()
    {
        canJump = true;
        canAttatch = true;
        canWallJumpRight = false;
        canWallJumpLeft = false;
    }
}
