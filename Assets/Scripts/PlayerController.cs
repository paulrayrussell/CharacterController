using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharController cControl;

    private float xMovVel = 0.5f;
    private float yMovVel = 3f;
    private float moveMult = 5f;
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
            cControl.vel.x += xMovVel * moveMult * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow) & cControl.state == CharController.CharacterState.GROUNDED)
        {
            cControl.vel.x -= xMovVel * moveMult * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space) & cControl.state == CharController.CharacterState.GROUNDED)
        {
            if (cControl.state == CharController.CharacterState.GROUNDED)
            {
                cControl.state = CharController.CharacterState.JUMPING;
                cControl.vel.y += yMovVel * moveMult * Time.deltaTime;
            }
        }
    }
}
