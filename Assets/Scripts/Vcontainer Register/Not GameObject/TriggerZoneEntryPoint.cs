using System.Collections;
using System.Collections.Generic;
using DATN2.GraphviewEditor.Runtime;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class TriggerZoneEntryPoint : IStartable
{
    private readonly Dictionary<string, IQuestService> _questControllers;

    [Inject]
    public TriggerZoneEntryPoint(Dictionary<string, IQuestService> questControllers)
    {
        this._questControllers = questControllers;
    }

    public void Start()
    {
        // Tìm tất cả DTSTriggerZone trong scene và inject service
        var triggerZones = Object.FindObjectsOfType<DTSDialogueTriggerZone>(true);
        foreach (var zone in triggerZones)
        {
            zone.SetQuestControllers(_questControllers);
        }
        Debug.Log($"[TriggerZoneEntryPoint] Injected QuestControllers into {triggerZones.Length} trigger zones");
    }
}
