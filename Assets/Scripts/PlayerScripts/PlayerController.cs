using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Rigidbody _playerRigidbody;

    private GrapplingController grapplingControllerScript;
    private ForceFieldShooter forceFieldShooterScript;
    private DoorOpener[] doorOpenerScript;
    private UIManager UIManagerScript;

    public Transform virtualCam;
    [HideInInspector] public CinemachineVirtualCamera cvCam;
    [HideInInspector] public CinemachinePOV cPOV;
    [SerializeField] private Transform cam;

    [HideInInspector] public static bool playerCreated;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool activateSpeedControl = true;
    public bool forceFieldsActive;
    private bool canSlide = true;
    private bool isSliding;
    private bool isRunning;
    private bool permaCrouch;

    [Header("Speed Parametres\n")]
    [SerializeField] private float force = 30f;
    [SerializeField] private float groundDrag = 10f;
    [SerializeField] private float airMultiplyer;
    public float fieldOfView;

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
    public float normalGrav = -10f;

    [Header("Audio Parameters\n")]
    [SerializeField] private float defaultVolume;
    [SerializeField] private float defaultPitch;
    public AudioClip[] sfxs;

    [SerializeField] private AudioSource _sfxAudioSource;
    private AudioSource _playerAudioSource;

    void Start()
    {
        playerCreated = true;

        _playerRigidbody = GetComponent<Rigidbody>();
        _playerAudioSource = GetComponent<AudioSource>();

        grapplingControllerScript = FindObjectOfType<GrapplingController>();
        doorOpenerScript = FindObjectsOfType<DoorOpener>(true);
        forceFieldShooterScript = FindObjectOfType<ForceFieldShooter>();
        UIManagerScript = FindObjectOfType<UIManager>();

        cvCam = virtualCam.GetComponent<CinemachineVirtualCamera>();

        Cursor.lockState = CursorLockMode.Locked;

        wallJumpDir = Vector3.forward;

        cPOV = cvCam.GetCinemachineComponent<CinemachinePOV>();

        if (PlayerPrefs.HasKey("fov"))
        {
            fieldOfView = PlayerPrefs.GetFloat("fov");

            cvCam.m_Lens.FieldOfView = PlayerPrefs.GetFloat("fov");
        }
        if (PlayerPrefs.HasKey("sensX"))
        {
            cPOV.m_HorizontalAxis.m_MaxSpeed = PlayerPrefs.GetFloat("sensX");
        }
        if (PlayerPrefs.HasKey("sensY"))
        {
            cPOV.m_VerticalAxis.m_MaxSpeed = PlayerPrefs.GetFloat("sensY");
        }
    }

    private void Update()
    {
        if (canMove)
        {
            PlayerInput();
            SpeedControl();
            Running(isRunning);
            PlayerWalkSFX();

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

        foreach (DoorOpener door in doorOpenerScript)
        {
            door.canInteract = Physics.Raycast(cam.position, cam.forward, door.interactDist, door.whatIsInteractable);
        }
    }

    void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        float leftTrigger = Input.GetAxis("LeftTrigger");
        float rightTrigger = Input.GetAxisRaw("RightTrigger");

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0))
        {
            if (canJump && isGrounded)
            {
                canJump = false;

                JumpMechanic();

                Invoke(nameof(JumpReset), jumpCooldown);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if(canSecondJump && !grapplingControllerScript.isGrappled && !isGrounded)
            {
                canSecondJump = false;

                JumpMechanic();
            }
        }

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0))
        {
            if (canAttatch)
            {
                isAttatched = true;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.JoystickButton0))
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

        if(UIManagerScript.XboxOneController == 1)
        {
            if (leftTrigger > 0)
            {
                isRunning = true;
            }
            else if (leftTrigger <= 0)
            {
                isRunning = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isRunning = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                isRunning = false;
            }
        }        

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton8))
        {
            if(isGrounded && canSlide)
            {
                canJump = false;
                isSliding = true;
                canSlide = false;
                GetComponent<CapsuleCollider>().height = 1f;
                _playerRigidbody.AddForce(Vector3.down * force, ForceMode.Impulse);
            }
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.JoystickButton8))
        {
            if (!permaCrouch)
            {
                GetComponent<CapsuleCollider>().height = 2f;
                canSlide = true;
                isSliding = false;
                canJump = true;
            }
        }
    }

    void PlayerWalkSFX()
    {
        if (Mathf.Abs(verticalInput) != 0 || Mathf.Abs(horizontalInput) != 0)
        {
            _playerAudioSource.volume = Mathf.Lerp(_playerAudioSource.volume, defaultVolume, 0.05f);
        }
        else if (!_playerAudioSource.mute)
        {
            if (_playerAudioSource.volume >= 0.01f)
            {
                _playerAudioSource.volume = Mathf.Lerp(_playerAudioSource.volume, 0f, Time.deltaTime * 2);
            }
        }

        if (isRunning)
        {
            _playerAudioSource.pitch = Mathf.Lerp(_playerAudioSource.pitch, defaultPitch, Time.deltaTime * 2);
        }
        else
        {
            _playerAudioSource.pitch = Mathf.Lerp(_playerAudioSource.pitch, 1f, 0.05f);
        }
    }

    public void PlayerSFX(AudioClip ac, float f1, float f2)
    {
        float randomIndex = Random.Range(f1, f2);

        _sfxAudioSource.pitch = randomIndex;

        _sfxAudioSource.PlayOneShot(ac);
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
        fov = cvCam.m_Lens.FieldOfView;

        if (b)
        {
            fov = Mathf.Lerp(fov, fieldOfView + 6, 0.05f);
        }
        else if(isGrounded)
        {
            fov = Mathf.Lerp(fov, fieldOfView, 0.05f);
        }

        cvCam.m_Lens.FieldOfView = fov;
    }

    void SpeedControl()
    {
        if (activateSpeedControl)
        {
            Vector3 flatVel = new Vector3(_playerRigidbody.velocity.x, 0, _playerRigidbody.velocity.z);
            float newForce = force / 2f;

            if (flatVel.magnitude > newForce && !isRunning)
            {
                Vector3 limitedVel = flatVel.normalized * newForce;
                _playerRigidbody.velocity = new Vector3(limitedVel.x, _playerRigidbody.velocity.y, limitedVel.z);
            }
        }
    }

    void JumpMechanic()
    {
        _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0f, _playerRigidbody.velocity.z);

        _playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        PlayerSFX(sfxs[0], 1, 1.5f);
    }

    void WallJumpMechanic()
    {
        _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0f, _playerRigidbody.velocity.z);

        _playerRigidbody.AddForce(wallJumpDir * jumpForce * 0.8f, ForceMode.Impulse);
        _playerRigidbody.AddForce(Vector3.up * jumpForce * 0.8f, ForceMode.Impulse);

        PlayerSFX(sfxs[0], 1, 1.5f);
    }

    void JumpReset()
    {
        canJump = true;
    }

    IEnumerator ResetPermaCrouch()
    {
        yield return new WaitForSeconds(.7f);

        if (!isSliding)
        {
            GetComponent<CapsuleCollider>().height = 2f;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("SecondJump"))
        {
            canSecondJump = true;
            Destroy(other.gameObject, 0.45f);
            forceFieldShooterScript._animator.Play("arm_reload");
            other.GetComponent<Animator>().Play("forcefield_destroy");
        }

        if (other.tag.Equals("CheckPoint"))
        {
            forceFieldsActive = forceFieldShooterScript.enabled;
        }

        if (other.tag.Equals("Crouch"))
        {
            GetComponent<CapsuleCollider>().height = 1f;
            _playerRigidbody.AddForce(Vector3.down * force, ForceMode.Impulse);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Crouch"))
        {
            permaCrouch = true;
            canSlide = false;
            canJump = false;
            isSliding = true;
            GetComponent<CapsuleCollider>().height = 1f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Collider>().tag.Equals("Crouch"))
        {
            StartCoroutine(ResetPermaCrouch());
            permaCrouch = false;
            isSliding = false;
            canSlide = true;
            canJump = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag.Equals("Wall"))
        {
            wallJumpDir = other.transform.up;
            canAttatch = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.tag.Equals("Wall"))
        {
            canAttatch = false;
            isAttatched = false;
            canWallJump = false;
        }
    }
}