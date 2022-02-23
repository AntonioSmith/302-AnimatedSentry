using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    public float visionDistance = 10;

    private List<TargetableObject> validTargets = new List<TargetableObject>();

    public TargetableObject target { get; private set; }

    private float cooldownScan = 0;
    private float cooldownPickTarget = 0;


    public bool playerWantsToAim { get; private set; }

    void Update()
    {
        playerWantsToAim = Input.GetButton("Fire2");

        cooldownScan -= Time.deltaTime;
        cooldownPickTarget -= Time.deltaTime;

        if (playerWantsToAim)
        {
            if (cooldownScan <= 0) ScanForTargets();
            if (cooldownPickTarget <= 0) PickTarget();
        } else
        {
            target = null;
        }

        print(target);
    }

    void ScanForTargets()
    {
        cooldownScan = .5f;

        validTargets.Clear();

        TargetableObject[] things = GameObject.FindObjectsOfType<TargetableObject>();
        foreach(TargetableObject thing in things)
        {
            Vector3 vToThing = thing.transform.position - transform.position;

            // Is close enough to see
            if(vToThing.sqrMagnitude < visionDistance * visionDistance)
            {
                float alignment = Vector3.Dot(transform.forward, vToThing.normalized);

                // Within 180 degrees of forward
                if(alignment > .4f)
                {
                    validTargets.Add(thing);
                }

            }
        }
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
