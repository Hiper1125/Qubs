using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoaderUI : MonoBehaviour
{
    private PlayerController player;
    private TextMeshProUGUI loaderTxt;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        loaderTxt = GetComponent<TextMeshProUGUI>();
    }

    private void FixedUpdate()
    {
        loaderTxt.text = string.Concat(player.cAmmo, '/', player.loaderSize);
    }
}
