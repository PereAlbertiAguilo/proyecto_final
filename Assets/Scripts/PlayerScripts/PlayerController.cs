using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Rigidbody _playerRigidbody;

    private GrapplingController grapplingControllerScript;
    private DoorOpener doorOpenerScript;

    public Transform virtualCam;
    [SerializeField] private Transform cam;

    [HideInInspector] public static bool playerCreated;
    [HideInInspector] public bool canMove = true;
    public bool forceFieldsActive;
    private bool canSlide = true;
    private bool isSliding;
    private bool isRunning;

    [Header("Speed Parametres\n")]
    [SerializeField] private float force = 30f;
    [SerializeField] private float groundDrag = 10f;
    [SerializeField] private float airMultiplyer;
    [SerializeField] private float fieldOfView;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    [Header("Jump Parametres\n")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float playerHight;

    [SerializeField] private LayerMask whatIsGorund;

    private bool canJump = true;
    [HideInInspector] public bool canSecondJump;
    [HideInInspector] public bool isGrounded = true;

    private Vector3 wallJumpDir;

    private bool canWallJump;
 
    [Header("Wall Attatch\n")]
    [SerializeField] private LayerMask whatIsWalls;

    private bool canAttatch;
    private bool isAttatched;

    [Header("Gravity Modifier\n")]
    [SerializeField] private float wallGrav = -1f;
    [SerializeField] private float normalGrav = -10f;


    void Start()
    {
        playerCreated = true;

        _playerRigidbody = GetComponent<Rigidbody>();

        grapplingControllerScript = FindObjectOfType<GrapplingController>();
        doorOpenerScript = FindObjectOfType<DoorOpener>();

        Cursor.lockState = CursorLockMode.Locked;

        transform.rotation = new Quaternion(0, -180, 0, 0);

        wallJumpDir = Vector3.forward;

        virtualCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = PlayerPrefs.GetFloat("fov");
        print(PlayerPrefs.GetFloat("fov"));
    }

    private void Update()
    {
        if (canMove)
        {
            PlayerInput();
            SpeedControl();
            Running(isRunning);

            transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);

            if (isGrounded)
            {
                _playerRigidbody.drag = groundDrag;
                Physics.gravity = new Vector3(0, normalGrav, 0);
            }
            else
            {
                _playerRigidbody.drag = 1f;
            }

            if (isAttatched)
            {
                Physics.gravity = new Vector3(0, wallGrav, 0);
                canWallJump = true;
            }
            else if (!canAttatch)
            {
                Physics.gravity = new Vector3(0, normalGrav, 0);
            }
            else if (!isGrounded)
            {
                Physics.gravity = new Vector3(0, normalGrav, 0);
            }
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            MovePlayer();
        }

        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHight * 0.5f + 0.1f, whatIsGorund);

        if (doorOpenerScript != null)
        {
            doorOpenerScript.canInteract = Physics.Raycast(cam.position, cam.forward, doorOpenerScript.interactDist, doorOpenerScript.whatIsInteractable);
        }
    }

    void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            if (canJump)
            {
                canJump = false;

                JumpMechanic();

                Invoke(nameof(JumpReset), jumpCooldown);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && canSecondJump && !grapplingControllerScript.isGrappled && !isGrounded)
        {
            canSecondJump = false;

            JumpMechanic();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (canAttatch)
            {
                isAttatched = true;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            if (canWallJump)
            {
                canWallJump = false;
                isAttatched = false;

                WallJumpMechanic();
            }
        }
        else
        {
            isAttatched = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isRunning = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isRunning = false;
        }

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && canSlide)
        {
            canSlide = false;
            isSliding = true;
            GetComponent<CapsuleCollider>().height = 1f;
            _playerRigidbody.AddForce(Vector3.down * force, ForceMode.Impulse);
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift) || !isGrounded)
        {
            GetComponent<CapsuleCollider>().height = 2f;
            canSlide = true;
            isSliding = false;
        }
    }

    void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if (isGrounded)
        {
            if (isSliding)
            {
                _playerRigidbody.AddForce(moveDirection.normalized * force * 2f, ForceMode.Force);
            }
            else if (isRunning)
            {
                _playerRigidbody.AddForce(moveDirection.normalized * force * 14f, ForceMode.Force);
            }
            else
            {
                _playerRigidbody.AddForce(moveDirection.normalized * force * 10f, ForceMode.Force);
            }
        }
        else if (isAttatched)
        {
            _playerRigidbody.AddForce(moveDirection.normalized * force * 2f, ForceMode.Force);
        }
        else if (grapplingControllerScript.isGrappled)
        {
            _playerRigidbody.AddForce(moveDirection.normalized * force * 6f * airMultiplyer, ForceMode.Force);
        }
        else
        {
            _playerRigidbody.AddForce(moveDirection.normalized * force * 8f * airMultiplyer, ForceMode.Force);
        }   
    }

    void Running(bool b)
    {
        float fov;
        fov = virtualCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView;

        if (b)
        {
            fov = Mathf.Lerp(fov, fieldOfView + 6, 0.05f);
        }
        else if(isGrounded)
        {
            fov = Mathf.Lerp(fov, fieldOfView, 0.05f);
        }

        virtualCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = fov;
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_playerRigidbody.velocity.x, 0, _playerRigidbody.velocity.z);
        float newForce = force / 2f;

        if(flatVel.magnitude > newForce && !isRunning)
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

    void WallJumpMechanic()
    {
        _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0f, _playerRigidbody.velocity.z);

        _playerRigidbody.AddForce(wallJumpDir * jumpForce * 0.8f, ForceMode.Impulse);
        _playerRigidbody.AddForce(Vector3.up * jumpForce * 0.8f, ForceMode.Impulse);
    }
    
    void JumpReset()
    {
        canJump = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("SecondJump"))
        {
            canSecondJump = true;
            Destroy(other.gameObject, 0.45f);
            other.GetComponent<Animator>().Play("forcefield_destroy");
        }

        if (other.tag.Equals("CheckPoint"))
        {
            forceFieldsActive = other.GetComponent<CheckPoint>().forceFieldsActive;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Wall"))
        {
            wallJumpDir = other.transform.up;
            canAttatch = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Wall"))
        {
            canAttatch = false;
            isAttatched = false;
            canWallJump = false;
        }
    }
}