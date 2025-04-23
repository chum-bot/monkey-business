using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MBNamespace.MBFunctions;

//i've been wanting to do this for a while...
public class DeathZone : MonoBehaviour 
{
    public Vector3 start { get; set; }

    private void Start()
    {
        start = transform.TransformPoint(new Vector3(5, 0, 0));
        Physics.gravity *= 2;
    }

    //technically the death zones respawn the monkeys which i think is pretty funny
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<Monkey>())
        {
            Monkey monkey = collision.gameObject.GetComponentInParent<Monkey>();
            PointManager.instance.Score(-monkey.pointValue);
            CreateMonkey(monkey);
            return;
        }
    }
}
