using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using SpriteFlick.Util;
using Unity.Mathematics;

public class CharController : MonoBehaviour
{
    private BoxCollider2D playerBox;
    private BoxCollider2D playerBoxStrunk;
    private float gravity_modifier = 0.005f;
    internal float velX = 0;
    internal float velY = 0;
    private float minFallClamp = -0.1f, maxFallClamp = 0.1f, frictionX = 5f, frictionY = 0;
    float rayCastLength = 0.1f;

    private Quaternion currentGroundSlope;
    private float vel;
    Vector2 platformTop;
    int ignoreLayer;

    public enum CharacterState
    {
        GROUNDED,
        JUMPING,
        FALLING
    }

    public CharacterState state;


    void Start()
    {
        playerBox = GetComponent<BoxCollider2D>();
        playerBox.size = new Vector2(playerBox.size.x*0.8f, playerBox.size.y*0.8f);
        ignoreLayer = GetIgnoreLayer();
    }

    private void FixedUpdate()
    {
        List<Vector3> vertices = GetBoxCorners(playerBox);
        
        List<Ray2D> bottomPlayerRay2Ds= CreateEdgeRays(vertices[0], vertices[2], 5);
        List<Ray2D> eastPlayerRay2Ds= CreateEdgeRays(vertices[2], vertices[3], 12);
        List<Ray2D> westPlayerRay2Ds= CreateEdgeRays(vertices[0], vertices[1], 12);
        List<Ray2D> northPlayerRay2Ds= CreateEdgeRays(vertices[1], vertices[3], 6);
        
        DebugDrawAllRayCasts(bottomPlayerRay2Ds, rayCastLength);
        DebugDrawAllRayCasts(eastPlayerRay2Ds, rayCastLength);
        DebugDrawAllRayCasts(westPlayerRay2Ds, rayCastLength);
        DebugDrawAllRayCasts(northPlayerRay2Ds, rayCastLength);
        
        RaycastHit2D rch = CheckAllRayCastsForaHit(bottomPlayerRay2Ds, rayCastLength, ignoreLayer);
        
        if (rch && state != CharacterState.JUMPING)
        {
            if (state != CharacterState.GROUNDED)
            {
                state = CharacterState.GROUNDED;
                velY = 0;
                platformTop = SetGroundSlopeRotation(rch, vertices);
            }

            state = CharacterState.GROUNDED;
            velX = Mathf.SmoothDamp(velX, 0, ref vel, frictionX * Time.deltaTime);
        }
        else 
        {
            if (velY < 0f) state = CharacterState.FALLING; //no collision set state to falling
            currentGroundSlope = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            velY -= gravity_modifier * 9.81f * Time.smoothDeltaTime;
        }
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, currentGroundSlope, 0.5f);
        velY = Mathf.Clamp(velY, minFallClamp, max: maxFallClamp);

        if (state == CharacterState.GROUNDED) //constrain... player along equation of line for platform top
        {
            transform.position = new Vector3(transform.position.x + (platformTop.x * velX)  , transform.position.y + (platformTop.y * velX), 0);
        }
        else
        {
            transform.position = new Vector3(transform.position.x + velX, transform.position.y + velY, 0);
        }
    }

    private RaycastHit2D CheckAllRayCastsForaHit(List<Ray2D> bottomPlayerRay2Ds, float rayCastLength, int ignoreLayer)
    {
        RaycastHit2D rch = new RaycastHit2D();
        for (int i = 1; i < bottomPlayerRay2Ds.Count; i++)
        {
            //length from ray2d will be normalized to 1
            rch = Physics2D.Raycast(bottomPlayerRay2Ds[i].origin, bottomPlayerRay2Ds[i].direction, rayCastLength, ignoreLayer);
            if (rch)
            {
                Debug.DrawRay(bottomPlayerRay2Ds[i].origin, bottomPlayerRay2Ds[i].direction * rayCastLength, Color.red);
                // Debug.Log("Hit");
                return rch;
            }
            else
            {
                Debug.DrawRay(bottomPlayerRay2Ds[i].origin, bottomPlayerRay2Ds[i].direction * rayCastLength, Color.green);
            }
        }

        return rch;
    }

    private List<Ray2D> CreateEdgeRays(Vector3 a, Vector3 b, int rayDivisions)
    {
        Vector3 dir = b-a;
        
        DebugUtil.DrawMarker(a, Color.blue);
        DebugUtil.DrawMarker(b, Color.yellow);
        
        Vector3 perpendicular = new Vector2(dir.y, -dir.x).normalized;
        
        List<Ray2D> rays = new List<Ray2D>();
        
        for (float raySpacer = 0.001f; rays.Count<=rayDivisions; raySpacer += (dir.sqrMagnitude) / rayDivisions)
        {
            //eq of start line is :: a + (scale * dir)
            var startOfRay = new Vector3(a.x + (raySpacer * dir.x), a.y + (raySpacer * dir.y), 0);
            Ray2D r2d = new Ray2D(startOfRay, new Vector3(perpendicular.x, perpendicular.y, 0));
            rays.Add(r2d);
        }

        return rays; 
    }

    private Vector2 SetGroundSlopeRotation(RaycastHit2D rch, List<Vector3> vertices)
    {
        Vector3 playerSWCorner = vertices[0];
        Vector3 playerSECorner = vertices[2];
        Vector3 playerSouthLine = playerSECorner - playerSWCorner;
        
        // List<Vector3> platCorns = GetBoxCorners((BoxCollider2D) rch.collider);
        // Vector3 platformTopCorn = platCorns[1];
        // DebugUtil.DrawMarker(platformTopCorn, Color.cyan);
        platformTop = new Vector3(rch.normal.y, -rch.normal.x);
        // Debug.DrawRay(platformTopCorn, new Vector3(platformTop.x*2, platformTop.y*2), Color.magenta);
        // Debug.DrawRay(new Vector3(playerBottomLineMidPt.x, playerBottomLineMidPt.y, 0), new Vector3(playerBottomLine.x, playerBottomLine.y, 0), Color.blue);

        float angleBetweenPlayerAndPlatform = Vector3.SignedAngle(playerSouthLine, platformTop, Vector3.forward);
        // Debug.Log("Before player adjust " + angleBetweenPlayerAndPlatform);
        angleBetweenPlayerAndPlatform =
            angleBetweenPlayerAndPlatform +
            transform.rotation.eulerAngles.z; //don't consider existing 'player' rotation Z rotation
        // Debug.Log("After player adjust " + angleBetweenPlayerAndPlatform);
        currentGroundSlope = Quaternion.Euler(new Vector3(0, 0, angleBetweenPlayerAndPlatform));
        
        return platformTop;
    }


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

    private List<Vector3> GetBoxCorners(BoxCollider2D boxColl)
    {
        Transform bcTransform = boxColl.transform;

        // The collider's centre point in the world
        Vector3 worldPosition = bcTransform.TransformPoint(0, 0, 0);

        // The collider's local width and height, accounting for scale, divided by 2
        Vector2 size = new Vector2(boxColl.size.x * bcTransform.localScale.x * 0.5f,
            boxColl.size.y * bcTransform.localScale.y * 0.5f);

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

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    private static void DebugDrawAllRayCasts(List<Ray2D> bottomPlayerRay2Ds, float rayCastLength)
    {
        for (int i = 0; i < bottomPlayerRay2Ds.Count; i++)
        {
            Debug.DrawRay(bottomPlayerRay2Ds[i].origin, bottomPlayerRay2Ds[i].direction * rayCastLength, Color.red);
        }
    }
}