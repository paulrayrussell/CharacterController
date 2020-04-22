using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmsController : MonoBehaviour
{
    private Vector3 vDiff;
    private float atan2;

    public Vector3 clickpt;
    // Update is called once per frame
    void Update()
    {
        RotateToClickPt();
    }

    private void RotateToClickPt()
    {
        vDiff = (clickpt - transform.position);
        atan2 = Mathf.Atan2(-vDiff.y, -vDiff.x); //added minus to x, y...not sure why this works
        transform.rotation = Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg);
    }
    
}
