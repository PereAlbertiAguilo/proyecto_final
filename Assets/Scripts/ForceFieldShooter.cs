using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] public GameObject destroyParticle;
    [SerializeField] private GameObject[] instance;

    private Rigidbody _rigidbody;

    private SphereCollider _sphereCollider;

    private PlayerController playerController;

    [Header("Others\n")]
    [SerializeField] private float clickDist;
    [SerializeField] private Animator _animator;
    [SerializeField] private new Transform camera;


    [Header("Hand Color\n")]
    [SerializeField] private Material handMat;

    [ColorUsage(true, true)]
    [SerializeField] private Color matColorBlue;
    [ColorUsage(true, true)]
    [SerializeField] private Color matColorPurple;

    public List<GameObject> forceFields = new List<GameObject>();

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        mode = 0;
        currentInstance = 0;
        currentLife = lifeTime;
    }

    private void Update()
    {
        PlayerInput();

        Timer(lifeTime);

        if (mode == 1)
        {
            handMat.SetColor("_EmissionColor", matColorPurple);
        }
        else
        {
            handMat.SetColor("_EmissionColor", matColorBlue);
        }

        if (instance[1] != null)
        {
            isGrappleInstatiated = true;
            mode = 0;
        }
        else
        {
            isGrappleInstatiated = false;
        }

        if (instance[1] == null && currentInstance > 0)
        {
            currentInstance = forceFields.Count;
        }

        if(forceFields.Count > 0)
        {
            canReload = true;
        }
    }

    void PlayerInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Shoot();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopForceField();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Fill());
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mode = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            mode = 1;
        }
    }

    void Timer(float t)
    {
        if (instance[1] != null && isTimerOn)
        {
            currentLife -= Time.deltaTime;

            if (currentLife <= 0.0f)
            {
                isTimerOn = false;

                instance[1].GetComponent<Animator>().Play("forcefield_destroy");
                Instantiate(destroyParticle, instance[1].transform.position, Quaternion.identity);

                Destroy(instance[1], 0.5f);
            }
        }
        else
        {
            currentLife = t;
        }
    }

    void Shoot()
    {
        if (canShoot && currentInstance < maxInstances)
        {
            _animator.Play("arm_shoot");

            canShoot = false;

            currentInstance++;

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

            _rigidbody = instance[mode].GetComponentInChildren<Rigidbody>();
            _sphereCollider = instance[mode].GetComponentInChildren<SphereCollider>();

            _sphereCollider.enabled = false;
            _rigidbody.AddForce(instance[mode].transform.forward * speed * 10, ForceMode.Force);
            forceFields.Add(instance[mode]);

            StartCoroutine(Cooldown());
        }
    }
    void StopForceField()
    {
        if(_rigidbody != null)
        {
            if (!canShoot && _rigidbody.velocity != Vector3.zero)
            {
                _animator.Play("arm_reload");
            }

            _rigidbody.velocity = Vector3.zero;
            _sphereCollider.enabled = true;
        }
    }

    IEnumerator Fill()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, clickDist))
        {
            if (hit.transform.CompareTag("Reset"))
            {
                if (playerController.isGrounded && forceFields.Count > 0 && canReload)
                {
                    _animator.Play("arm_shoot");

                    canShoot = false;
                    canReload = false;

                    foreach (GameObject g in forceFields)
                    {
                        if (g != null)
                        {
                            Instantiate(destroyParticle, g.transform.position, Quaternion.identity);
                            g.GetComponent<Animator>().Play("forcefield_destroy");

                            Destroy(g, 0.50f);
                        }
                    }

                    foreach (GameObject g in instance)
                    {
                        Destroy(g, 0.50f);
                    }

                    forceFields.Clear();

                    yield return new WaitForSeconds(0.1f);

                    currentInstance = 0;

                    _animator.Play("arm_reload");

                    canShoot = true;
                    isGrappleInstatiated = false;
                    isTimerOn = false;
                    canReload = true;
                }
            }
        }
    }

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