using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldShooter : MonoBehaviour
{

    private bool canShoot = true;
    private bool isShooting;

    [Header("Instance Parameters\n")]
    [SerializeField] private float speed;
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private int maxInstances;
    [HideInInspector] public int currentInstance;

    private int mode;

    [SerializeField] public Transform shootPoint, cam;

    [Header("Instances\n")]
    [SerializeField] GameObject forceField;
    [SerializeField] GameObject dobleJump;
    [SerializeField] GameObject particle;
    private GameObject instance;

    private Rigidbody _rigidbody;

    private SphereCollider _sphereCollider;

    private PlayerController playerController;

    [Header("\n")]
    [SerializeField] private Animator _animator;

    private List<GameObject> forceFields = new List<GameObject>();

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        mode = 1;
        currentInstance = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mode = 1;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            mode = 2;
        } 


        if (Input.GetMouseButtonDown(1))
        {
            Shoot();
            _animator.Play("arm_shoot");
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopForceField();
            _animator.Play("arm_reload");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(forceFields.Count > 0 && playerController.isGrounded)
            {
                foreach (GameObject g in forceFields)
                {
                    Destroy(g);
                    currentInstance = 0;
                }
            }
        }
    }

    void Shoot()
    {
        /*
        if (forceFields.Count >= maxInstances)
        {
            Destroy(forceFields[0]);
            forceFields.Remove(forceFields[0]);
        }
        */
        if (canShoot && currentInstance < maxInstances)
        {
            canShoot = false;

            currentInstance++;

            if(mode == 1)
            {
                instance = Instantiate(forceField, shootPoint.position, cam.transform.rotation);
            }
            else if(mode == 2)
            {
                instance = Instantiate(dobleJump, shootPoint.position, cam.transform.rotation);
            }

            Instantiate(particle, shootPoint.position, cam.transform.rotation);
            _rigidbody = instance.GetComponentInChildren<Rigidbody>();
            _sphereCollider = instance.GetComponentInChildren<SphereCollider>();
            _sphereCollider.enabled = false;
            _rigidbody.AddForce(instance.transform.forward * speed * 10, ForceMode.Force);
            forceFields.Add(instance);

            StartCoroutine(Cooldown());
            StartCoroutine(MaxDisplacement());
        }
    }
    void StopForceField()
    {
        if(_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.zero;
            _sphereCollider.enabled = true;

            /*
            if (forceFields.Count > maxInstances)
            {
                Destroy(forceFields[0]);
                forceFields.Remove(forceFields[0]);
            }
            */
        }
    }

    IEnumerator MaxDisplacement()
    {
        yield return new WaitForSeconds(cooldown);
        StopForceField();
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }
}