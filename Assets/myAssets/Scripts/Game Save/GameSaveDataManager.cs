using System.Collections.Generic;
using Towy.Utilities;
using UnityEngine;

public class GameSaveDataManager : Singleton<GameSaveDataManager>
{
    private GameSaveData m_Data;

    private void LoadData()
    {
        if (m_Data == null)
        {
            m_Data = GameSaveManager.LoadData<GameSaveData>("savegame");
        }
    }

    public static float BestMeters
    {
        get
        {
            Instance.LoadData();
            return Instance.m_Data.bestMeters;
        }

        set
        {
            Instance.LoadData();
            Instance.m_Data.bestMeters = value;
            Instance.SaveData();
        }
    }

    private void SaveData()
    {
        GameSaveManager.SaveData("savegame", m_Data);
    }
}
