using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugInfoUpdater : MonoBehaviour
{
    [SerializeField] private Text vel= null;
    [SerializeField] private Text state= null;
    [SerializeField] private Text angle = null;
    [SerializeField] private Text platform = null;
    [SerializeField] private Text ray_cnt = null;
    [SerializeField] private Text collideE = null;
    [SerializeField] private Text collideW = null;
    [SerializeField] private Text corrAngle = null;
    [SerializeField] private Text negPlafotfm = null;
    [SerializeField] private Text lift = null;
    [SerializeField] private Text maxVel = null;
    [SerializeField] private CharController pc = null;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private float maxX, maxY, minX, minY;
    // Update is called once per frame
    void Update()
    {
        vel.text = "Vel: " + pc.vel.ToString("n2");
        state.text = "State: " + pc.state;
        platform.text = "Platform: " + pc.platformTop.ToString("n2");
        ray_cnt.text = "Raycnt: " + pc.rayCnt;
        angle.text = "Angle: " + pc.correctedAngle;
        collideE.text = "Coll E: " + pc.collidingEast;
        collideW.text = "Coll W: " + pc.collidingWest;
        corrAngle.text = "Angle player & platform : " + pc.angleBetweenPlayerAndPlatform;
        negPlafotfm.text = "Neg slope: " + pc.negativesSlope;
        lift.text = "Lifting : " + pc.liftingCharacter;
        if (pc.vel.x > maxX) maxX = pc.vel.x;
        if (pc.vel.x < minX) minX = pc.vel.x;
        if (pc.vel.y > maxY) maxY = pc.vel.y;
        if (pc.vel.y < minY) minY = pc.vel.y;

        maxVel.text = "MaxVel : " + maxX.ToString("n2") + "," +  maxX.ToString("n2") + " - MinVel : " + minX.ToString("n2") + "," +  minY.ToString("n2");
    }
}
