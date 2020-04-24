using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerEnemyBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private float vel;
    private Vector3 startPos, endPos;
    
    public void Setup(Vector3 shootDir, float speed)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(shootDir * speed, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Debug.Log("Bullet hit " + other.collider + " - destroying bullet");
        // Destroy(this);
        gameObject.SetActive(false);
    }
    

    private void Update()
    {
        if (Camera.main.WorldToScreenPoint(transform.position).x > Camera.main.scaledPixelWidth + (Camera.main.scaledPixelWidth*0.2f) || Camera.main.WorldToScreenPoint(transform.position).x < - (Camera.main.scaledPixelWidth*0.2f)) gameObject.SetActive(false);
    }
}
