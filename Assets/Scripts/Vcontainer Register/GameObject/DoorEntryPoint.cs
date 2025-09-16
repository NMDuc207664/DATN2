using UnityEngine;
using VContainer;
using VContainer.Unity;
using DATN2.Assets.Scripts.Logics.Interface;

// Tạo một EntryPoint để inject services vào tất cả DoorInteraction dùng khi có 1 component dùng nhiều trong object trong scene
public class DoorEntryPoint : IStartable
{
    private readonly IInventoryService inventoryService;

    [Inject]
    public DoorEntryPoint(IInventoryService inventoryService)
    {
        this.inventoryService = inventoryService;
    }

    public void Start()
    {
        // Tìm tất cả DoorInteraction trong scene và inject service
        var doorInteractions = Object.FindObjectsOfType<DoorInteraction>();
        foreach (var door in doorInteractions)
        {
            door.SetInventoryService(inventoryService);
        }
        Debug.Log($"[DoorEntryPoint] Injected InventoryService into {doorInteractions.Length} doors");
    }
}