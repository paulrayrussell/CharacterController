﻿﻿using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    [SerializeField] float minEdge = 0.2f;
    [SerializeField] float maxEdge = 0.8f;
    [SerializeField] float emergencyMinEdge = 0.1f;
    [SerializeField] float emergencyMaxEdge = 0.9f;
    [SerializeField] private float dtMult = 0.25f;

    private CharController charController;
    private GameObject player;
    private Camera cam;
    private Vector3 rhsPosY;
    private Vector3 lhsPosY;
    private Vector3 rhsPosX;
    private Vector3 lhsPosX;
    
    void Start()
    {
        cam = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");
        charController = player.GetComponent<CharController>();
    }

    public enum State
    {
        gtMax_POSVelocity, gtMax_NegVelocity, ltMin_NegVelocity, ltMin_PosVelocity, ltMax_NegVelocity_Norm, gtMin_PosVelocity_Norm, gtEmergency_POSVelocity, ltEmergency_POSVelocity
    }

    public State state = State.gtMin_PosVelocity_Norm;
    private Vector3 velX,vel2X,vel3X,vel4X;
    private Vector3 velY,vel2Y,vel3Y,vel4Y;

    void LateUpdate()
    {
        rhsPosX = cam.ViewportToWorldPoint(new Vector3(maxEdge, cam.WorldToViewportPoint(cam.transform.position).y, cam.WorldToViewportPoint(cam.transform.position).z));
        lhsPosX = cam.ViewportToWorldPoint(new Vector3(minEdge, cam.WorldToViewportPoint(cam.transform.position).y, cam.WorldToViewportPoint(cam.transform.position).z));
        
        rhsPosY = cam.ViewportToWorldPoint(new Vector3(cam.WorldToViewportPoint(cam.transform.position).x, maxEdge, cam.WorldToViewportPoint(cam.transform.position).z));
        lhsPosY = cam.ViewportToWorldPoint(new Vector3(cam.WorldToViewportPoint(cam.transform.position).x, minEdge, cam.WorldToViewportPoint(cam.transform.position).z));
        
        float t = dtMult;

        float xPos = cam.WorldToViewportPoint(player.transform.position).x;
        
        if (xPos > emergencyMaxEdge && charController.vel.x>0) 
        {
            cam.transform.position = new Vector3(cam.transform.position.x + charController.vel.x, cam.transform.position.y, cam.transform.position.z);
            state = State.gtEmergency_POSVelocity;
        } 
        else if (xPos > maxEdge && charController.vel.x>0) 
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position,rhsPosX, ref velX, t);
            state = State.gtMax_POSVelocity;
        } 
        else if (xPos > maxEdge && charController.vel.x<0) 
        {
            cam.transform.position = new Vector3(cam.transform.position.x + charController.vel.x, cam.transform.position.y, cam.transform.position.z);
            state = State.gtMax_NegVelocity;
        } 
        else if (xPos < emergencyMinEdge && charController.vel.x<0) 
        {
            cam.transform.position = new Vector3(cam.transform.position.x + charController.vel.x, cam.transform.position.y, cam.transform.position.z);
            state = State.ltEmergency_POSVelocity;
        }
        else if (xPos < emergencyMinEdge && charController.vel.x>0) 
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, lhsPosX, ref vel3X, t);
            state = State.ltEmergency_POSVelocity;
        }
        else if (xPos < minEdge && charController.vel.x<0)
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, lhsPosX, ref vel2X, t);
            state = State.ltMin_NegVelocity;
        }       
        else if (xPos < minEdge && charController.vel.x>0)
        {
            cam.transform.position = new Vector3(cam.transform.position.x + charController.vel.x, cam.transform.position.y, cam.transform.position.z);
            state = State.ltMin_PosVelocity;
        }
        else if (xPos < maxEdge && charController.vel.x<0) //norm #1
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, lhsPosX, ref vel3X, t);
            state = State.ltMax_NegVelocity_Norm;
        }
        else if (xPos > minEdge && charController.vel.x>0) //norm #2
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, rhsPosX, ref vel4X, t);
            state = State.gtMin_PosVelocity_Norm;
        }

        // Debug.Log(state);
        
        float yPos = cam.WorldToViewportPoint(player.transform.position).y;

        if (yPos > emergencyMaxEdge && charController.vel.y>0) 
        {
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y+ charController.vel.y, cam.transform.position.z);
        } 
        else if (yPos > maxEdge && charController.vel.y>0) 
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, rhsPosY, ref velY, t);
        } 
        else if (yPos > maxEdge && charController.vel.y<0)
        {
            cam.transform.position =  new Vector3(cam.transform.position.x, cam.transform.position.y+ charController.vel.y, cam.transform.position.z);
        } 
        else if (yPos < emergencyMinEdge && charController.vel.y<0)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + charController.vel.y, cam.transform.position.z);
        }
        else if (yPos < minEdge && charController.vel.y<0)
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, lhsPosY, ref vel2Y, t);
        }       
        else if (yPos < minEdge && charController.vel.y>0)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y+ charController.vel.y, cam.transform.position.z);
        }
        else if (yPos < maxEdge && charController.vel.y<0) //norm #1
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, lhsPosY, ref vel3Y, t);
        }
        else if (yPos > minEdge && charController.vel.y>0) //norm #2
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, rhsPosY, ref vel4Y, t);
        }

    }
}
