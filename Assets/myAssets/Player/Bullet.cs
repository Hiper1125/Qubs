using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

   [Range(0.01f, 0.2f)]
    public float speed;

    private void Start()
    {
        StartCoroutine(Shooted());
    }

    private IEnumerator Shooted()
    {
        while(!GameManager.isStopped)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed);
            yield return new WaitForFixedUpdate();
        }
    }
}
