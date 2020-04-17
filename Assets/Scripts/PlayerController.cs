using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharController cControl;

    private float xMovVel = 0.5f;
    private float yMovVel = 35f;
    // Start is called before the first frame update
    void Start()
    {
        cControl = GetComponent<CharController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log( 4.9 * Time.deltaTime * xMovVel);
        // Debug.Log("---" + moveConst * xMovVel);
        
        
        if (Input.GetKey(KeyCode.RightArrow) & cControl.state == CharController.CharacterState.GROUNDED)
        {
            cControl.vel.x += xMovVel * Time.deltaTime; //must have dt - as will vary
        }
        if (Input.GetKey(KeyCode.LeftArrow) & cControl.state == CharController.CharacterState.GROUNDED)
        {
            cControl.vel.x -= xMovVel * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space) & cControl.state == CharController.CharacterState.GROUNDED)
        {
            if (cControl.state == CharController.CharacterState.GROUNDED)
            {
                cControl.state = CharController.CharacterState.JUMPING;
                cControl.vel.y += yMovVel * Time.deltaTime;
            }
        }
    }
}
