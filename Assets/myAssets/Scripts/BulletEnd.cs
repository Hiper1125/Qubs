using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider obj)
    {
        if(obj.gameObject.CompareTag("Bullet"))
        {
            GameObject.Destroy(obj.gameObject);
        }
    }
}
