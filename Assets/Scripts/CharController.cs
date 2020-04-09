using System;
using System.Collections.Generic;
using UnityEngine;
using SpriteFlick.Util;
using Unity.Mathematics;

public class CharController : MonoBehaviour
{
    private BoxCollider2D playerBox;
    private float gravity_modifier = 0.05f;
    internal float velX = 0;
    internal float velY = 0;
    private float skinOffset = 0.05f, minFallClamp = -0.1f, maxFallClamp = 0.1f, frictionX = 5f,  frictionY = 0;

    public enum CharacterState
    {
        GROUNDED, JUMPING, FALLING
    }

    public CharacterState state;
    
    
    void Start()
    {
        playerBox = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        List<Vector3> boxCorners = GetBoxCorners(playerBox);
        Vector3 minXPos = boxCorners[0];
        Vector3 maxXPos = boxCorners[2];


        
        DebugUtil.DrawMarker(minXPos, Color.cyan);
        DebugUtil.DrawMarker(maxXPos, Color.cyan);
        Vector3 playerBottomLineMidPt = (minXPos + maxXPos) / 2;
        //
        // DebugUtil.DrawMarker(playerBottomLineMidPt, Color.cyan);
        Vector3 playerBottomLine = maxXPos - minXPos;
        // Debug.DrawRay(new Vector3(minXPos.x, minXPos.y, 0), new Vector3(baseLine.x*2, baseLine.y*2, 0), Color.green);

        Vector3 perpendicularToPlayerBottomLine = new Vector2(playerBottomLine.y, -playerBottomLine.x).normalized * skinOffset;
        
        //CREATE RAYS ALONG BOTTOM EDGE
        List<Ray2D> bottomPlayerRay2Ds = new List<Ray2D>();
        float rayCastSpacing = playerBottomLine.sqrMagnitude / 3;
        var startOfRay = Vector3.zero;
        for (float raySpacer = 0; ;raySpacer += rayCastSpacing)
        {
                //eq of start line is :: minXPos + scale * vector bottom line
                startOfRay = new Vector3(minXPos.x + (raySpacer * playerBottomLine.x), minXPos.y + (raySpacer * playerBottomLine.y), playerBottomLine.z);
                if (startOfRay.x > maxXPos.x) break;
                Ray2D r2d = new Ray2D(startOfRay, new Vector3(perpendicularToPlayerBottomLine.x, perpendicularToPlayerBottomLine.y, 0));
                bottomPlayerRay2Ds.Add(r2d);
        }

        float rayCastLength = 0.1f;
        foreach (var rayCast in bottomPlayerRay2Ds)
        {
            Debug.DrawRay(rayCast.origin, rayCast.direction * rayCastLength, Color.cyan);
        }
        
        //

        //CHANGE THIS SO IT CHECKS ALL RAYS, WHICH SHOULD BE FIRST TO HIT? 
        // Debug.DrawRay(new Vector3(playerBottomLineMidPt.x, playerBottomLineMidPt.y, 0), new Vector3(perpendicularToPlayerBottomLine.x*2, perpendicularToPlayerBottomLine.y*2, 0), Color.magenta);
        
        var ignoreLayer = GetIgnoreLayer();

        RaycastHit2D rch = Physics2D.Raycast(playerBottomLineMidPt, perpendicularToPlayerBottomLine, skinOffset, ignoreLayer);
        
        if (!rch)
        {
            state = CharacterState.FALLING; //no collision set state to falling
            currentGroundSlope = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            velY -= gravity_modifier * 9.81f * Time.smoothDeltaTime;
        }
        else
        {
            if (state != CharacterState.GROUNDED)
            {
                Debug.Log("Hit collider " + rch.collider.name);
                SetGroundSlopeRotation(rch, playerBottomLine);
                velY = 0;
            }

            state = CharacterState.GROUNDED;
            velX = Mathf.SmoothDamp(velX, 0, ref vel, frictionX * Time.deltaTime);
        }
        
        transform.rotation = Quaternion.Lerp(transform.rotation, currentGroundSlope, 0.1f);
        velY = Mathf.Clamp(velY, minFallClamp, max:maxFallClamp);
        
        transform.position = new Vector3(transform.position.x + velX, transform.position.y + velY, 0);
    }

