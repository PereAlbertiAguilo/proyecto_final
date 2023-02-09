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

    public Transform cam;
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
        //Get the gameobject components and stores them in variables
        lr = GetComponent<LineRenderer>();
        _armMat = GetComponent<MeshRenderer>().material;
        _armAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        //Get scripts from scene and store them in variables
        playerControllerScript = player.GetComponent<PlayerController>();

        //Stops the grapple
        StopGrapple();
    }

    void Update()
    {
        //Stores all the gameobjects with a certain tag to an array
        grappables = GameObject.FindGameObjectsWithTag("Grapable");

        //if the player is close enough to one of the stored grappables it can start a grapple
        foreach(GameObject g in grappables)
        {
            float dist = Vector3.Distance(player.position, g.transform.position);

            if (dist <= maxDistance)
            {
                //Starts a gprapple
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.JoystickButton4))
                {
                    if (isGrappleAvalible)
                    {
                        StartGrapple(g.transform);
                    }
                }
            }
        }

        //Stops the grapple
        if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.JoystickButton4))
        {
            if (isGrappled)
            {
                StopGrapple();
            }
        }

        //If is grappled even if the grapple point pos changes the gprappling still attatches and follows it
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

    //Adds a joint as a component to the player with a start point same as the player and the end point same as the given transform with some determinated physic parameters given with variables
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

    //Stops the grapple and destroys the joint
    public void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);

        isGrappled = false;
    }

    //Draws the grapple rope with a line renderer with the same start and end of the joint if they exist
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

    //Start a timer that limits the time bettween grapples
    public IEnumerator StartCooldown()
    {
        isGrappleAvalible = false;
        yield return new WaitForSeconds(CooldownDuration);
        isGrappleAvalible = true;
    }
}
