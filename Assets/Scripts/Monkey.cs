using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBNamespace;
using UnityEngine.UI;
using static MBNamespace.MBFunctions;
using static MBNamespace.MBVars;
using System;

public class Monkey : MonoBehaviour
{
    [SerializeField]
    public SORTING sortingMetric;

    [SerializeField]
    public int pointValue;
    //i think it'd be cool if each monkey had point values, we could manipulate that
    //solid gold monkey worth 20 thousand points or smth silly

    [SerializeField]
    public float speed = 1.0f;

    public bool fly { get; set; } //fly! (check for when monkey is thrown, replacement for long winded antigrab stuff)

    public Rigidbody[] rbs { get; set; }

    public MONKEYTYPE type;
    public string color { get; set; }

    private string fullGovernmentColor;

    private float softlockTimer;

    public Vector3 initPos { get; set; }

    public Dictionary<Rigidbody, Vector3> rbInitPos;
    public Vector3 typeforce { get; set; }

    //getting the average positon of all the rigidbodies in the ragdoll
    //so i have a singular point to reference for the monkey's initPos
    public Vector3 AverageRBPos(Rigidbody[] positions)
    {
        Vector3 avg = Vector3.zero;
        foreach (Rigidbody rb in positions)
        {
            avg += rb.position;
        }
        return avg / positions.Length;
    }

    // Start is called before the first frame update
    void Start()
    {
        //it'll just be the color for now
        //we can change this later once the models have more stuff going on
        //i'm taking directly from it now so i don't have to hardcode colors in
        //and we can easily make new ones w/ materials
        rbs = GetComponentsInChildren<Rigidbody>();
        rbInitPos = new Dictionary<Rigidbody, Vector3>();
        foreach (Rigidbody rb in rbs)
        {
            rb.mass *= 1.4f;
            rbInitPos[rb] = rb.position;
        }
        initPos = AverageRBPos(rbs);
        typeforce = new Vector3(0, 0, 0);
        gameObject.SetActive(true);
        fullGovernmentColor = GetComponentInChildren<Renderer>().material.mainTexture.name;
        color = fullGovernmentColor.Substring(0, fullGovernmentColor.IndexOf("M"));
        softlockTimer = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Rigidbody rb in rbs)
        {
            rb.AddForce(typeforce);
            if (fly && rb.velocity.magnitude <= 2)
            {
                softlockTimer -= Time.deltaTime;
            }
            else
            {
                softlockTimer = 1.5f;
            }
        }
            if (softlockTimer <= 0)
            {
                CreateMonkey(this);
            }
        //the monkeys harness the power of the typeforce...
        //(it's just some added forces for each type)
        //(from the type attributes in MBNamespace)
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (!GeometryUtility.TestPlanesAABB(planes, GetComponent<Renderer>().bounds)) CreateMonkey(this);
        //man why does onbecameinvisible not work anymore
        //i guess this would be better in the long run bc it works in case we'd want more cameras

    }

    //so you don't softlock if it rolls off screen
    //this is now entirely unnecessary because it literally can't roll off screen (there are walls)
    void OnBecameInvisible()
    {
        CreateMonkey(this);
    }
}
