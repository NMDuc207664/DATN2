using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DATN2.GraphviewEditor
{
    public class DTSDialogueManager : MonoBehaviour
    {
        public static DTSDialogueManager Instance { get; set; }
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}