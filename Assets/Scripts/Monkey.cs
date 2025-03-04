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

    public Rigidbody rb { get; set; }

    public MONKEYTYPE type;
    public Color color { get; set; }

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
        color = GetComponent<Renderer>().material.color;
        initPos = rb.position;
        typeforce = new Vector3(0, 0, 0);
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(typeforce);
        //the monkeys harness the power of the typeforce...
        //(it's just some added forces for each type)
        //(from the type attributes in MBNamespace)
        //doesn't do anything currently, all that stuff is commented out
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (!GeometryUtility.TestPlanesAABB(planes, GetComponent<Renderer>().bounds)) CreateMonkey(this);
        //man why does onbecameinvisible not work anymore
        //i guess this would be better in the long run bc it works in case we'd want more cameras
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Barrel>()) //no need to loop now!
        {
            Barrel hitBarrel = collision.gameObject.GetComponent<Barrel>();
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
    void OnBecameInvisible()
    {
        CreateMonkey(this);
    }
}
