using DATN2.Assets.Scripts.Data;
using UnityEngine;

public class DefaultDialogue : MonoBehaviour
{
    public QuestDataSO _questData;
    public int _questIndex;

    public void Setup(QuestDataSO questData, int questIndex)
    {
        _questData = questData;
        _questIndex = questIndex;

        Debug.Log($"[DefaultDialogue:{gameObject.name}] Setup done with QuestData = {questData.Key}, QuestIndex = {_questIndex}");

        // Thử debug nội dung bên trong quest
        if (_questData.quests != null && _questData.quests.Count > _questIndex)
        {
            var q = _questData.quests[_questIndex];
            Debug.Log($"   ▶ HasDefaultDialogue: {q.HasDefaultDialogue}");
            Debug.Log($"   ▶ DialogueGraph: {q.dialogues?.dialogueGraph}");
        }
    }
}
