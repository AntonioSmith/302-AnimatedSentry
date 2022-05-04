using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    CharacterController pawn;
    PlayerTargeting targetingScript;

    public Camera cam;

    public float walkSpeed = 5;

    [Range(-10, -1)]
    public float gravity = -1;

    public Transform boneShoulderLeft;
    public Transform boneShoulderRight;
    public Transform boneLegLeft;
    public Transform boneLegRight;
    public Transform boneHip;
    public Transform boneSpine;

    public bool isGrounded
    {
        get { return pawn.isGrounded || cooldownJumpWindow > 0; }
    }

    private Vector3 inputDir;

    private float velocityVertical = 0;
    private float cooldownJumpWindow = 0;

    void Start()
    {
        pawn = GetComponent<CharacterController>();
        targetingScript = GetComponent<PlayerTargeting>();
    }

    void Update()
    {
        if (cooldownJumpWindow > 0) cooldownJumpWindow -= Time.deltaTime;

        float v = Input.GetAxisRaw("Vertical"); // The Raw part bypasses project settings, using getaxis would basically add easing
        float h = Input.GetAxisRaw("Horizontal");

        bool wantsToRun = Input.GetKey(KeyCode.LeftShift); // If holding shift, start sprinting

        if (wantsToRun) // If running, set walkspeed to 10, else set walkspeed to 5
        {
            walkSpeed = 10;
        }
        else
        {
            walkSpeed = 5;
        }

        bool playerWantsToMove = (v != 0 || h != 0);
        bool playerIsAiming = (targetingScript && targetingScript.playerWantsToAim && targetingScript.target);

        if (!playerWantsToMove)
        {
            IdleAnimation();
        }

        if (playerIsAiming)
        {
            Vector3 toTarget = targetingScript.target.transform.position - transform.position;
            toTarget.Normalize();

            Quaternion worldRot = Quaternion.LookRotation(toTarget);
            Vector3 euler = worldRot.eulerAngles;
            euler.x = 0;
            euler.z = 0;
            worldRot.eulerAngles = euler;

            transform.rotation = AnimMath.Ease(transform.rotation, worldRot, .01f);
        }
        else if(cam && playerWantsToMove) // Turn to look at camera if player moving 
        {
            float playerYaw = cam.transform.eulerAngles.y;
            float camYaw = cam.transform.eulerAngles.y;

            while (camYaw < playerYaw - 180) camYaw += 360;
            while (camYaw > playerYaw + 180) camYaw -= 360;

            Quaternion playerRotation = Quaternion.Euler(0, playerYaw, 0);
            Quaternion targetRotation = Quaternion.Euler(0, playerYaw, 0);

            transform.rotation = AnimMath.Ease(playerRotation, targetRotation, .01f);
        }

        inputDir = transform.forward * v + transform.right * h; // move left and right
        if (inputDir.sqrMagnitude > 1) inputDir.Normalize(); // Clamp movement to prevent diagonal movement from being faster

        bool wantsToJump = Input.GetButtonDown("Jump");
        if(isGrounded){
            velocityVertical = 0;
            if (wantsToJump)
            {
                cooldownJumpWindow = 0;
                velocityVertical = 5;
            }
        }

            velocityVertical += gravity * Time.deltaTime;

        Vector3 moveAmount = inputDir * walkSpeed + Vector3.up * velocityVertical;
        pawn.Move(moveAmount * Time.deltaTime);
        if (pawn.isGrounded)
        {
            cooldownJumpWindow = .5f;
            velocityVertical = 0;
            WalkAnimation();
        }
        else
        {
            AirAnimation();
        }
    }

    private void IdleAnimation()
    {
        float speed = 500f; // speed of arm movement
        float height = 200f; // how high the shoulders go

        // Animate arms to slowly move up and down
        Vector3 pos = boneShoulderLeft.transform.position; // set the shoulder bone to a variable
        float shoulderHeight = Mathf.Sin(Time.time * speed); // change shoulderHeight along a sin wave
        pos = new Vector3(pos.x, shoulderHeight, pos.z) * height; // move left shoulder along sin wave
        print("Idling");
    }

    private void AirAnimation()
    {
        
    }

    void WalkAnimation()
    {
        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDir);
        Vector3 axis = Vector3.Cross(Vector3.up, inputDirLocal);

        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);
        alignment = Mathf.Abs(alignment);

        float degrees = AnimMath.Lerp(10, 40, alignment);
        float speed = 10;
        float wave = Mathf.Sin(Time.time * speed) * degrees;

        boneLegLeft.localRotation = Quaternion.AngleAxis(wave, axis);
        boneLegRight.localRotation = Quaternion.AngleAxis(-wave, axis);

        if (boneHip)
        {
            float walkAmount = axis.magnitude;
            float offsetY = Mathf.Cos(Time.time * speed) * walkAmount * .05f;
            boneHip.localPosition = new Vector3(0, offsetY, 0);
        }
    }
}
