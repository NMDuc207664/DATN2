using UnityEngine;
using VContainer;
using VContainer.Unity;
using DATN2.Assets.Scripts.Logics.Interface;
using DATN2.Assets.Scripts.Logics.Controllers;

// Tạo một EntryPoint để inject services vào tất cả DoorInteraction dùng khi có 1 component dùng nhiều trong object trong scene
public class DoorEntryPoint : IStartable
{
    private readonly IInventoryService inventoryService;
    private readonly ISceneService sceneService;

    [Inject]
    public DoorEntryPoint(IInventoryService inventoryService, ISceneService sceneService)
    {
        this.inventoryService = inventoryService;
        this.sceneService = sceneService;
    }

    public void Start()
    {
        // Tìm tất cả DoorInteraction trong scene và inject service
        var doorInteractions = Object.FindObjectsOfType<DoorInteraction>();
        foreach (var door in doorInteractions)
        {
            door.SetInventoryService(inventoryService, sceneService);
        }
        Debug.Log($"[DoorEntryPoint] Injected InventoryService into {doorInteractions.Length} doors");
    }
}