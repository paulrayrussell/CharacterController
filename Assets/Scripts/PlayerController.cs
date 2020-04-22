using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharController cCont;
    [SerializeField] private PlayerAnimationController animationCont = null;
    [SerializeField] private ArmsController armsCont = null;
    [SerializeField] private ArmsAnimationController armsAnimationCont = null;
    [SerializeField] private Transform firept = null;
    
    private float xMovVel = 0.5f;

    private float yMovVel = 35f;
    [SerializeField] private GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        cCont = GetComponent<CharController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log( 4.9 * Time.deltaTime * xMovVel);
        // Debug.Log("---" + moveConst * xMovVel);


        if (Input.GetKey(KeyCode.RightArrow) & cCont.state == CharController.CharacterState.GROUNDED)
        {
            cCont.vel.x += xMovVel * Time.deltaTime; //must have dt - as will vary
        }

        if (Input.GetKey(KeyCode.LeftArrow) & cCont.state == CharController.CharacterState.GROUNDED)
        {
            cCont.vel.x -= xMovVel * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Space) & cCont.state == CharController.CharacterState.GROUNDED)
        {
            if (cCont.state == CharController.CharacterState.GROUNDED)
            {
                cCont.state = CharController.CharacterState.JUMPING;
                cCont.vel.y += yMovVel * Time.deltaTime;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (cCont.state != CharController.CharacterState.GROUNDED) return;

            Vector3 mouseClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 shootDir = mouseClickPos - firept.position;

            if (shootDir.x > 0 && !animationCont.spriteFacingRight) return;
            if (shootDir.x < 0 && animationCont.spriteFacingRight) return;
            armsAnimationCont.GetComponent<SpriteRenderer>().enabled = true;
            armsCont.clickpt = mouseClickPos;
            StartCoroutine(FirePause(shootDir));

          
            int enLayer = LayerMask.NameToLayer("Enemies");
            int enBitMask = 1 << enLayer;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 25, enBitMask);

            if (hit)
            {
             
            }
        }
    }

    private IEnumerator FirePause(Vector3 shootDir)
    {
        cCont.isShooting = true;
        yield return new WaitForSeconds(0.5f);
        GameObject b = Instantiate(bullet, firept.position, armsCont.transform.rotation);
        b.GetComponent<PowerEnemyBullet>().Setup(shootDir, 0.005f);
        yield return new WaitForSeconds(0.5f);
        cCont.isShooting = false;
        armsAnimationCont.GetComponent<SpriteRenderer>().enabled = false;
    }
}