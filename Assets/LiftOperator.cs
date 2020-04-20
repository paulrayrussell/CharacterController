using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftOperator : MonoBehaviour
{
    [SerializeField] private Transform max = null;
    [SerializeField] private Transform min = null;
    [SerializeField] private float vel = 1;
    // Start is called before the first frame update
    public enum State
    {
        STATIC, MAX_PT, MIN_PT
    }

    public State state = State.STATIC;
    private Vector3 target;

    void Start()
    {
        target = min.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= max.position.y)
        {
            state = State.MAX_PT;
            vel = -1;
        }
        if (transform.position.y <= min.position.y)
        {
            state = State.MIN_PT;
            vel = 1;
        }
        
        if (state!= State.STATIC) transform.position = Vector3.MoveTowards(transform.position, target, vel * Time.deltaTime);
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER");
        if (state== State.STATIC) vel = 1;
    }
}
