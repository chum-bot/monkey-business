using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBNamespace;

//just a cylinder wth a color... for now
//also lets me check if a barrel is in fact a barrel and not just a cylinder with a color
public class Barrel : MonoBehaviour
{
    [SerializeField]
    public MBVars.SORTING sortingMetric; 
    public Rigidbody rb { get; set; }
    public Color color { get; set; }

    [SerializeField]
    public int scoreMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        color = GetComponent<Renderer>().material.color;
    }
}
