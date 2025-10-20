using System;
using UnityEngine;
[Serializable]
public class SaveModel
{
    [SerializeField] public string SaveId;
    [SerializeField] public string SaveName;
    [SerializeField] public string Time;
    [SerializeField] public string SceneName;
    [SerializeField] public string[] Item;
    [SerializeField] public Vector3 PlayerPosition;
    [SerializeField] public Vector3 PlayerRotation;
    [SerializeField] public Vector3 PlayerScale;

}
