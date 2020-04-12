﻿﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using SpriteFlick.Util;
using Unity.Mathematics;

public class CharController : MonoBehaviour
{
    private BoxCollider2D playerBox;
    internal Vector2 vel;
    internal float minFallClamp = -0.5f, maxFallClamp = 0.5f, frictionX = 5f, frictionY = 0;
    private const float gravity_modifier = 0.05f;
    private const float rayCastLengthHorizontal = 0.075f;
    private const float rayCastLengthVertical = 0.15f;
    private const float knockBackConst = 50;

    private float angleBetweenPlayerAndPlatform;
    internal float correctedAngle;

    private Quaternion currentGroundSlope;
    internal Vector2 platformTop;
    private float ref_damp_vel;
    int ignoreLayer;
    internal bool collidingNorth, collidingEast, collidingSouth, collidingWest; 
    public enum CharacterState
    {
        GROUNDED,
        JUMPING,
        FALLING
    }

    public CharacterState state;
    internal int rayCnt;


    void Start()
    {
        playerBox = GetComponent<BoxCollider2D>();
        playerBox.size = new Vector2(playerBox.size.x*0.9f, playerBox.size.y*0.8f);
        ignoreLayer = GetIgnoreLayer();
    }

    public float frictionMultiplier;
    internal float actingFriction;

