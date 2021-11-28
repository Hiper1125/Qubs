using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using Towy.Utilities.Audio;

public class PlayerController : MonoBehaviour
{
    private TextMeshProUGUI hint;
    private GameObject loader;
    [Header("Sounds")] public AudioClip shootSfx;

    [Range(0, 100)] public float shootVolume = 100f;
    public AudioClip reloadSfx;

    [Range(0, 100)] public float reloadVolume = 100f;

    [Header("Player Stats")] [Range(0.01f, 0.2f)]
    public float plSpeed;

    private float border = 1.5f;

    [Header("Weapon Stats")] public GameObject bullet;
    [Range(0, 20)] public int loaderSize;
    public int cAmmo;
    [Range(1, 10)] public float reloadSpeed;

    private void Awake()
    {
        cAmmo = loaderSize;
        hint = GameObject.FindGameObjectWithTag("Hint").GetComponent<TextMeshProUGUI>();
        loader = GameObject.FindGameObjectWithTag("Loader");
        loader.SetActive(false);
    }

    void FixedUpdate()
    {
        if (GameManager.isStopped) return;

        float direction = Input.GetAxis("Horizontal");

        if (direction < 0)
        {
            transform.position = transform.position.x - plSpeed < -(border)
                ? transform.position
                : new Vector3(transform.position.x - plSpeed, transform.position.y, transform.position.z);
        }
        else if (direction > 0)
        {
            transform.position = transform.position.x + plSpeed > border
                ? transform.position
                : new Vector3(transform.position.x + plSpeed, transform.position.y, transform.position.z);
        }
    }

    void OnMouseDrag()
    {
        if (GameManager.isStopped) return;

        var position = transform.position;
        float distance_to_screen = Camera.main.WorldToScreenPoint(position).z;
        position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, position.y, distance_to_screen));

        float xPos = Math.Abs(transform.position.x - position.x);

        if (transform.position.x - position.x > 0)
        {
            if (transform.position.x - xPos > (-border))
            {
                transform.position = new Vector3(position.x, 0.3f, -3.8f);
            }
        }
        else
        {
            if (transform.position.x + xPos < border)
            {
                transform.position = new Vector3(position.x, 0.3f, -3.8f);
            }
        }
    }

    void Update()
    {
        if (GameManager.isStopped) return;

        if (!loader.activeInHierarchy)
        {
            loader.SetActive(true);
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetButtonDown("Jump")) && cAmmo > 0)
        {
            StopAllCoroutines();

            GameObject.Instantiate(bullet,
                new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

            AudioManager.PlaySound(shootSfx, shootVolume);

            cAmmo -= 1;

            if (cAmmo == 0)
            {
                StopAllCoroutines();
                StartCoroutine(ReloadLoader());
            }
        }
    }

    public void Death()
    {
        if (FindObjectOfType<Meters>().m_Meters > GameSaveDataManager.BestMeters)
        {
            GameSaveDataManager.BestMeters = FindObjectOfType<Meters>().m_Meters;
        }

        StopAllCoroutines();
        StartCoroutine(SlowDown());
        AudioManager.StopMusic(AudioManager.lastAddIndex);
        loader.SetActive(false);
    }

    private IEnumerator ReloadLoader()
    {
        float singleBl = reloadSpeed / loaderSize;

        int loaded = cAmmo;

        yield return new WaitForSeconds(reloadSpeed / 2);

        while (loaded < loaderSize)
        {
            yield return new WaitForSeconds(singleBl / 3);
            loaded += loaded + 1 > loaderSize ? 0 : 1;
            cAmmo = loaded;

            AudioManager.PlaySound(reloadSfx, reloadVolume);
        }

        yield break;
    }

    private IEnumerator SlowDown()
    {
        while (transform.position.y > -0.3)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.01f, transform.position.z);
            yield return new WaitForSeconds(0.01f);
        }

        hint.text = "Press to continue";
        hint.gameObject.GetComponent<Hint>().selfControl = false;
        hint.gameObject.SetActive(true);

        while (!Input.GetButton("Jump") && !Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        hint.gameObject.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}