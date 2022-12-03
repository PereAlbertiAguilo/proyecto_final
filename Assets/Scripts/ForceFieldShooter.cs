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
    [SerializeField] private float lifeTime = 10f;

    public int maxInstances;
    [HideInInspector] public int currentInstance;

    private int mode;

    [SerializeField] public Transform shootPoint, cam;

    [Header("Instances\n")]
    [SerializeField] GameObject forceField;
    [SerializeField] GameObject dobleJump;
    [SerializeField] GameObject destroyParticle;
    [SerializeField] private GameObject[] instance;

    private Rigidbody _rigidbody;

    private SphereCollider _sphereCollider;

    private PlayerController playerController;

    [Header("Others\n")]
    [SerializeField] private float clickDist;
    [SerializeField] private Animator _animator;
    [SerializeField] private new Transform camera;

    private List<GameObject> forceFields = new List<GameObject>();

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        mode = 0;
        currentInstance = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mode = 0;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            mode = 1;
        }
        if(instance[1] == null && currentInstance > 0)
        {
            currentInstance = forceFields.Count;
        }

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
            else if(mode == 1)
            {
                instance[mode] = Instantiate(forceField, shootPoint.position, cam.transform.rotation);
                StartCoroutine(DestroyOverLifeTime(mode));
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
                if (playerController.isGrounded && forceFields.Count > 0)
                {
                    _animator.Play("arm_shoot");

                    for (int i = 0; i <= currentInstance + 1; i++)
                    {
                        if (forceFields.Count > 0)
                        {
                            canShoot = false;

                            currentInstance--;

                            if (forceFields[currentInstance] != null)
                            {
                                Instantiate(destroyParticle, forceFields[currentInstance].transform.position, Quaternion.identity);
                            }

                            Destroy(forceFields[currentInstance]);
                            forceFields.Remove(forceFields[currentInstance]);

                            yield return new WaitForSeconds(0.1f);
                        }
                    }

                    _animator.Play("arm_reload");
                    yield return new WaitForSeconds(0.4f);
                    canShoot = true;
                }
            }
        }
    }

    IEnumerator DestroyOverLifeTime(int i)
    {
        yield return new WaitForSeconds(lifeTime);
        if(instance[i] != null)
        {
            Destroy(instance[i]);
            Instantiate(destroyParticle, instance[i].transform.position, Quaternion.identity);
            currentInstance++;
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);

        StopForceField();

        if (currentInstance <= maxInstances)
        {
            canShoot = true;
        }
    }
}