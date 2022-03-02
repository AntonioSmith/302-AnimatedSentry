using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAt : MonoBehaviour
{
    private PlayerTargeting playerTargeting;

    private Quaternion startRotation;
    private Quaternion goalRotation;

    public bool lockAxisX = false;
    public bool lockAxisY = false;
    public bool lockAxisZ = false;

    void Start()
    {
        playerTargeting = GetComponentInParent<PlayerTargeting>();
        startRotation = transform.localRotation;
    }

    void Update()
    {
        TurnTowardsTarget();
    }

    private void TurnTowardsTarget()
    {
        if(playerTargeting && playerTargeting.target && playerTargeting.playerWantsToAim)
        {
            Vector3 vToTarget = playerTargeting.target.transform.position - transform.position;
            vToTarget.Normalize();

            Quaternion worldRot = Quaternion.LookRotation(vToTarget, Vector3.up);
            Quaternion prevRot = transform.rotation;
            Vector3 eulerBefore = transform.localEulerAngles;
            transform.rotation = worldRot;
            Vector3 eulerAfter = transform.localEulerAngles;
            transform.rotation = prevRot;

            /*
            Quaternion localRot = worldRot;
            if (transform.parent)
            {
                localRot = Quaternion.Inverse(transform.parent.rotation) * worldRot;
            }
            */

            if (lockAxisX) eulerAfter.x = eulerBefore.x;
            if (lockAxisY) eulerAfter.y = eulerBefore.y;
            if (lockAxisZ) eulerAfter.z = eulerBefore.z;

            goalRotation = Quaternion.Euler(eulerAfter);
        }
        else
        {
            goalRotation = startRotation;
        }

        transform.localRotation = AnimMath.Ease(transform.localRotation, goalRotation, .001f);

    }
}
