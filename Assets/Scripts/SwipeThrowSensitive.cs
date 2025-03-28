using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static MBNamespace.MBFunctions;
using UnityEngine.UI;

public class SwipeThrowSensitive : MonoBehaviour
{
    [SerializeField]
    float maxSpeed = 100f;

    [SerializeField]
    float minSpeed = 10.0f;

    [SerializeField]
    float minHeight = 240;
    [SerializeField]
    float maxHeight = 450;

    private Vector3 startPos;
    private Vector3 endPos;
    private bool isDragging;


    private Vector3 lastMousePos;
    private Vector3 dragVector;

    //this is literally only here for the gizmos
    //i wouldn't define it outside of here otherwise
    private Vector3 throwforce;

    private Monkey monkey;
    private Rigidbody rb;

    private Plane dragPlane;

    //dragTime!
    //tracks how long the player is actively moving the monkey
    //so we can get the speed at which they flicked it
    private float dragTime;

    void Start()
    {
        Physics.gravity *= 4;
        rb = GetComponent<Rigidbody>();
        monkey = GetComponent<Monkey>();
        lastMousePos = Input.mousePosition;
        dragVector = Vector3.zero;
        dragTime = 0;
        monkey.fly = false;
    }


    void OnMouseDown()
    {
        if (!monkey.fly)
        {
            throwforce = Vector3.zero;
            isDragging = true;

            rb.isKinematic = true;
            dragPlane = new Plane(Camera.main.transform.forward, transform.position);

            startPos = Input.mousePosition;
            dragVector = Vector3.zero;
            dragTime = 0;
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        //this changes every frame you're holding it
        //honestly this is more like dragDistance, but endPos - startPos is the true drag distance so
        dragVector = Input.mousePosition - lastMousePos;
        if(dragVector.magnitude > 0.0f)
        {
            //dragTime counts up when you're dragging
            dragTime += Time.deltaTime;
        }
        else
        {
            //and resets when you're not
            dragTime = 0;
            startPos = Input.mousePosition; 
            //the startPos reset makes it so you can move the cursor from the starting position to aim
            //and you don't have to start from where you first grabbed the monkey
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            transform.position = ray.GetPoint(enter);
        }
        //the autorelease
        if (dragVector.magnitude > 0 && transform.position.y > Camera.main.orthographicSize + 18) 
        {
            Throw();
        }
        lastMousePos = Input.mousePosition;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        Throw();
    }

    void Throw()
    {
        isDragging = false;
        monkey.fly = true;
        rb.isKinematic = false;
        endPos = Input.mousePosition;

        //only goes if the dragTime isn't 0 so it doesn't give you a NaN error later
        if (dragTime > 0)
        {
            //the throwforce
            //it has the direction of the vector you dragged in
            //so it goes in the direction you dragged it
            throwforce = (endPos-startPos) / dragTime * Time.deltaTime;

            //when testing i found that
            //with some specific values at the specific camera height i was testing with in the builder (677)
            //the lowest power throw always sinks the front, and the highest power throw always sinks the back (with good enough aim ofc)
            //so in order to scale that to the window i took those values and the relationship between them
            //and used them down here

            float minHeightScale = 677 / minHeight;
            float maxHeightScale = 677 / maxHeight;

            //min and max height are used as the "range" of throwing heights to use
            //these are not the exact mins and maxes, those are calced in the clamp below
            //these scales are the ratio between the specific values i found with the 677 builder window
            //they are now scaled to the height of the window so the ratio remains for all of them

            float dragHeight = Math.Clamp((endPos - monkey.initPos).y, 
                Camera.main.scaledPixelHeight / minHeightScale,
                Camera.main.scaledPixelHeight / maxHeightScale);

            //monkey speed scaling based on the max speed
            //for those crisp perfect back sinks
            monkey.speed = maxSpeed / (Camera.main.scaledPixelHeight / maxHeightScale / Camera.main.scaledPixelHeight);

            throwforce.z += throwforce.magnitude; // the forward
            throwforce = throwforce.normalized * Math.Clamp(
                monkey.speed * dragHeight / Camera.main.scaledPixelHeight, 
                minSpeed, maxSpeed);
        }
        rb.AddForce(throwforce, ForceMode.Impulse);
        //impulse makes it mass-based so we can change that and have it affect the throwing
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + rb.velocity);
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + throwforce);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(throwforce.x, transform.position.y, transform.position.z));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(transform.position.x, throwforce.y, transform.position.z));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(transform.position.x, transform.position.y, throwforce.z));
    }

}