using System.Collections.Generic;
using Towy.Utilities;
using System;

[Serializable]
public class GameSaveData : IDataStore
{
    public float bestMeters;
    
    public void Init()
    {
        bestMeters = 0f;
    }

    public void PostLoad()
    {
    }

    public void PreSave()
    {
    }
}