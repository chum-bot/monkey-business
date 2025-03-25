using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static MBNamespace.MBFunctions;

public class SwipeThrowSensitive : MonoBehaviour
{
    [SerializeField]
    float maxSpeed = 100f;

    [SerializeField]
    float minSpeed = 10.0f;

    private Vector3 startPos;
    private Vector3 endPos;
    private bool isDragging;

    //
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
        if (dragVector.magnitude > 0 && transform.position.y > Camera.main.orthographicSize + 16) 
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
        //the dragSpeed check is so it doesn't go flying when you release it and have only been barely moving
        if (dragTime > 0)
        {
            //the throwforce
            throwforce = (endPos-startPos) / dragTime * Time.deltaTime;
            //it has the direction of the dragVector
            //so it goes in the direction you dragged it

            float dragHeight = Math.Clamp((endPos - monkey.initPos).y, 50, 400);

            throwforce.z += throwforce.magnitude; // the forward
            throwforce = throwforce.normalized * Math.Clamp(
                monkey.speed * 
                (dragHeight / Camera.main.scaledPixelHeight) * 
                (monkey.dragSensitivity * dragVector.magnitude), 
                minSpeed, maxSpeed);
            Debug.Log(dragHeight);
            //basically how this works is
            //it takes the base speed of the monkey and multiplies the throwforce by the height of the drag
            //so it has that level of power
            //with a min and max so it doesn't fly too far
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