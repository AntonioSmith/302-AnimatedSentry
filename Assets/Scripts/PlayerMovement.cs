using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    CharacterController pawn;

    public Camera cam;

    public float walkSpeed = 5;

    [Range(-10, -1)]
    public float gravity = -1;

    public Transform boneLegLeft;
    public Transform boneLegRight;

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
    }

    void Update()
    {
        if (cooldownJumpWindow > 0) cooldownJumpWindow -= Time.deltaTime;

        float v = Input.GetAxisRaw("Vertical"); // The Raw part bypasses project settings, using getaxis would basically add easing
        float h = Input.GetAxisRaw("Horizontal");

        bool playerWantsToMove = (v != 0 || h != 0);

        // Turn to look at camera if player moving 
        if (cam && playerWantsToMove)
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
        if (pawn.isGrounded) cooldownJumpWindow = .5f;

        WalkAnimation();
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
    }
}
