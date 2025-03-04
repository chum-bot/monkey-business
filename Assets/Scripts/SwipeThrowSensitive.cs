using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static MBNamespace.MBFunctions;

public class SwipeThrowSensitive : MonoBehaviour
{
    public float verticalSensitivity = 0.001f;

    public float horizontalSensitivity = 0.001f;
    public float upwardSensitivity = 0.004f;

    [SerializeField]
    float sensitivity = 1.0f;

    [SerializeField]
    float maxSpeed = 100f;

    [SerializeField]
    float minSpeed = 10.0f;

    private Vector3 startPos;
    private Vector3 endPos;
    private bool isDragging;

    //my stuff for testing
    private Vector3 lastMousePos;
    private Vector3 dragSpeed;

    private Vector3 throwVelocity;
    private Vector3 throwVel;

    private Monkey monkey; //defined the monkey being thrown for use with antigrab
    private Rigidbody rb;

    private Plane dragPlane;

    private float dragTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        monkey = GetComponent<Monkey>();
        lastMousePos = Input.mousePosition;
        dragSpeed = Vector3.zero;
        dragTime = 0;
    }


    void OnMouseDown()
    {
        if (AntiGrab(monkey)) //don't forget to add this when you add your throwing script!
        {
            isDragging = true;

            rb.isKinematic = true;
            dragPlane = new Plane(Camera.main.transform.forward, transform.position);


            startPos = Input.mousePosition;
            dragSpeed = Vector3.zero;
            dragTime = 0;

        }
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        dragSpeed = (Input.mousePosition - lastMousePos);
        if(dragSpeed.sqrMagnitude > 0.0f)
        {
            dragTime += Time.deltaTime;
        }
        else
        {
            dragTime = 0;
            startPos = Input.mousePosition; //so you can move the cursor to aim and you don't have to start from where you first grabbed the monkey
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            transform.position = ray.GetPoint(enter);
        }
        lastMousePos = Input.mousePosition;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;
        rb.isKinematic = false;
        endPos = Input.mousePosition;

        //float vert = endPos.y - startPos.y;
        //float hor = endPos.x - startPos.x;
        //float horSpeed = hor / dragTime;
        //float speed = horSpeed * horizontalSensitivity;
        //float verticalSpeed = vert / dragTime;
        //float vertSpeed = verticalSpeed * verticalSensitivity;
        //float upSpeed = verticalSpeed * upwardSensitivity;

        //all of that up top but in one thing
        //only goes if the dragTime isn't 0 so it doesn't give you a NaN error later
        if (dragTime > 0 && dragSpeed.magnitude > minSpeed)
        {
            Vector3 relationForward = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.forward.z);

            throwVel = (endPos - startPos) / dragTime * Time.deltaTime;

            throwVel.z = relationForward.z * throwVel.magnitude;
            Debug.Log(relationForward);
            Debug.Log(throwVel);
            throwVel = throwVel.normalized * Math.Clamp(dragSpeed.magnitude, minSpeed, maxSpeed) * monkey.speed; //goes as far as the dragSpeed

        }


        Debug.Log($"{dragSpeed.magnitude} is the dragSpeed");

        //Vector3 cam = Camera.main.transform.forward;
        //cam.y = 0;
        //cam.Normalize();

        //Vector3 camRight = Camera.main.transform.right;

        //throwVelocity = cam * vertSpeed + camRight * speed + Vector3.up * upSpeed;
        rb.AddForce(throwVel, ForceMode.VelocityChange);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.transform.position + Camera.main.transform.forward);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.transform.position + throwVel);
    }

}