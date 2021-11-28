using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hint : MonoBehaviour
{
    private TextMeshProUGUI hint;

    public bool selfControl = true;

    private void Awake()
    {
        hint = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if(selfControl)
        {
            StartCoroutine(HintManager());
        }
    }

    private IEnumerator HintManager()
    {
        while(!Input.GetButtonDown("Jump") && !Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        gameObject.SetActive(false);
        yield break;
    }    


}
