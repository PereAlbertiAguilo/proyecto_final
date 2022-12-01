using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody _playerRigidbody;

    private GrapplingController grapplingController;

    [Header("Speed Parametres")]
    public float force = 30f;
    private float defaultForce;
    public float forceMultiplyer = 2f;
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

    private bool canJump = true;
    private bool isGrounded = true;

    private bool canWallJumpRight;
    private bool canWallJumpLeft;
    private bool canWallJumpFront;
    private bool canWallJumpBack;

    [Header("Wall Attatch")]
    public LayerMask whatIsWalls;

    private bool canAttatch;
    private bool isWalledRight;
    private bool isWalledLeft;
    private bool isWalledFront;
    private bool isWalledBack;

    [Header("Gravity Modifier")]
    [SerializeField] private float wallGrav = -1f;
    [SerializeField] private float normalGrav = -10f;


    void Start()
    {
        defaultForce = force;
        _playerRigidbody = GetComponent<Rigidbody>();
        grapplingController = FindObjectOfType<GrapplingController>();

        Cursor.lockState = CursorLockMode.Locked;

        transform.rotation = new Quaternion(0, -180, 0, 0);
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
                _playerRigidbody.drag = groundDrag;
                canAttatch = true;
                CantWallJump();
                Physics.gravity = new Vector3(0, normalGrav, 0);
            }
            else
            {
                _playerRigidbody.drag = 1f;
            }

            if (isWalledRight || isWalledLeft || isWalledBack || isWalledFront)
            {
                Physics.gravity = new Vector3(0, wallGrav, 0);
            }
            else if (!isGrounded)
            {
                Physics.gravity = new Vector3(0, normalGrav, 0);
            }

            if (isWalledRight)
            {
                canWallJumpRight = true;
                ResetOtherJumps(isWalledBack, isWalledFront, isWalledLeft);
            }
            else if (isWalledLeft)
            {
                canWallJumpLeft = true;
                ResetOtherJumps(isWalledBack, isWalledFront, isWalledRight);
            }
            else if (isWalledFront)
            {
                canWallJumpFront = true;
                ResetOtherJumps(isWalledBack, isWalledLeft, isWalledRight);
            }
            else if (isWalledBack)
            {
                canWallJumpBack = true;
                ResetOtherJumps(isWalledLeft, isWalledFront, isWalledRight);
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
                AttatchWallFront();
                AttatchWallBack();

                if (!isWalledRight || !isWalledLeft || !isWalledBack || !isWalledFront)
                {
                    Physics.gravity = new Vector3(0, normalGrav, 0);
                    CantWallJump();
                }
            }
            else
            {
                NotWalled();
                Invoke(nameof(WallJumpReset), jumpCooldown);
            }
        }
        else
        {
            Physics.gravity = new Vector3(0, normalGrav, 0);

            NotWalled();
            CantWallJump();
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

                    Invoke(nameof(WallJumpReset), jumpCooldown);
                }
                
                if (canWallJumpLeft)
                {
                    canJump = false;
                    canAttatch = false;
                    canWallJumpLeft = false;

                    WallJumpMechanic(transform.right);

                    Invoke(nameof(WallJumpReset), jumpCooldown);
                }

                if (canWallJumpFront)
                {
                    canJump = false;
                    canAttatch = false;
                    canWallJumpFront = false;

                    WallJumpMechanic(-transform.forward);

                    Invoke(nameof(WallJumpReset), jumpCooldown);
                }

                if (canWallJumpBack)
                {
                    canJump = false;
                    canAttatch = false;
                    canWallJumpBack = false;

                    WallJumpMechanic(transform.forward);

                    Invoke(nameof(WallJumpReset), jumpCooldown);
                }
            }
        }

        force = defaultForce;

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            float newForce;

            newForce = force * forceMultiplyer;

            force = newForce;
        }
    }

    void AttatchWallLeft()
    {
        isWalledLeft = Physics.Raycast(transform.position, -transform.right, playerHight * 0.25f + 0.25f, whatIsWalls);
    }

    void AttatchWallRight()
    {
        isWalledRight = Physics.Raycast(transform.position, transform.right, playerHight * 0.25f + 0.25f, whatIsWalls);
    }

    void AttatchWallFront()
    {
        isWalledFront = Physics.Raycast(transform.position, transform.forward, playerHight * 0.25f + 0.25f, whatIsWalls);
    }

    void AttatchWallBack()
    {
        isWalledBack = Physics.Raycast(transform.position, -transform.forward, playerHight * 0.25f + 0.25f, whatIsWalls);
    }

    void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if (isGrounded)
        {
            _playerRigidbody.AddForce(moveDirection.normalized * force * 10f, ForceMode.Force);
        }
        else if (isWalledRight || isWalledLeft || isWalledBack || isWalledFront)
        {
            _playerRigidbody.AddForce(moveDirection.normalized * force * 2f, ForceMode.Force);
        }
        else if (grapplingController.isGrappled)
        {
            _playerRigidbody.AddForce(moveDirection.normalized * force * 6f * airMultiplyer, ForceMode.Force);
        }
        else
        {
            _playerRigidbody.AddForce(moveDirection.normalized * force * 8f * airMultiplyer, ForceMode.Force);
        }
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_playerRigidbody.velocity.x, 0, _playerRigidbody.velocity.z);
        float newForce = force / 2;

        if(flatVel.magnitude > newForce)
        {
            Vector3 limitedVel = flatVel.normalized * newForce;
            _playerRigidbody.velocity = new Vector3(limitedVel.x, _playerRigidbody.velocity.y, limitedVel.z);
        }
    }

    void JumpMechanic()
    {
        _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0f, _playerRigidbody.velocity.z);

        _playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void WallJumpMechanic(Vector3 direction)
    {
        _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0f, _playerRigidbody.velocity.z);

        _playerRigidbody.AddForce(Vector3.up * jumpForce * 0.7f, ForceMode.Impulse);
        _playerRigidbody.AddForce(direction * jumpForce * 0.8f, ForceMode.Impulse);
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
        CantWallJump();
    }

    void NotWalled()
    {
        isWalledBack = false;
        isWalledFront = false;
        isWalledLeft = false;
        isWalledRight = false;
    }

    void CantWallJump()
    {
        canWallJumpBack = false;
        canWallJumpFront = false;
        canWallJumpLeft = false;
        canWallJumpRight = false;
    }

    void ResetOtherJumps(bool b1, bool b2, bool b3)
    {
        b1 = false;
        b2 = false;
        b3 = false;
    }
}
