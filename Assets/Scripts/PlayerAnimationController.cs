using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private CharController charController;

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

        transform.position = charController.transform.position;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, charController.currentGroundSlopeRotation, 40f * Time.deltaTime);
        
        if (charController.vel.x>0.001f)
        {
            if (flipPoint>0.9) spriteRender.flipX = true;
            flipPoint = Mathf.SmoothDamp(flipPoint, 1, ref vel, 0.1f );

        }
        if (charController.vel.x<-0.001f)
        {
            if (flipPoint<-0.9) spriteRender.flipX = false;
            flipPoint = Mathf.SmoothDamp(flipPoint, -1, ref vel, 0.1f );
        }
    }
}
