using System.Collections.Generic;
using UnityEngine;

namespace DATN2.Assets.Scripts.Logics
{
    [System.Serializable]
    public struct TeleportPair
    {
        public Transform entryPoint; // Điểm vào của teleport
        public Transform destinationPoint; // Điểm đích của teleport
    }

    public class TeleportSystem : MonoBehaviour
    {
        public static TeleportSystem Instance { get; private set; }

        [SerializeField] private List<TeleportPair> teleportPairs = new List<TeleportPair>();
        [SerializeField] private string playerTag = "Player"; // Tag của nhân vật

        private void Awake()
        {
            // Thiết lập singleton
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Giữ instance này giữa các scene
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void RegisterTeleportPoint(Transform entry, Transform destination)
        {
            if (entry == null || destination == null)
            {
                Debug.LogError($"Failed to register teleport point: Entry or Destination is null for {entry?.name ?? "null"}");
                return;
            }
            teleportPairs.Add(new TeleportPair { entryPoint = entry, destinationPoint = destination });
            Debug.Log($"Registered teleport point: {entry.name} -> {destination.name}");
        }

        public void Teleport(Collider player, Transform entry)
        {
            Debug.Log($"Attempting to teleport {player.name} from {entry.name}");
            foreach (var pair in teleportPairs)
            {
                if (pair.entryPoint == entry)
                {
                    // Kiểm tra tag của GameObject gốc
                    Transform rootTransform = player.gameObject.transform.root;
                    if (rootTransform.CompareTag(playerTag))
                    {
                        // Teleport GameObject gốc
                        rootTransform.position = pair.destinationPoint.position;
                        rootTransform.rotation = pair.destinationPoint.rotation; // Tùy chọn: điều chỉnh hướng
                        Debug.Log($"Teleported {rootTransform.name} from {entry.name} to {pair.destinationPoint.name}");
                        return;
                    }
                    else
                    {
                        Debug.LogWarning($"Root GameObject {rootTransform.name} does not have tag {playerTag}");
                    }
                    return;
                }
            }
            Debug.LogWarning($"No teleport destination found for entry point: {entry.name}");
        }

        // Hiển thị các cặp teleport trong Scene view để dễ debug
        private void OnDrawGizmos()
        {
            foreach (var pair in teleportPairs)
            {
                if (pair.entryPoint != null && pair.destinationPoint != null)
                {
                    // Vẽ hình cầu tại điểm vào
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(pair.entryPoint.position, 0.5f);

                    // Vẽ hình cầu tại điểm đích
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(pair.destinationPoint.position, 0.5f);

                    // Vẽ đường nối giữa điểm vào và điểm đích
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(pair.entryPoint.position, pair.destinationPoint.position);
                }
            }
        }
    }
}