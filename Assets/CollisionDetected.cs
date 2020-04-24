using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetected : MonoBehaviour
{
    public EventHandler collisionEnter;
    private BoxCollider2D cd2d;

    void Start()
    {
        cd2d = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider.name.StartsWith("Bullet")) return;
        cd2d.enabled = false;
        collisionEnter?.Invoke(this, new EventArgs());
        Debug.Log(other.collider.name);
    }
}
