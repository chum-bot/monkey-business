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

    public Rigidbody rb { get; set; }

    public MONKEYTYPE type;
    public string color { get; set; }

    private string fullGovernmentColor;

    private float softlockTimer;

    public Vector3 initPos { get; set; }
    public Vector3 typeforce { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        //it'll just be the color for now
        //we can change this later once the models have more stuff going on
        //i'm taking directly from it now so i don't have to hardcode colors in
        //and we can easily make new ones w/ materials
        rb = GetComponent<Rigidbody>();
        initPos = rb.position;
        typeforce = new Vector3(0, 0, 0);
        gameObject.SetActive(true);
        fullGovernmentColor = GetComponentInChildren<Renderer>().material.mainTexture.name;
        color = fullGovernmentColor.Substring(0, fullGovernmentColor.IndexOf("M"));
        softlockTimer = 2;
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(typeforce);
        //the monkeys harness the power of the typeforce...
        //(it's just some added forces for each type)
        //(from the type attributes in MBNamespace)
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (!GeometryUtility.TestPlanesAABB(planes, GetComponent<Renderer>().bounds)) CreateMonkey(this);
        //man why does onbecameinvisible not work anymore
        //i guess this would be better in the long run bc it works in case we'd want more cameras

        if (fly && rb.velocity.magnitude == 0)
        {
            softlockTimer -= Time.deltaTime;
        }
        else
        {
            softlockTimer = 2;
        }
        if(softlockTimer <= 0)
        {
            CreateMonkey(this);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent<Barrel>()) //no need to loop now!
        {
            Barrel hitBarrel = collider.gameObject.GetComponent<Barrel>();
            if (IsSorted(this, hitBarrel))
            {
                PointManager.instance.Score((pointValue  + comboCount) * hitBarrel.scoreMultiplier);
            }
            else
            {
                PointManager.instance.Score(-pointValue);
                comboCount = 0;
            }
            CreateMonkey(this);
            return;
        }
    }

    //so you don't softlock if it rolls off screen
    //this is now entirely unnecessary because it literally can't roll off screen (there are walls)
    void OnBecameInvisible()
    {
        CreateMonkey(this);
    }
}
