using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleEnd : MonoBehaviour
{
    private IEnumerator OnTriggerEnter(Collider obj)
    {
        if(obj.gameObject.CompareTag("Obstacle"))
        {
            GameManager.dodged++;
            GameObject.Destroy(obj.gameObject);
            yield return null;
        }
    }
}
