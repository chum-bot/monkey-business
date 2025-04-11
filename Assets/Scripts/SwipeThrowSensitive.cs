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
        Physics.gravity *= 4;
        rb = GetComponent<Rigidbody>();
        monkey = GetComponent<Monkey>();
        lastMousePos = Input.mousePosition;
        dragVector = Vector3.zero;
        dragTime = 0;
        if (monkey != null)
        {
            monkey.fly = false;
        }
        else
        {
            fly = false;
            // if this is a ragdoll, get every rigidbody on the root object
            rbs = transform.root.GetComponentsInChildren<Rigidbody>();
        }
        ragdoll = (monkey == null);
    }

    void OnMouseDown()
    {
        // only allow pick-up if not already flying
        if (monkey != null)
        {
            if (monkey.fly) return;
        }
        else
        {
            if (fly) return;
        }

        throwforce = Vector3.zero;
        isDragging = true;

        if (ragdoll)
        {
            // make all ragdoll bodies kinematic
            foreach (Rigidbody body in rbs)
            {
                body.isKinematic = true;
            }
            dragPlane = new Plane(Camera.main.transform.forward, transform.root.position);
        }
        else
        {
            rb.isKinematic = true;
            dragPlane = new Plane(Camera.main.transform.forward, transform.position);
        }

        startPos = Input.mousePosition;
        // for ragdoll, store the initial screen position
        if (ragdoll)
        {
            init = startPos;
        }
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
            Vector3 targetpos = ray.GetPoint(enter);
            if (ragdoll)
            {
                // move the entire ragdoll using the root position
                transform.root.position = targetpos;
            }
            else
            {
                transform.position = targetpos;
            }
        }
        // autorelease if there is a drag and the object's height is above a threshold
        if (dragVector.magnitude > 0)
        {
            float posY = ragdoll ? transform.root.position.y : transform.position.y;
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
        if (monkey != null)
        {
            monkey.fly = true;
        }
        else
        {
            fly = true;
        }
        if (ragdoll)
        {
            // revert all ragdoll bodies to non-kinematic so physics resumes
            foreach (Rigidbody body in rbs)
            {
                body.isKinematic = false;
            }
        }
        else
        {
            rb.isKinematic = false;
        }
        endPos = Input.mousePosition;

        if (dragTime > 0)
        {
            // calculate force based on drag distance and time
            throwforce = (endPos - startPos) / dragTime * Time.deltaTime;
            float minHeightScale = 677 / minHeight;
            float maxHeightScale = 677 / maxHeight;
            // use monkey.initPos if available; otherwise use the stored initial position
            float baseY = (monkey != null) ? monkey.initPos.y : init.y;
            float dragHeight = Math.Clamp((endPos - new Vector3(0, baseY, 0)).y,
                Camera.main.scaledPixelHeight / minHeightScale,
                Camera.main.scaledPixelHeight / maxHeightScale);

            float spd;
            if (monkey != null)
            {
                monkey.speed = maxSpeed / (Camera.main.scaledPixelHeight / maxHeightScale / Camera.main.scaledPixelHeight);
                spd = monkey.speed;
            }
            else
            {
                // for ragdolls, use a similar speed calculation
                spd = maxSpeed * (677f / maxHeight);
            }

            // add forward momentum and clamp the force magnitude
            throwforce.z += throwforce.magnitude;
            throwforce = throwforce.normalized * Math.Clamp(
                spd * dragHeight / Camera.main.scaledPixelHeight,
                minSpeed, maxSpeed);

            Debug.Log($"FLY OBJECT! WITH A SPEED OF {spd} AND A DRAG HEIGHT OF {dragHeight}, YOU SHALL REACH YOUR DESTINATION WITH A MAGNITUDE OF {spd * dragHeight / Camera.main.scaledPixelHeight}.");
            Debug.Log(Camera.main.scaledPixelHeight);
        }

        // apply the calculated impulse force
        if (ragdoll)
        {
            foreach (Rigidbody body in rbs)
            {
                body.AddForce(throwforce, ForceMode.Impulse);
            }
        }
        else
        {
            rb.AddForce(throwforce, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        // choose the proper position for gizmos based on object type
        Vector3 curPos = ragdoll ? transform.root.position : transform.position;
        Vector3 curVel = ragdoll ? Vector3.zero : rb.velocity;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(curPos, curPos + curVel);
        Gizmos.color = Color.black;
        Gizmos.DrawLine(curPos, curPos + throwforce);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(curPos, curPos + new Vector3(throwforce.x, curPos.y, curPos.z));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(curPos, curPos + new Vector3(curPos.x, throwforce.y, curPos.z));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(curPos, curPos + new Vector3(curPos.x, curPos.y, throwforce.z));
    }
}
