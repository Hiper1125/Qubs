using System.Collections;
using System.Collections.Generic;
using Towy.Utilities.Audio;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] obstacles;
    private const float defaultTimeBtwSpawn = 5f;
    public static float timeBtwSpawn;

    public static int dodged;

    public static bool isStopped;
    private bool isStarted;

    public int forceSpawn = -1;

    [Header("Sounds")] public AudioClip hitSfx;
    [Range(0, 100)] public float hitVolume = 100f;

    // Start is called before the first frame update
    void Start()
    {
        dodged = 0;
        isStopped = true;
        isStarted = false;
    }

    private void Update()
    {
        if (!isStarted)
        {
            isStarted = (Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0));

            if (isStarted)
            {
                isStopped = false;
                isStarted = true;
                StartCoroutine(SpawnObstacle());
            }
        }
    }

    private IEnumerator SpawnObstacle()
    {
        while (!isStopped)
        {
            GameObject.Instantiate(obstacles[forceSpawn == -1 ? Random.Range(0, obstacles.Length) : (forceSpawn - 1)],
                new Vector3(Random.Range(-1.3f, 1.3f), 1, 45), Quaternion.identity, transform);

            if (dodged < 1)
            {
                timeBtwSpawn = defaultTimeBtwSpawn;
            }
            else if (Random.Range(0, dodged) > (dodged / 2))
            {
                if ((timeBtwSpawn - ((float) dodged / 20f)) > 1.5f)
                {
                    timeBtwSpawn -= (float) dodged / 20f;
                }
                else if ((timeBtwSpawn - ((float) dodged / 100f)) > 1.5f)
                {
                    timeBtwSpawn -= (float) dodged / 100f;
                }
                else if ((timeBtwSpawn - ((float) dodged / 1000f)) > 1f)
                {
                    timeBtwSpawn -= (float) dodged / 1000f;
                }
            }

            yield return new WaitForSeconds(timeBtwSpawn);
        }
    }
}