using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    CharacterController pawn;

    public Camera cam;

    public float walkSpeed = 5;

    void Start()
    {
        pawn = GetComponent<CharacterController>();
    }

    void Update()
    {
        float v = Input.GetAxisRaw("Vertical"); // The Raw part bypasses project settings, using getaxis would basically add easing
        float h = Input.GetAxisRaw("Horizontal");

        bool playerWantsToMove = (v != 0 || h != 0);

        // Turn to look at camera if player moving 
        if (cam && playerWantsToMove)
        {
            float playerYaw = cam.transform.eulerAngles.y;
            float camYaw = cam.transform.eulerAngles.y;

            //while (camYaw > playerYaw + 180) camYaw -= 360;
            //while (camYaw < playerYaw - 180) camYaw += 360;

            Quaternion targetRotation = Quaternion.Euler(0, playerYaw, 0);
            transform.rotation = AnimMath.Ease(transform.rotation, targetRotation, .01f);
        }

        Vector3 moveDir = transform.forward * v + transform.right * h; // move left and right
        if (moveDir.sqrMagnitude > 1) moveDir.Normalize(); // Clamp movement to prevent diagonal movement from being faster

        pawn.SimpleMove(moveDir * walkSpeed);
    }
}
