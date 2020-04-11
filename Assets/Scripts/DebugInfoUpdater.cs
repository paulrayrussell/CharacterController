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
    [SerializeField] private CharController pc = null;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        vel.text = "Vel: " + pc.vel;
        state.text = "State: " + pc.state;
        platform.text = "Platform: " + pc.platformTop;
        ray_cnt.text = "Raycnt: " + pc.rayCnt;
        angle.text = "Angle: " + pc.angleBetweenPlayerAndPlatform;
        
        
    }
}
