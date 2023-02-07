using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceFieldShooter : MonoBehaviour
{
    [HideInInspector] public bool canShoot = true;
    private bool isGrappleInstatiated;
    [HideInInspector] public bool isTimerOn;
    private bool canReload;

    [Header("Instance Parameters\n")]
    [SerializeField] private float speed;
    public float cooldown = 2f;
    public float lifeTime = 10f;
    [HideInInspector] public float currentLife;


    [HideInInspector] public int maxInstances;
    [HideInInspector] public int currentInstance;

    private int mode;

    [SerializeField] public Transform shootPoint, cam;

    [Header("Instances\n")]
    [SerializeField] private GameObject forceField;
    [SerializeField] private GameObject dobleJump;
    [SerializeField] private GameObject[] instance;

    public List<GameObject> forceFields = new List<GameObject>();

    private Rigidbody _rigidbody;

    private SphereCollider _sphereCollider;

    private PlayerController playerControllerScript;

    [Header("Others\n")]
    [SerializeField] private LayerMask whatIsReloadTerminal;

    [SerializeField] private float clickDist;
    public Animator _animator;

    [Header("Hand Color\n")]

    private Material sphereMat;

    [SerializeField] private GameObject sphere;

    [ColorUsage(true, true)]
    [SerializeField] private Color matColorBlue;
    [ColorUsage(true, true)]
    [SerializeField] private Color matColorPurple;

    [SerializeField] private Material purpleMat;
    [SerializeField] private Material blueMat;


    private void Start()
    {
        //Get scripts from scene and store them in variables
        playerControllerScript = FindObjectOfType<PlayerController>();

        mode = 0;
        currentInstance = 0;
        currentLife = lifeTime;
    }

    private void Update()
    {
        PlayerInput();

        Timer(lifeTime);

        //Sets the color of an image determined by on if an int mode changes frome 0 to 1
        if (mode == 1)
        {
            sphere.GetComponent<Image>().color = matColorPurple;
        }
        else
        {
            sphere.GetComponent<Image>().color = matColorBlue;
        }

        //Causes that if mode 1 has been triggered to switch to mode 0
        if (instance[1] != null)
        {
            isGrappleInstatiated = true;
            mode = 0;
        }
        else
        {
            isGrappleInstatiated = false;
        }

        //Sets the courrent instance to the max even if the mode 1 has not been used
        if (instance[1] == null && currentInstance > 0)
        {
            currentInstance = forceFields.Count;
        }

        //Sets the ability to reload if the player shoots at least one time
        if(forceFields.Count > 0)
        {
            canReload = true;
        }
    }

    void PlayerInput()
    {
        //Shoots and stops a forcefield
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            Shoot();
        }
        else if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.JoystickButton5))
        {
            StopForceField();
        }

        //Reloads if a raycast hits a certain layer
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            RaycastHit hit;

            if (Physics.Raycast(cam.transform.position, cam.forward, out hit, clickDist, whatIsReloadTerminal))
            {
                Reload();
            }
        }

        //Toggles bettween the 2 modes
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyUp(KeyCode.JoystickButton3))
        {
            if(mode == 1)
            {
                mode = 0;
            }
            else
            {
                mode = 1;
            }
        }
    }

    //Timer logic that start when a mode 1 forcefield is instantiated
    void Timer(float t)
    {
        if (instance[1] != null && isTimerOn)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                isTimerOn = false;

                instance[1].GetComponent<Animator>().Play("forcefield_destroy");

                Destroy(instance[1], 0.5f);

                currentLife = t;
            }

            currentLife -= Time.deltaTime;

            if (currentLife <= 0.0f)
            {
                isTimerOn = false;

                instance[1].GetComponent<Animator>().Play("forcefield_destroy");

                Destroy(instance[1], 0.5f);
            }
        }
        else
        {
            currentLife = t;
        }
    }

    //Shoot logic
    void Shoot()
    {
        if (canShoot && currentInstance < maxInstances)
        {
            playerControllerScript.PlayerSFX(playerControllerScript.sfxs[2], 1, 1.5f);

            _animator.Play("arm_shoot");

            canShoot = false;

            currentInstance++;

            //Determinates whitch forcefield to shoot depending on the mode
            if(mode == 0)
            {
                instance[mode] = Instantiate(dobleJump, shootPoint.position, cam.transform.rotation);
            }
            else if(mode == 1 && !isGrappleInstatiated)
            {
                instance[mode] = Instantiate(forceField, shootPoint.position, cam.transform.rotation);

                isTimerOn = true;
            }
            else
            {
                mode = 0;
                instance[mode] = Instantiate(dobleJump, shootPoint.position, cam.transform.rotation);
            }

            //Sets a rigidbody variable and a collider variable to be the same as the last forcefield instantiated
            _rigidbody = instance[mode].GetComponentInChildren<Rigidbody>();
            _sphereCollider = instance[mode].GetComponentInChildren<SphereCollider>();

            _sphereCollider.enabled = false;

            //If is in the shooting state adds a forward force the the rigidbody of the last forcefield instantiated
            _rigidbody.AddForce(instance[mode].transform.forward * speed * 10, ForceMode.Force);

            //Adds the last forcefield instantiated to a list
            forceFields.Add(instance[mode]);

            StartCoroutine(Cooldown());
        }
    }

    //Stop Forcefield logic
    void StopForceField()
    {
        //If there is a forcefield instantiated stops the rigidbodys velocity of the last forcefield instantiated
        if (_rigidbody != null)
        {
            if (!canShoot && _rigidbody.velocity != Vector3.zero)
            {
                if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("arm_reload"))
                {
                    _animator.Play("arm_reload");
                }
            }

            _rigidbody.velocity = Vector3.zero;
            _sphereCollider.enabled = true;
        }
    }

    //Reload logic
    public void Reload()
    {
        //Chechs if the player can reload
        if (forceFields.Count > 0 && canReload)
        {
            _animator.Play("arm_shoot");

            canShoot = false;
            canReload = false;

            //Destroys all the instatiated forcefields on the scene with a delay so that an animation of the forcefields can be played
            foreach (GameObject g in forceFields)
            {
                if (g != null)
                {
                    g.GetComponent<Animator>().Play("forcefield_destroy");

                    Destroy(g, 0.50f);
                }
            }

            //Destroys all the instatiated forcefields on the scene with a delay so that an animation of the forcefields can be played
            foreach (GameObject g in instance)
            {
                Destroy(g, 0.50f);
            }

            //Clears the list of forcefields
            forceFields.Clear();

            //Sets the currentinstance to 0
            currentInstance = 0;

            _animator.Play("arm_reload");

            //Resets all the paramters
            canShoot = true;
            isGrappleInstatiated = false;
            isTimerOn = false;
            playerControllerScript.canSecondJump = false;
            canReload = true;
        }
    }

    //When called starts a cooldown timer
    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);

        StopForceField();

        if (currentInstance < maxInstances)
        {
            canShoot = true;
        }
    }
}