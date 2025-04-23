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

    // this is literally only here for the gizmos
    // i wouldn't define it outside of here otherwise
    private Vector3 throwforce;

    private Monkey monkey;
    private Rigidbody rb;

    // new variables for ragdoll support:
    private bool ragdoll;      // indicates whether this is a ragdoll (true if no Monkey component)
    private bool fly;          // used for ragdoll fly state
    private Rigidbody[] rbs;   // holds all rigidbodies (ragdoll parts)
    private Vector3 init;      // initial screen position for ragdoll objects

    private Plane dragPlane;

    // dragTime!
    // tracks how long the player is actively moving the object
    // so we can get the speed at which they flicked it
    private float dragTime;

    void Start()
    {
        monkey = GetComponentInParent<Monkey>();
        lastMousePos = Input.mousePosition;
        dragVector = Vector3.zero;
        dragTime = 0;
        monkey.fly = false;
    }

    void OnMouseDown()
    {
        // only allow pick-up if not already flying
        if (monkey.fly) return;
        throwforce = Vector3.zero;
        isDragging = true;

        // make all ragdoll bodies kinematic
        foreach (Rigidbody rb in monkey.rbs)
        {
            rb.isKinematic = true;
        }
        dragPlane = new Plane(Camera.main.transform.forward, monkey.transform.position);

        startPos = Input.mousePosition;
        dragVector = Vector3.zero;
        dragTime = 0;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        // update drag vector based on the current and last mouse positions
        dragVector = Input.mousePosition - lastMousePos;
        if (dragVector.magnitude > 0.0f)
        {
            dragTime += Time.deltaTime;
        }
        else
        {
            dragTime = 0;
            startPos = Input.mousePosition;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            transform.position = ray.GetPoint(enter);

        }
        // autorelease if there is a drag and the object's height is above a threshold
        if (dragVector.magnitude > 0)
        {
            float posY = monkey.transform.position.y;
            if (posY > Camera.main.orthographicSize + 18)
            {
                Throw();
            }
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
        // revert all ragdoll bodies to non-kinematic so physics resumes
        foreach (Rigidbody body in monkey.rbs)
        {
            body.isKinematic = false;
        }
        endPos = Input.mousePosition;

        if (dragTime > 0)
        {
            // calculate force based on drag distance and time
            throwforce = (endPos - startPos) / dragTime * Time.deltaTime;
            float minHeightScale = 677 / minHeight;
            float maxHeightScale = 677 / maxHeight;
            // use monkey.initPos if available; otherwise use the stored initial position
            float baseY = monkey.initPos.y;
            float dragHeight = Math.Clamp(endPos.y - baseY,
                Camera.main.scaledPixelHeight / minHeightScale,
                Camera.main.scaledPixelHeight / maxHeightScale);

                monkey.speed = maxSpeed / (Camera.main.scaledPixelHeight / maxHeightScale / Camera.main.scaledPixelHeight);
            

            // add forward momentum and clamp the force magnitude
            throwforce.z += throwforce.magnitude;
            throwforce = throwforce.normalized * Math.Clamp(
                monkey.speed * dragHeight / Camera.main.scaledPixelHeight,
                minSpeed, maxSpeed);

            Debug.Log($"FLY OBJECT! WITH A SPEED OF {monkey.speed} AND A DRAG HEIGHT OF {dragHeight}, YOU SHALL REACH YOUR DESTINATION WITH A MAGNITUDE OF {monkey.speed * dragHeight / Camera.main.scaledPixelHeight}.");
        }

        // apply the calculated impulse force
            foreach (Rigidbody body in monkey.rbs)
            {
                body.AddForce(throwforce, ForceMode.Impulse);
            }
    }

    private void OnDrawGizmos()
    {
        // choose the proper position for gizmos based on object type
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(monkey.initPos, 1f);
    }

    //i do not understand why this is needed
    //but it works so
    void Update()
    {
            bool down = Input.GetMouseButtonDown(0);
            bool up = Input.GetMouseButtonUp(0);
            bool hold = Input.GetMouseButton(0);

            if (down)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {

                    if (Array.Exists(monkey.rbs, rb => rb == hit.rigidbody))
                    {
                        OnMouseDown();
                    }
                }
            }
            if (hold && isDragging)
            {
                OnMouseDrag();
            }
            if (up && isDragging)
            {
                OnMouseUp();
            }
        
    }
}
