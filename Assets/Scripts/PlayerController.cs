using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharController cControl;

    private float xMovVel = 0.2f;
    private float yMovVel = 2.5f;
    private float moveMult = 0.07f;
    // Start is called before the first frame update
    void Start()
    {
        cControl = GetComponent<CharController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow) & cControl.state == CharController.CharacterState.GROUNDED)
        {
            cControl.vel.x += xMovVel * moveMult;
        }
        if (Input.GetKey(KeyCode.LeftArrow) & cControl.state == CharController.CharacterState.GROUNDED)
        {
            cControl.vel.x -= xMovVel * moveMult;
        }
        if (Input.GetKey(KeyCode.Space) & cControl.state == CharController.CharacterState.GROUNDED)
        {
            if (cControl.state == CharController.CharacterState.GROUNDED)
            {
                cControl.state = CharController.CharacterState.JUMPING;
                cControl.vel.y += yMovVel * moveMult;
            }
        }
    }
}
