using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MBNamespace.MBFunctions;

//i've been wanting to do this for a while...
public class DeathZone : MonoBehaviour 
{

    private void Start()
    {

    }

    //technically the death zones respawn the monkeys which i think is pretty funny
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<MenuSpawnerMonkey>())
        {
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.GetComponentInParent<Monkey>())
        {
            Monkey monkey = collision.gameObject.GetComponentInParent<Monkey>();
                PointManager.instance.Score(-monkey.pointValue * 5);
            CreateMonkey(monkey);
            return;
        }
    }
}
