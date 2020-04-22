using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    [SerializeField] private Animator animator = null;
    [SerializeField] private SpriteRenderer spriteRender = null;
    [SerializeField] private CharController cCont = null;
    [SerializeField] private float groundedRotationSmoother = 20f;

    private float flipPoint = 0;
    private float vel;
    internal bool spriteFacingRight;

    // Start is called before the first frame update
    void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = transform.parent.position; //this should come from the top level component
        
        if (cCont.state == CharController.CharacterState.FALLING)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.parent.transform.rotation, 500f * Time.deltaTime);
        }
        else 
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, cCont.transform.rotation, groundedRotationSmoother * Time.deltaTime);
        }
        
        if (cCont.isShooting)
        {
            animator.SetBool("Shoot", true);
        } else
        {
            animator.SetBool("Shoot", false);
        }
        
        if (cCont.vel.x>0.001f)
        {
            if (flipPoint>0.9) spriteRender.flipX = true;
            flipPoint = Mathf.SmoothDamp(flipPoint, 1, ref vel, 0.1f );
            animator.SetBool("Walking", true);
            animator.SetBool("Idle", false);
            spriteFacingRight = true;

        }
        else if (cCont.vel.x<-0.001f)
        {
            if (flipPoint<-0.9) spriteRender.flipX = false;
            flipPoint = Mathf.SmoothDamp(flipPoint, -1, ref vel, 0.1f );
            animator.SetBool("Walking", true);
            animator.SetBool("Idle", false);
            spriteFacingRight = false;

        } 
        else
        {
            animator.SetBool("Walking", false);
            animator.SetBool("Idle", true);
        }
    }
}
