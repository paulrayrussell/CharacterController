﻿﻿using System;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController bodyAnimationCont = null;
    [SerializeField] private ArmsRotationController armsRotationCont = null;
    [SerializeField] private GameObject armsAnimatorGO = null;
    [SerializeField] private BulletObjectPooler bulletPooler = null;
    [SerializeField] private Transform leftFirept = null;
    [SerializeField] private float armOffset = 0.1f;
    [SerializeField] private float shootSpeed = 0.5f;
    [SerializeField] private AudioSource shotSound = null;

    // private CollisionDetected collisionDetected;
    private CharController charController;
    private float xMovVel = 0.5f;
    private float yMovVel = 35f;
    private SpriteRenderer srOuter, srInner;
    private bool firing;


    void Start()
    {
        charController = GetComponent<CharController>();
        SpriteRenderer[] sra = armsAnimatorGO.GetComponentsInChildren<SpriteRenderer>();
        srOuter = sra[0];
        srInner = sra[1];
        // collisionDetected = GetComponentInChildren<CollisionDetected>();
        // collisionDetected.collisionEnter += StateChange;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow) & charController.state == CharController.CharacterState.GROUNDED && !charController.isShooting)
        {
            charController.vel.x += xMovVel * Time.deltaTime; //must have dt - as will vary
        }

        if (Input.GetKey(KeyCode.LeftArrow) & charController.state == CharController.CharacterState.GROUNDED && !charController.isShooting)
        {
            charController.vel.x -= xMovVel * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Space) & charController.state == CharController.CharacterState.GROUNDED && !charController.isShooting)
        {
            if (charController.state == CharController.CharacterState.GROUNDED)
            {
                charController.state = CharController.CharacterState.JUMPING;
                charController.vel.y += yMovVel * Time.deltaTime;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (charController.isShooting) return;
            if (charController.state != CharController.CharacterState.GROUNDED) return;

            Vector3 mouseClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if ((new Vector3(mouseClickPos.x, mouseClickPos.y, transform.position.z) - transform.position).magnitude < 3f)
            {
                Debug.Log("Too close");
                // return;
            }
            
            Vector3 shootDir = (mouseClickPos - leftFirept.position);
            float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
            // Debug.Log(angle);
            if (shootDir.x > 0 && !bodyAnimationCont.spriteFacingRight) return;
            if (shootDir.x < 0 && bodyAnimationCont.spriteFacingRight) return;
            
            StartCoroutine(FirePause(shootDir.normalized, mouseClickPos));
            
            int enLayer = LayerMask.NameToLayer("Enemies");
            int enBitMask = 1 << enLayer;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 25, enBitMask);

            if (hit)
            {
             
            }
        }
    }

    private IEnumerator FirePause(Vector3 shootDir, Vector3 mouseClickPos)
    {
        charController.isShooting = true;




        armsAnimatorGO.transform.localScale = bodyAnimationCont.spriteFacingRight ? //flip the scale to mirror arms
            new Vector3( armsAnimatorGO.transform.localScale.x, -1, armsAnimatorGO.transform.localScale.z) : 
            new Vector3( armsAnimatorGO.transform.localScale.x, 1, armsAnimatorGO.transform.localScale.z);

        armsAnimatorGO.transform.position = bodyAnimationCont.spriteFacingRight ? //offset the arms for left or right
            new Vector3(armsAnimatorGO.transform.position.x + armOffset, armsAnimatorGO.transform.position.y, armsAnimatorGO.transform.position.z) :
            new Vector3(armsAnimatorGO.transform.position.x - armOffset, armsAnimatorGO.transform.position.y, armsAnimatorGO.transform.position.z);

        armsRotationCont.clickpt = mouseClickPos;

        srInner.enabled = true; //show the arms     
        srOuter.enabled = true;
        yield return new WaitForSeconds(0.1f);
        
        shotSound.PlayOneShot(shotSound.clip);
        BulletObjectPooler.BulletStruct bs = bulletPooler.GetPooledObject();
        bs.bulletPrefab.transform.rotation = armsRotationCont.transform.rotation;
        bs.bulletPrefab.transform.position = leftFirept.position;
        bs.bulletPrefab.SetActive(true);
        bs.bulletScript.Setup(shootDir, shootSpeed);

        yield return new WaitForSeconds(0.3f);

        //hide arms
        srInner.enabled = false;
        srOuter.enabled = false;

        //put arms back to original offset
        armsAnimatorGO.transform.position = bodyAnimationCont.spriteFacingRight
            ? new Vector3(armsAnimatorGO.transform.position.x - armOffset, armsAnimatorGO.transform.position.y, armsAnimatorGO.transform.position.z)
            : new Vector3(armsAnimatorGO.transform.position.x + armOffset, armsAnimatorGO.transform.position.y, armsAnimatorGO.transform.position.z);
        
        charController.isShooting = false;
    }
}