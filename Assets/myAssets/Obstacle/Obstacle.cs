using System.Collections;
using Towy.Utilities.Audio;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public ObstacleType type;
    [Range(0, 1)] public float speed;
    private float border = 2.3f;
    private float baseScale = 3.5f;
    private PlayerController player;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        if (type == ObstacleType.Dynamic)
        {
            speed += (speed + ((float) GameManager.dodged / 1000f)) < 0.7f ? ((float) GameManager.dodged / 1000f) : 0;
        }
        else if (type == ObstacleType.Static || type == ObstacleType.Growing)
        {
            speed += (speed + ((float) GameManager.dodged / 1000f)) < 0.25f ? ((float) GameManager.dodged / 1000f) : 0;
        }

        switch (type)
        {
            case ObstacleType.Static:
                StartCoroutine(StaticMove());
                break;
            case ObstacleType.Dynamic:
                StartCoroutine(DynamicMove());
                break;
            case ObstacleType.Growing:
                StartCoroutine(GrowingMove());
                break;
        }
    }

    private void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.CompareTag("Player"))
        {
            GameManager.isStopped = true;
            player.Death();
            GameObject.Destroy(this.gameObject);
        }
        else if (obj.gameObject.CompareTag("Bullet"))
        {
            GameObject.Destroy(obj.gameObject);
            GameManager.dodged++;
            var gManager = FindObjectOfType<GameManager>();
            AudioManager.PlaySound(gManager.hitSfx, gManager.hitVolume);
            GameObject.Destroy(this.gameObject);
        }
        else if (obj.gameObject.CompareTag("End"))
        {
            GameManager.dodged++;
            GameObject.Destroy(this.gameObject);
        }
    }

    private IEnumerator StaticMove()
    {
        while (!GameManager.isStopped)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - speed);
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator DynamicMove()
    {
        StartCoroutine(StaticMove());

        bool direction = transform.position.x > -(border) ? true : false;

        while (!GameManager.isStopped)
        {
            if (transform.position.x >= -(border) && direction == false)
            {
                transform.position =
                    new Vector3(transform.position.x - speed, transform.position.y, transform.position.z);
            }
            else if (transform.position.x <= -(border) && direction == false)
            {
                direction = true;
            }
            else if (direction == true && transform.position.x <= border)
            {
                transform.position =
                    new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
            }
            else if (transform.position.x >= border && direction == true)
            {
                direction = false;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator GrowingMove()
    {
        StartCoroutine(StaticMove());

        bool growed = false;

        yield return new WaitForSeconds(GameManager.timeBtwSpawn / 2);

        while (!GameManager.isStopped)
        {
            if (transform.localScale.x <= baseScale && growed == false)
            {
                transform.localScale = new Vector3(transform.localScale.x + (speed / 10), transform.localScale.y,
                    transform.localScale.z);
            }
            else if (transform.localScale.x >= baseScale && growed == false)
            {
                growed = true;
            }
            else if (transform.localScale.x >= 0.5f && growed == true)
            {
                transform.localScale = new Vector3(transform.localScale.x - (speed / 5), transform.localScale.y,
                    transform.localScale.z);
            }
            else if (transform.localScale.x <= 0.5f && growed == true)
            {
                growed = false;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}