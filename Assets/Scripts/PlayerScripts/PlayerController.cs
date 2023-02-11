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

    private float oneUnit = 1f;
    private float halfUnit = .5f;
    private float dobleUnit = 2f;
    private float tenthOfUnit = .1f;
    private float normalForceMulti = 10f;
    private float runningForceMulti = 14f;
    private float runningFov = 6f;
    private float wallJumpMulti = .8f;

    void Start()
    {
        //Get player components and store them in variables
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerAudioSource = GetComponent<AudioSource>();

        //Get scripts from scene and store them in variables
        grapplingControllerScript = FindObjectOfType<GrapplingController>();
        doorOpenerScript = FindObjectsOfType<DoorOpener>(true);
        forceFieldShooterScript = FindObjectOfType<ForceFieldShooter>();
        UIManagerScript = FindObjectOfType<UIManager>();

        cvCam = virtualCam.GetComponent<CinemachineVirtualCamera>();

        Cursor.lockState = CursorLockMode.Locked;

        wallJumpDir = Vector3.forward;

        cPOV = cvCam.GetCinemachineComponent<CinemachinePOV>();

        //Set the field of view and the sensiblities if they have been changed in the main menu
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
        //Restricting the movement of the player with a boolean
        if (canMove)
        {
            PlayerInput();
            SpeedControl();
            Running(isRunning);
            PlayerWalkSFX();

            //sets the rotation of the player to be the same as the camera
            transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);

            //Drag value and gravity changes depending on if the player is touching the ground or not
            if (isGrounded)
            {
                _playerRigidbody.drag = groundDrag;
                Physics.gravity = new Vector3(0, normalGrav, 0);
            }
            else
            {
                _playerRigidbody.drag = oneUnit;
            }

            //Gravity changes depending on if the player is attatched to a wall
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
        //Restricting the movement of the player with a boolean
        if (canMove)
        {
            MovePlayer();
        }

        //Raycast that checks if the player is touching the ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHight * halfUnit + tenthOfUnit, whatIsGorund);

        //Raycast that checks if the player can interact with certain objects
        foreach (DoorOpener door in doorOpenerScript)
        {
            door.canInteract = Physics.Raycast(cam.position, cam.forward, door.interactDist, door.whatIsInteractable);
        }
    }

    //All the inputs on whitch the player depends to move arround
    void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        float leftTrigger = Input.GetAxis("LeftTrigger");

        //Jump if the player is on the ground
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0))
        {
            if (canJump && isGrounded)
            {
                canJump = false;

                JumpMechanic();

                Invoke(nameof(JumpReset), jumpCooldown);
            }
        }

        //Jump if the player obtains a second jump power up
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if(canSecondJump && !grapplingControllerScript.isGrappled && !isGrounded)
            {
                canSecondJump = false;

                JumpMechanic();
            }
        }

        //Jump if the player is attatched to a wall
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

        //Player running state
        if(UIManagerScript.XboxOneController == oneUnit)
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

        //Player crouching state
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton8))
        {
            if(isGrounded && canSlide)
            {
                canJump = false;
                isSliding = true;
                canSlide = false;
                GetComponent<CapsuleCollider>().height = halfUnit;
                _playerRigidbody.AddForce(Vector3.down * force, ForceMode.Impulse);
            }
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.JoystickButton8))
        {
            if (!permaCrouch)
            {
                GetComponent<CapsuleCollider>().height = dobleUnit;
                canSlide = true;
                isSliding = false;
                canJump = true;
            }
        }
    }

    //Emits a sound effect if the player is moving in any direction, and changes the pitch if, while moveing, the player start running
    void PlayerWalkSFX()
    {
        if (Mathf.Abs(verticalInput) != 0 || Mathf.Abs(horizontalInput) != 0)
        {
            _playerAudioSource.volume = Mathf.Lerp(_playerAudioSource.volume, defaultVolume, Time.deltaTime * dobleUnit);
        }
        else if (!_playerAudioSource.mute)
        {
            if (_playerAudioSource.volume >= 0.01f)
            {
                _playerAudioSource.volume = Mathf.Lerp(_playerAudioSource.volume, 0f, Time.deltaTime * dobleUnit);
            }
        }

        if (isRunning)
        {
            _playerAudioSource.pitch = Mathf.Lerp(_playerAudioSource.pitch, defaultPitch, Time.deltaTime * dobleUnit);
        }
        else
        {
            _playerAudioSource.pitch = Mathf.Lerp(_playerAudioSource.pitch, oneUnit, Time.deltaTime * dobleUnit);
        }
    }

    //Plays a given audioclip with a random picth value determinated by 2 given floats
    public void PlayerSFX(AudioClip ac, float f1, float f2)
    {
        float randomIndex = Random.Range(f1, f2);

        _sfxAudioSource.pitch = randomIndex;

        _sfxAudioSource.PlayOneShot(ac);
    }

    //All the diferents move speeds of the player depending on if is touching the ground, running, crouching, attatched to a wall, grappling or in free fall
    void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if (isGrounded)
        {
            if (isSliding)
            {
                _playerRigidbody.AddForce(moveDirection.normalized * force * dobleUnit, ForceMode.Force);
            }
            else if (isRunning)
            {
                _playerRigidbody.AddForce(moveDirection.normalized * force * runningForceMulti, ForceMode.Force);
            }
            else
            {
                _playerRigidbody.AddForce(moveDirection.normalized * force * normalForceMulti, ForceMode.Force);
            }
        }
        else if (isAttatched)
        {
            _playerRigidbody.AddForce(moveDirection.normalized * force * dobleUnit, ForceMode.Force);
        }
        else if (grapplingControllerScript.isGrappled)
        {
            _playerRigidbody.AddForce(moveDirection.normalized * force * normalForceMulti * airMultiplyer, ForceMode.Force);
        }
        else
        {
            _playerRigidbody.AddForce(moveDirection.normalized * force * (normalForceMulti - dobleUnit) * airMultiplyer, ForceMode.Force);
        }   
    }

    //Changes the field of view if the player is running
    void Running(bool b)
    {
        float fov;
        fov = cvCam.m_Lens.FieldOfView;

        if (b)
        {
            fov = Mathf.Lerp(fov, fieldOfView + runningFov, Time.deltaTime * dobleUnit);
        }
        else if(isGrounded)
        {
            fov = Mathf.Lerp(fov, fieldOfView, Time.deltaTime * dobleUnit);
        }

        cvCam.m_Lens.FieldOfView = fov;
    }

    //If the player isn't running the players max speed is clamped to a max value
    void SpeedControl()
    {
        if (activateSpeedControl)
        {
            Vector3 flatVel = new Vector3(_playerRigidbody.velocity.x, 0, _playerRigidbody.velocity.z);
            float newForce = force / dobleUnit;

            if (flatVel.magnitude > newForce && !isRunning)
            {
                Vector3 limitedVel = flatVel.normalized * newForce;
                _playerRigidbody.velocity = new Vector3(limitedVel.x, _playerRigidbody.velocity.y, limitedVel.z);
            }
        }
    }

    //When called this function adds an upwards force to the player
    void JumpMechanic()
    {
        _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0f, _playerRigidbody.velocity.z);

        _playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        PlayerSFX(sfxs[0], oneUnit, oneUnit + halfUnit);
    }

    //When called this function adds an upwards force to the player and a force in the opposite directon of the wall that is collideing
    void WallJumpMechanic()
    {
        _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0f, _playerRigidbody.velocity.z);

        _playerRigidbody.AddForce(wallJumpDir * jumpForce * wallJumpMulti, ForceMode.Impulse);
        _playerRigidbody.AddForce(Vector3.up * jumpForce * wallJumpMulti, ForceMode.Impulse);

        PlayerSFX(sfxs[0], oneUnit, oneUnit + halfUnit);
    }

    //When called set the ability to jump to true
    void JumpReset()
    {
        canJump = true;
    }

    //Adds a delay to exit the permacrouch state
    IEnumerator ResetPermaCrouch()
    {
        yield return new WaitForSeconds(halfUnit + tenthOfUnit + tenthOfUnit);

        if (!isSliding)
        {
            GetComponent<CapsuleCollider>().height = dobleUnit;
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        //Sets the ability to jump in mid air
        if (other.tag.Equals("SecondJump"))
        {
            canSecondJump = true;
            Destroy(other.gameObject, halfUnit);
            forceFieldShooterScript._animator.Play("arm_reload");
            other.GetComponent<Animator>().Play("forcefield_destroy");
        }

        //Saves to a checkpoint if the player has the ability to use the forcefieldshooter script
        if (other.tag.Equals("CheckPoint"))
        {
            forceFieldsActive = forceFieldShooterScript.enabled;
        }

        //Enters the permacrouch state
        if (other.tag.Equals("Crouch"))
        {
            GetComponent<CapsuleCollider>().height = halfUnit;
            _playerRigidbody.AddForce(Vector3.down * force, ForceMode.Impulse);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Stays in the permacrouch state
        if (other.tag.Equals("Crouch"))
        {
            permaCrouch = true;
            canSlide = false;
            canJump = false;
            isSliding = true;
            GetComponent<CapsuleCollider>().height = halfUnit;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Exits the permacrouch state
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
        //Lets the player the ability to attatch to wall
        if (other.collider.tag.Equals("Wall"))
        {
            wallJumpDir = other.transform.up;
            canAttatch = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        //Exits the wall attatched state
        if (other.collider.tag.Equals("Wall"))
        {
            canAttatch = false;
            isAttatched = false;
            canWallJump = false;
        }
    }
}