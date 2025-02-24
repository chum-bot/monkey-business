using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MBNamespace.MBFunctions;

public class SwipeThrowSensitive : MonoBehaviour
{
    public float verticalSensitivity = 0.001f;

    public float horizontalSensitivity = 0.001f;
    public float upwardSensitivity = 0.004f;

    private Vector2 startPos;
    private bool isDragging;
    private float startTime;

    private Monkey monkey; //defined the monkey being thrown for use with antigrab
    private Rigidbody rb;

    private Plane dragPlane;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        monkey = GetComponent<Monkey>();
    }

    void OnMouseDown()
    {
        if (AntiGrab(monkey)) //don't forget to add this when you add your throwing script!
        {
            isDragging = true;

            rb.isKinematic = true;
            dragPlane = new Plane(Camera.main.transform.forward, transform.position);


            startPos = Input.mousePosition;
            startTime = Time.time;

        }
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {

            transform.position = ray.GetPoint(enter);
        }
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;
        rb.isKinematic = false;
        Vector2 endPos = Input.mousePosition;
        float vert = endPos.y - startPos.y;
        float elapsed = Mathf.Max(Time.time - startTime, 0.01f);

        float hor = endPos.x - startPos.x;
        float horSpeed = hor / elapsed;
        float speed = horSpeed * horizontalSensitivity;
        float verticalSpeed = vert / elapsed;

        float vertSpeed = verticalSpeed * verticalSensitivity;

        float upSpeed = verticalSpeed * upwardSensitivity;

        Vector3 cam = Camera.main.transform.forward;
        cam.y = 0;
        cam.Normalize();

        Vector3 camRight = Camera.main.transform.right;
        Vector3 throwVelocity = cam * vertSpeed + camRight * speed + Vector3.up * upSpeed;
        rb.velocity = throwVelocity;
    }

}