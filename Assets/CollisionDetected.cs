using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetected : MonoBehaviour
{
    public EventHandler collisionEnter;
    private BoxCollider2D cd2d;
    [SerializeField] private EnemyAnimationController anim = null;

    void Start()
    {
        cd2d = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider.name.StartsWith("Bullet")) return;
        cd2d.enabled = false;
        float offset;
        if (anim.spriteFacingRight) offset = -0.6f;
        else offset = 0.6f;
        collisionEnter?.Invoke(this, new LocEventArg(new Vector3(other.contacts[0].point.x+offset, other.contacts[0].point.y, 0)));
        Debug.Log(other.collider.name);
    }
    
    public class LocEventArg : EventArgs
    {
        public Vector3 location;

        public LocEventArg(Vector3 location)
        {
            this.location = location;
        }
    }
}
