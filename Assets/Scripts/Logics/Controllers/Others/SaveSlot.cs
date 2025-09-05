using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class SaveSlot : MonoBehaviour
    {
        [SerializeField] private string profileId;
        // [SerializeField] private string date;
        // [SerializeField] private string playTime;
        [SerializeField] private GameObject noDataContent;
        [SerializeField] private GameObject hasDataContent;
        [SerializeField] private TextMeshProUGUI saveName;
        [SerializeField] private TextMeshProUGUI saveDate;
        [SerializeField] private TextMeshProUGUI playTime;
        public void SetData(SaveModel saveModel)
        {
            if (saveModel == null)
            {
                noDataContent.SetActive(true);
                hasDataContent.SetActive(false);
            }
            else
            {
                noDataContent.SetActive(false);
                hasDataContent.SetActive(true);
                saveName.text = saveModel.SaveName;
                saveDate.text = TimeFormatterExtension.FormatSaveTime(saveModel.Time);
                // playTime.text = saveModel.PlayTime;
            }
        }
        public string GetProfileId()
        {
            return profileId;
        }
    }
}