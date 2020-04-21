using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftOperator : MonoBehaviour
{
    [SerializeField] private Transform max = null;
    [SerializeField] private Transform min = null;
    // Start is called before the first frame update

    private bool isStatic = true;
    private bool goingUp = false;
    private Vector3 target;
    private GameObject player;
    void Start()
    {
        target = min.transform.position;
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 disp = player.transform.position - transform.position;
        Debug.Log(disp.magnitude);
        
        if (!isStatic && goingUp && transform.position.y >= target.y)
        {
            isStatic = true;
        }
        if (!isStatic && !goingUp && transform.position.y <= target.y )
        {
            isStatic = true;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            int playerLayer = LayerMask.NameToLayer("Player");
            int tilesLayer = LayerMask.NameToLayer("Tiles");
            int backgroundLayer = LayerMask.NameToLayer("Background");
            int playerLayerMask = 1 << playerLayer;
            int tilesLayerMask = 1 << tilesLayer;
            int backgroundLayerMask = 1 << backgroundLayer;

            int ignoreLayer = playerLayerMask | tilesLayerMask | backgroundLayerMask;

            // This would cast rays only against colliders in the two layers mentioned
            // But instead we want to collide against everything except these two layers. The ~ operator does this, it inverts a bitmask.
            ignoreLayer = ~ignoreLayer;
                
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 5, ignoreLayer);
            if (hit && isStatic)
            {
               
                
                if (goingUp) target = min.position;
                if (!goingUp) target = max.position;
                goingUp = !goingUp;
                isStatic = false;
            }
            
        }
        
        if (!isStatic) transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, target.y, 0), 1 * Time.deltaTime);
    }
}