    private void SetGroundSlopeRotation(RaycastHit2D rch, Vector3 playerBottomLine)
    {
        // List<Vector3> platCorns = GetBoxCorners((BoxCollider2D) rch.collider);
        // Vector3 platformTopCorn = platCorns[1];
        // DebugUtil.DrawMarker(platformTopCorn, Color.cyan);
        Vector3 platformTop = new Vector3(rch.normal.y, -rch.normal.x); //needs to be perp in 3D
        // Debug.DrawRay(platformTopCorn, new Vector3(platformTop.x*2, platformTop.y*2), Color.magenta);
        // Debug.DrawRay(new Vector3(playerBottomLineMidPt.x, playerBottomLineMidPt.y, 0), new Vector3(playerBottomLine.x, playerBottomLine.y, 0), Color.blue);

         float angleBetweenPlayerAndPlatform = Vector3.SignedAngle(playerBottomLine, platformTop, Vector3.forward);
        // Debug.Log("Before player adjust " + angleBetweenPlayerAndPlatform);
        angleBetweenPlayerAndPlatform = angleBetweenPlayerAndPlatform + transform.rotation.eulerAngles.z; //don't consider existing 'player' rotation Z rotation
        // Debug.Log("After player adjust " + angleBetweenPlayerAndPlatform);
        currentGroundSlope = Quaternion.Euler(new Vector3(0, 0, angleBetweenPlayerAndPlatform));
    }


    private Quaternion currentGroundSlope;
    private float vel;

    private int GetIgnoreLayer()
    {
        int uiLayer = LayerMask.NameToLayer("UI");
        int playerLayer = LayerMask.NameToLayer("Player");
        int playerLayerMask = 1 << playerLayer;
        int uiLayerMask = 1 << uiLayer;

        int ignoreLayer = uiLayerMask | playerLayerMask;

        // This would cast rays only against colliders in the two layers mentioned
        // But instead we want to collide against everything except these two layers. The ~ operator does this, it inverts a bitmask.
        ignoreLayer = ~ignoreLayer;
        return ignoreLayer;
    }

    private List<Vector3> GetBoxCorners(BoxCollider2D boxColl) {
 
         Transform bcTransform = boxColl.transform;
 
         // The collider's centre point in the world
         Vector3 worldPosition = bcTransform.TransformPoint(0, 0, 0);
 
         // The collider's local width and height, accounting for scale, divided by 2
         Vector2 size = new Vector2(boxColl.size.x * bcTransform.localScale.x * 0.5f, boxColl.size.y * bcTransform.localScale.y * 0.5f);
 
         // STEP 1: FIND LOCAL, UN-ROTATED CORNERS
         // Find the 4 corners of the BoxCollider2D in LOCAL space, if the BoxCollider2D had never been rotated
         Vector3 corner1 = new Vector2(-size.x, -size.y);
         Vector3 corner2 = new Vector2(-size.x, size.y);
         Vector3 corner3 = new Vector2(size.x, -size.y);
         Vector3 corner4 = new Vector2(size.x, size.y);
 
         // STEP 2: ROTATE CORNERS
         // Rotate those 4 corners around the centre of the collider to match its transform.rotation
         corner1 = RotatePointAroundPivot(corner1, Vector3.zero, bcTransform.eulerAngles);
         corner2 = RotatePointAroundPivot(corner2, Vector3.zero, bcTransform.eulerAngles);
         corner3 = RotatePointAroundPivot(corner3, Vector3.zero, bcTransform.eulerAngles);
         corner4 = RotatePointAroundPivot(corner4, Vector3.zero, bcTransform.eulerAngles);
 
         // STEP 3: FIND WORLD POSITION OF CORNERS
         // Add the 4 rotated corners above to our centre position in WORLD space - and we're done!
         corner1 = worldPosition + corner1;
         corner2 = worldPosition + corner2;
         corner3 = worldPosition + corner3;
         corner4 = worldPosition + corner4;
         List<Vector3> vertList = new List<Vector3>();
         vertList.Add(corner1);
         vertList.Add(corner2);
         vertList.Add(corner3);
         vertList.Add(corner4);
         return vertList;
     }

     Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
         Vector3 dir = point - pivot; // get point direction relative to pivot
         dir = Quaternion.Euler(angles) * dir; // rotate it
         point = dir + pivot; // calculate rotated point
         return point; // return it
     }
     
}
