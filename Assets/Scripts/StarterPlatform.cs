using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<Monkey>())
        {
            collision.gameObject.GetComponentInParent<Monkey>().fly = false;
        }
    }
}
