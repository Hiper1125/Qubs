using UnityEngine;
using TMPro;

public class Meters : MonoBehaviour
{
    public TextMeshProUGUI meters;
    public TextMeshProUGUI bestMeters;
    [HideInInspector] public float m_Meters = 0f;

    void Start()
    {
        meters.text = "0M";
        bestMeters.text = "BEST " + string.Format("{0:0.00}", GameSaveDataManager.BestMeters).ToString() + "m";
    }

    void FixedUpdate()
    {
        if (!GameManager.isStopped)
        {
            m_Meters += 0.02f;
            meters.text = string.Format("{0:0.00}", m_Meters).ToString() + "m";

            if (m_Meters > GameSaveDataManager.BestMeters)
            {
                bestMeters.text = "BEST " + string.Format("{0:0.00}", m_Meters).ToString() + "m";
            }
        }
    }
}