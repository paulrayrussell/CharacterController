using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    [SerializeField] private Animator animator = null;
    [SerializeField] private SpriteRenderer spriteRender = null;
    [SerializeField] private CharController charController = null;
    [SerializeField] private float groundedRotationSmoother = 20f;

    private float flipPoint = 0;
    private float vel;

    // Start is called before the first frame update
    void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = transform.parent.position; //this should come from the top level component
        
        if (charController.state == CharController.CharacterState.FALLING)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.parent.transform.rotation, 500f * Time.deltaTime);
        }
        else 
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, charController.transform.rotation, groundedRotationSmoother * Time.deltaTime);
        }
        
        if (charController.vel.x>0.01f)
        {
            if (flipPoint>0.9) spriteRender.flipX = true;
            flipPoint = Mathf.SmoothDamp(flipPoint, 1, ref vel, 0.1f );
            animator.SetBool("Walking", true);
            animator.SetBool("Idle", false);

        }
        else if (charController.vel.x<-0.01f)
        {
            if (flipPoint<-0.9) spriteRender.flipX = false;
            flipPoint = Mathf.SmoothDamp(flipPoint, -1, ref vel, 0.1f );
            animator.SetBool("Walking", true);
            animator.SetBool("Idle", false);
        } 
        else
        {
            animator.SetBool("Walking", false);
            animator.SetBool("Idle", true);
        }
    }
}
