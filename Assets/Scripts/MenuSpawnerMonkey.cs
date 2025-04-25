using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBNamespace;
using UnityEngine.UI;
using static MBNamespace.MBFunctions;
using static MBNamespace.MBVars;
using System;

public class MenuSpawnerMonkey : MonoBehaviour
{

    [SerializeField]
    public float speed = 1.0f;
    public Rigidbody[] rbs { get; set; }
    public string color { get; set; }

    private string fullGovernmentColor;

    public Dictionary<Rigidbody, Vector3> rbInitPos;

    //so when you grab the monkey all its pieces are spaced out an it doesn't scrunch up
    public Dictionary<Rigidbody, Vector3> rbPosDiff;

    public Vector3 truePosition { get; set; }

    //getting the average positon of all the rigidbodies in the ragdoll
    //so i have a singular point to reference for the monkey's initPos
    //actually completely unused lol, but i feel like it'd be useful
    public Vector3 AverageRBPos(Rigidbody[] positions)
    {
        Vector3 avg = Vector3.zero;
        foreach (Rigidbody rb in positions)
        {
            avg += rb.position;
        }
        return avg / positions.Length;
    }

    public void UpdateRBRelations()
    {
        foreach (Rigidbody rb in rbs)
        {
            rbPosDiff[rb] = rb.position - truePosition;
        }
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
        rbPosDiff = new Dictionary<Rigidbody, Vector3>();
        truePosition = AverageRBPos(rbs);
        foreach (Rigidbody rb in rbs)
        {
            rbInitPos[rb] = rb.position;
            rbPosDiff[rb] = rb.position - truePosition;
        }
        gameObject.SetActive(true);
        fullGovernmentColor = GetComponentInChildren<Renderer>().material.mainTexture.name;
        color = fullGovernmentColor.Substring(0, fullGovernmentColor.IndexOf("M"));
    }
}
