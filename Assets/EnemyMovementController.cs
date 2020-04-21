using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    private CharController cControl;
    private GameObject player;

    private float xMovVel = 0.4f;
    private float yMovVel = 35f;

    private float vel;
    // Start is called before the first frame update
    void Start()
    {
        cControl = GetComponent<CharController>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log( 4.9 * Time.deltaTime * xMovVel);
        // Debug.Log("---" + moveConst * xMovVel);
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
