using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftOperator : MonoBehaviour
{
    [SerializeField] private Transform max = null;
    [SerializeField] private Transform min = null;
    // Start is called before the first frame update
    public enum State
    {
        TRAVELLING_TO_MIN, TRAVELLING_TO_MAX, STATIC_AT_MAX, STATIC_AT_MIN
    }

    public State state = State.STATIC_AT_MIN;
    private Vector3 target;

    void Start()
    {
        target = min.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > max.position.y) state = State.STATIC_AT_MAX;
        if (transform.position.y < min.position.y) state = State.STATIC_AT_MIN;
        
        if (state== State.TRAVELLING_TO_MIN) transform.position = Vector3.MoveTowards(transform.position, min.position, Time.deltaTime);
        if (state== State.TRAVELLING_TO_MAX) transform.position = Vector3.MoveTowards(transform.position, max.position, Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (state)
        {
            case State.STATIC_AT_MIN :
            {
                target = max.position;
                state = State.TRAVELLING_TO_MIN;
                break;
            }
            case State.STATIC_AT_MAX :
            {
                target = min.position;
                state = State.TRAVELLING_TO_MIN;
                break;
            }
        }
    }
}
