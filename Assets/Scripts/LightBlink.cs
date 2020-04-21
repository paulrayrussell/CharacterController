using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightBlink : MonoBehaviour
{
    // Start is called before the first frame update
    private Light2D localLight;
    [SerializeField] private float onDelay = 1;
    [SerializeField] private float offDelay = 2;

    void Start()
    {
        localLight = GetComponent<Light2D>();
        StartCoroutine(StartOnOff());
    }

    private IEnumerator StartOnOff()
    {
        while (true)
        {
            localLight.enabled = true;
            yield return new WaitForSeconds(onDelay);
            localLight.enabled = false;
            yield return new WaitForSeconds(offDelay);
        }
    }
}