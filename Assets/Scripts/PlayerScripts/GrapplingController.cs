using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingController : MonoBehaviour
{
    private LineRenderer lr;

    private Vector3 grapplePoint;
    private Vector3 currentGrapplePosition;

    private GameObject target;

    private SpringJoint joint;

    [SerializeField] private LayerMask whatIsGrappleable;

    public new Transform camera;
    public Transform gunTip, player;

    [HideInInspector] public bool isGrappled;
    [HideInInspector] public bool isGrappleAvalible = true;

    [SerializeField] private float CooldownDuration = 1.0f;

    private Material _armMat;

    private Animator _armAnimator;

    [SerializeField] private float disolveFactor;

    [Header("Rope Physics\n")]
    [SerializeField] private float spring = 4.5f;
    [SerializeField] private float damper = 7f;
    [SerializeField] private float massScale = 4.5f;

    [Header("Rope Distances\n")]
    [SerializeField] private float maxStrech = 4.5f;
    [SerializeField] private float minStrech = 4.5f;
    [SerializeField] private float maxDistance = 10f;

    private PlayerController playerControllerScript;

    [Header("\n")]
    [SerializeField] private GameObject[] grappables;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        _armMat = GetComponent<MeshRenderer>().material;
        _armAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        playerControllerScript = player.GetComponent<PlayerController>();

        StopGrapple();
    }

    void Update()
    {
        grappables = GameObject.FindGameObjectsWithTag("Grapable");

        foreach(GameObject g in grappables)
        {
            float dist = Vector3.Distance(player.position, g.transform.position);

            if (dist <= maxDistance)
            {
                print("canGrapple");
                if (Input.GetMouseButtonDown(1) && isGrappleAvalible)
                {
                    StartGrapple(g.transform);
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    StopGrapple();
                }
            }
        }


        if (isGrappled)
        {
            _armAnimator.Play("arm_disolve0");

            if (target != null)
            {
                joint.connectedAnchor = target.transform.position;
            }
            else
            {
                StopGrapple();
            }
        }
        else
        {
            _armAnimator.Play("arm_disolve1");
        }
    }

    void LateUpdate()
    {
        DrawRope();

        _armMat.SetFloat("_DisolveFactor", disolveFactor);
    }

    void StartGrapple(Transform t)
    {
        target = t.gameObject;
        grapplePoint = t.position;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

        joint.maxDistance = distanceFromPoint * maxStrech;
        joint.minDistance = distanceFromPoint * minStrech;

        joint.spring = spring;
        joint.damper = damper;
        joint.massScale = massScale;

        lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;

        transform.parent.GetComponent<Animator>().Play("arm_exit");

        playerControllerScript.PlayerSFX(playerControllerScript.sfxs[1], 1, 1.5f);

        isGrappled = true;

        StartCoroutine(StartCooldown());
    }
    /*
    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            target = hit.collider.gameObject;
            grapplePoint = hit.transform.position;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * maxStrech;
            joint.minDistance = distanceFromPoint * minStrech;

            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = massScale;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;

            transform.parent.GetComponent<Animator>().Play("arm_exit");

            playerControllerScript.PlayerSFX(playerControllerScript.sfxs[1], 1, 1.5f);

            isGrappled = true;

            StartCoroutine(StartCooldown());
        }
    }
    */

    public void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);

        isGrappled = false;
    }


    void DrawRope()
    {
        if(target != null)
        {
            if (!joint)
            {
                return;
            }
            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, target.transform.position, Time.deltaTime * 8f);

            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, currentGrapplePosition);
        }
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    public IEnumerator StartCooldown()
    {
        isGrappleAvalible = false;
        yield return new WaitForSeconds(CooldownDuration);
        isGrappleAvalible = true;
    }
}
