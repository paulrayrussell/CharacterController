using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private CharController cControl;
    private GameObject player;
    private CollisionDetected cd;
    public bool isDead;
    public EventHandler stateChange;
    private float xMovVel = 0.2f;
    // private float yMovVel = 35f;
    [SerializeField] private AudioSource grunt;

    private float vel;
    // Start is called before the first frame update
    void Start()
    {
        cd = GetComponentInChildren<CollisionDetected>();
        cControl = GetComponent<CharController>();
        player = GameObject.FindWithTag("Player");
        cd.collisionEnter += Collision;
    }

    private void Collision(object sender, EventArgs e)
    {
        isDead = true;
        cControl.vel.x = 0;
        stateChange?.Invoke(this, new EventArgs());
        grunt.PlayOneShot(grunt.clip);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log( 4.9 * Time.deltaTime * xMovVel);
        // Debug.Log("---" + moveConst * xMovVel);
        if (isDead) return;
        
        float disp = (player.transform.position - transform.position).magnitude;
        if (disp<2)
        {
            vel = 0;
        }
        else {
            if (player.transform.position.x > transform.position.x) vel = xMovVel; else vel = -xMovVel;
        }
        
        cControl.vel.x += vel * Time.deltaTime; //must have dt - as will vary
    }


}
