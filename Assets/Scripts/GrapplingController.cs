using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingController : MonoBehaviour
{
    private LineRenderer lr;

    private Vector3 grapplePoint;
    private Vector3 currentGrapplePosition;

    private SpringJoint joint;

    public LayerMask whatIsGrappleable;

    public new Transform camera;
    public Transform gunTip, player;

    public bool isGrappled;
    public bool isAvalible = true;

    [SerializeField] private float CooldownDuration = 1.0f;

    [Header("Rope Physics")]
    [SerializeField] private float spring = 4.5f;
    [SerializeField] private float damper = 7f;
    [SerializeField] private float massScale = 4.5f;

    [Header("Rope Distances")]
    [SerializeField] private float maxStrech = 4.5f;
    [SerializeField] private float minStrech = 4.5f;
    [SerializeField] private float maxDistance = 10f;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isAvalible)
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
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
            grapplePoint = hit.point;
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

    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);

        isGrappled = false;
    }


    void DrawRope()
    {
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
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
        isAvalible = false;
        yield return new WaitForSeconds(CooldownDuration);
        isAvalible = true;
    }

}
