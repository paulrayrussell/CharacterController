using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform player;
    private Vector3 vel;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z), ref vel, 0.5f);
    }
}
