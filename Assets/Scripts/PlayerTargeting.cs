using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    public float visionDistance = 10;
    [Range(1, 20)]
    public float roundsPerSecond = 5;

    public PointAt boneShoulderRight;
    public PointAt boneShoulderLeft;

    private List<TargetableObject> validTargets = new List<TargetableObject>();

    public TargetableObject target { get; private set; }

    private float cooldownScan = 0;
    private float cooldownPickTarget = 0;
    private float cooldownAttack = 0;

    private CameraController cam;

    public bool playerWantsToAim { get; private set; }
    public bool playerWantsToAttack { get; private set; }

    private void Start()
    {
        cam = FindObjectOfType<CameraController>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        playerWantsToAim = Input.GetButton("Fire2");
        playerWantsToAttack = Input.GetButton("Fire1");

        cooldownScan -= Time.deltaTime;
        cooldownPickTarget -= Time.deltaTime;
        cooldownAttack -= Time.deltaTime;

        if (playerWantsToAim)
        {
            if(target != null)
            {
                // Turn towards target
                Vector3 toTarget = target.transform.position - transform.position;
                toTarget.y = 0;

                if (toTarget.magnitude > 2 && !CanSeeThing(target))
                {
                    target = null;
                }
            }

            if (cooldownScan <= 0) ScanForTargets();
            if (cooldownPickTarget <= 0) PickTarget();
        } else
        {
            target = null;
        }

        print(target);
        if (boneShoulderLeft) boneShoulderLeft.target = target ? target.transform : null;
        if (boneShoulderRight) boneShoulderRight.target = target ? target.transform : null;

        DoAttack();
    }

    void DoAttack()
    {
        if (cooldownAttack > 0) return;
        if (!playerWantsToAim) return;
        if (!playerWantsToAttack) return;
        if (target == null) return;
        if (!CanSeeThing(target)) return;

        cooldownAttack = 1f / roundsPerSecond;

        // TODO: Do an attack

        boneShoulderLeft.transform.localEulerAngles += new Vector3(-30, 0, 0);
        boneShoulderRight.transform.localEulerAngles += new Vector3(-30, 0, 0);

        if(cam) cam.Shake(.25f);
    }

    void ScanForTargets()
    {
        cooldownScan = .5f;

        validTargets.Clear();

        TargetableObject[] things = GameObject.FindObjectsOfType<TargetableObject>();
        foreach(TargetableObject thing in things)
        {
            if (CanSeeThing(thing))
            {
                validTargets.Add(thing);
            }            
        }
    }

    private bool CanSeeThing(TargetableObject thing)
    {
        Vector3 vToThing = thing.transform.position - transform.position;

        // Is too far to see
        if (vToThing.sqrMagnitude > visionDistance * visionDistance) return false;
        // how much is in front of player
        float alignment = Vector3.Dot(transform.forward, vToThing.normalized);

        // If not within 180 degrees of forward
        if (alignment < .4f) return false;

        // Check for occlusion
        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = vToThing;

        Debug.DrawRay(ray.origin, ray.direction, Color.red);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, visionDistance))
        {
            bool canSee = false;
            Transform xform = hit.transform;

            do
            {
                if (xform.gameObject == thing.gameObject)
                {
                    canSee = true;
                    break;
                }
                xform = xform.parent;
            } while (xform != null);

            if (!canSee) return false;
        }

        return true;
    }

    void PickTarget()
    {
        if (target) return;

        float closestDistanceSoFar = visionDistance;

        foreach(TargetableObject thing in validTargets)
        {
            Vector3 vToThing = thing.transform.position - transform.position;

            float dis = vToThing.sqrMagnitude;

            if(dis < closestDistanceSoFar || target == null)
            {
                closestDistanceSoFar = dis;
                target = thing;
            }
        }
    }
}
