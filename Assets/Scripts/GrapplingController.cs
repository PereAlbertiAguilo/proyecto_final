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

    public LayerMask whatIsGrappleable;

    public new Transform camera;
    public Transform gunTip, player;

    [HideInInspector] public bool isGrappled;
    [HideInInspector] public bool isGrappleAvalible = true;

    [SerializeField] private float CooldownDuration = 1.0f;

    [Header("Rope Physics\n")]
    [SerializeField] private float spring = 4.5f;
    [SerializeField] private float damper = 7f;
    [SerializeField] private float massScale = 4.5f;

    [Header("Rope Distances\n")]
    [SerializeField] private float maxStrech = 4.5f;
    [SerializeField] private float minStrech = 4.5f;
    [SerializeField] private float maxDistance = 10f;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isGrappleAvalible)
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        if (isGrappled)
        {
            if(target != null)
            {
                joint.connectedAnchor = target.transform.position;
            }
            else
            {
                StopGrapple();
            }
        }
    }

    void LateUpdate()
    {
        DrawRope();
    }

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

            isGrappled = true;

            StartCoroutine(StartCooldown());
        }
    }

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