    private void FixedUpdate()
    {
        Vector3[] vertices = GetBoxCorners(playerBox);

        List<Ray2D> northPlayerRay2Ds = CreateEdgeRays(vertices[1], vertices[3], true);
        List<Ray2D> southPlayerRay2Ds = CreateEdgeRays(vertices[0], vertices[2], false);
        List<Ray2D> eastPlayerRay2Ds = CreateEdgeRays(vertices[2], vertices[3], false, false);
        List<Ray2D> westPlayerRay2Ds = CreateEdgeRays(vertices[0], vertices[1], true, false);

        RaycastHit2D southRch = CheckAllRayCastsForaHit(ref southPlayerRay2Ds, rayCastLengthVertical, ignoreLayer);
        RaycastHit2D northRch = CheckAllRayCastsForaHit(ref northPlayerRay2Ds, rayCastLengthVertical, ignoreLayer);
        RaycastHit2D eastRch = CheckAllRayCastsForaHit(ref eastPlayerRay2Ds, rayCastLengthHorizontal, ignoreLayer);
        RaycastHit2D westRch = CheckAllRayCastsForaHit(ref westPlayerRay2Ds, rayCastLengthHorizontal, ignoreLayer);

        if (northRch && vel.y > 0.01f)
        {
            collidingNorth = true;
            vel.y = -vel.y / 6 * knockBackConst * Time.deltaTime;
        }
        else collidingNorth = false;

        if (eastRch && vel.x > 0.01f)
        {
            collidingEast = true;
            vel.x = -vel.x /  6 * knockBackConst * Time.deltaTime;
        }
        else collidingEast = false;
        
        if (westRch && vel.x < 0.01f)
        {
            collidingWest = true;
            vel.x = -vel.x /  6 * knockBackConst * Time.deltaTime;
        }
        else collidingWest = false;

        if (southRch && state!=CharacterState.JUMPING)
        {
           state = CharacterState.GROUNDED;
           vel.y = 0;
           actingFriction = GetFriction(southRch);
           vel.x = Mathf.SmoothDamp(vel.x, 0, ref ref_damp_vel,  (frictionX * Time.deltaTime) * actingFriction);
           platformTop = SetGroundSlopeRotation(southRch, vertices); 
           if (correctedAngle<40f) transform.rotation =  Quaternion.RotateTowards(transform.rotation, currentGroundSlope, 4f); //to stop small change thrashing
           transform.position = new Vector3(transform.position.x + (platformTop.x * vel.x * knockBackConst * Time.deltaTime), transform.position.y + (platformTop.y * vel.x* knockBackConst * Time.deltaTime), 0);
        }
        else
        {
            state = CharacterState.FALLING;
            vel.y -= gravity_modifier * 9.81f * Time.smoothDeltaTime;
            if (vel.y<-0.01f) transform.rotation =  Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.position.x, transform.position.y, 0), 7.5f); //to stop small change thrashing
            vel.y = Mathf.Clamp(vel.y, minFallClamp, max: maxFallClamp);
            transform.position = new Vector3(transform.position.x + vel.x* knockBackConst * Time.deltaTime, transform.position.y + vel.y* knockBackConst * Time.deltaTime, 0);
        }
    }

    float GetFriction(RaycastHit2D rch)
    {
        if ((vel.x > 0.01f && rch.collider.transform.rotation.eulerAngles.z < 0) || (vel.x < 0.01f && rch.collider.transform.rotation.eulerAngles.z > 0))
        {
            correctedAngle = angleBetweenPlayerAndPlatform;
            if (correctedAngle < 2f) correctedAngle = 0f;
            if (correctedAngle > 90) correctedAngle = (360 - correctedAngle);
            if (correctedAngle < 17f) return 1;

            float frictionCoefficient = 1/(correctedAngle * correctedAngle * frictionMultiplier);
            Mathf.Clamp(frictionCoefficient, 0.001f, 10f); //avoid overflow on where gradient approaches 0 or 90
            return frictionCoefficient;
        }
        
        return 1;
    }
    
    private List<Ray2D> CreateEdgeRays(Vector3 a, Vector3 b, bool reverseLine, bool useTopEdgeRay = true)
    {
        Vector3 displacement = b-a; //distance between two corners
        
        Vector3 perpendicular = reverseLine ? new Vector2(-displacement.y, displacement.x).normalized : new Vector2(displacement.y, -displacement.x).normalized; // in some cases we want to shoo ary in revser
        
        List<Ray2D> rays = new List<Ray2D>();
        if (useTopEdgeRay) rays.Add(new Ray2D(a, new Vector3(perpendicular.x, perpendicular.y, 0))); //put a ray on point a 
        rays.Add(new Ray2D(b, new Vector3(perpendicular.x, perpendicular.y, 0))); //and b

        float rayDivisions = 0.1f;
        for (float scalar = rayDivisions; ;scalar += rayDivisions) //we we travel along equation of line inserting ray ever 1/4 of line
        {
            Vector3 startOfRay = new Vector3(a.x + (scalar * displacement.x), a.y + (scalar * displacement.y), 0);
            if ((startOfRay - a).magnitude > displacement.magnitude) break; //length of line eq - start point > original line
            Ray2D r2d = new Ray2D(startOfRay, new Vector3(perpendicular.x, perpendicular.y, 0));
            rays.Add(r2d);
        }

        rayCnt = rays.Count;
        return rays; 
    }
    
    private RaycastHit2D CheckAllRayCastsForaHit(ref List<Ray2D> rays, float rayCastLength, int ignoreLayer)
    {
        RaycastHit2D rch = new RaycastHit2D();
        List<Tuple<RaycastHit2D, int>> hits = new List<Tuple<RaycastHit2D, int>>();
        for (int i = 0; i < rays.Count; i++)
        {
            //length from ray2d will be normalized to 1
            rch = Physics2D.Raycast(rays[i].origin, rays[i].direction, rayCastLength, ignoreLayer);
            Debug.DrawRay(rays[i].origin, rays[i].direction * rayCastLength, Color.green);
            if (rch) hits.Add(new Tuple<RaycastHit2D, int>(rch, i)); //else rays.RemoveRange(i,1);
        }
        
        if (hits.Count > 0)
        {
            hits.Sort((hit1, hit2)=> hit1.Item1.distance.CompareTo(hit2.Item1.distance));
            Debug.DrawRay(rays[hits[0].Item2].origin, rays[hits[0].Item2].direction * rayCastLength, Color.red);
            return hits[0].Item1;
        }


        return rch;
    }

    private Vector2 SetGroundSlopeRotation(RaycastHit2D rch, Vector3[] vertices)
    {
        Vector3 playerSWCorner = vertices[0];
        Vector3 playerSECorner = vertices[2];
        Vector3 playerSouthLine = playerSECorner - playerSWCorner;
        
        Vector3[] platformCorners = GetBoxCorners((BoxCollider2D) rch.collider);
        Vector3 platformTopCorner = platformCorners[1];
        DebugUtil.DrawMarker(platformTopCorner, Color.cyan);
        platformTop = new Vector3(rch.normal.y, -rch.normal.x);
        Debug.DrawRay(platformTopCorner, new Vector3(platformTop.x*2, platformTop.y*2), Color.magenta); //issue this is changing
        Debug.DrawRay(new Vector3(playerSWCorner.x, playerSWCorner.y, 0), new Vector3(playerSouthLine.x, playerSouthLine.y, 0), Color.blue);

        angleBetweenPlayerAndPlatform = Vector3.SignedAngle(playerSouthLine, platformTop, Vector3.forward);
        angleBetweenPlayerAndPlatform = angleBetweenPlayerAndPlatform + transform.rotation.eulerAngles.z; //don't consider existing 'player' rotation Z rotation
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

    private Vector3[] GetBoxCorners(BoxCollider2D boxColl)
    {
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
        
        Vector3[] vertList = new Vector3[4];
        vertList[0] = corner1;
        vertList[1] = corner2;
        vertList[2] = corner3;
        vertList[3] = corner4;
        return vertList;
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    private static void DebugDrawAllRayCasts(List<Ray2D> rays, float rayCastLength)
    {
        for (int i = 0; i < rays.Count; i++)
        {
            Debug.DrawRay(rays[i].origin, rays[i].direction * rayCastLength, Color.red);
        }
    }
}