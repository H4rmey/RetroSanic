using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private Player      player;
    private Animator    animator;

    private int faceDirection, faceDirectionOld;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        player.input.pHorizontal = 1;
    }

    // Update is called once per frame
    void Update()
    {
        int direction = player.input.pHorizontal;

        if (player.input.pHorizontal == 0)
            direction = faceDirectionOld;
        player.transform.localScale = new Vector3(direction, 1, 1);

        animator.SetBool("IsWallSliding",   player.state == Player.STATE.WALLSLIDING);
        animator.SetBool("IsAirBorne",      (player.state == Player.STATE.AIRBORNE && (player.velocity.y < 0)));
        animator.SetBool("IsGrounded",      player.controller.collisions.below);
        animator.SetBool("IsRunning",       (player.input.pLeft || player.input.pRight));


        faceDirectionOld = direction;
    }

    public void PlayAnimation(string animationTriggerName)
    {
        animator.SetTrigger(animationTriggerName);
    }
}
