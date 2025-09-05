using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistent
{
    void LoadData(SaveModel saveModel);
    void SaveData(ref SaveModel saveModel);
}
