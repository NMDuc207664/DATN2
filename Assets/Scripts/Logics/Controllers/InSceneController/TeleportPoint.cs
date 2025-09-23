using UnityEngine;

namespace DATN2.Assets.Scripts.Logics
{
    public class TeleportPoint : MonoBehaviour
    {
        [SerializeField] private Transform destination; // Điểm đích của teleport point này
        private bool isRegistered = false;

        private void Start()
        {
            TryRegisterTeleportPoint();
        }

        private void Update()
        {
            // Thử đăng ký lại nếu chưa đăng ký thành công
            if (!isRegistered)
            {
                TryRegisterTeleportPoint();
            }
        }

        private void TryRegisterTeleportPoint()
        {
            // Đăng ký cặp teleport với TeleportSystem
            if (TeleportSystem.Instance != null)
            {
                TeleportSystem.Instance.RegisterTeleportPoint(transform, destination);
                isRegistered = true;
            }
            else if (TeleportSystem.Instance == null)
            {
                Debug.LogWarning($"TeleportSystem not found for {gameObject.name}. Retrying in Update...");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Player entered teleport point.");
            // Gọi hàm Teleport từ TeleportSystem
            if (TeleportSystem.Instance != null)
            {
                TeleportSystem.Instance.Teleport(other, transform);
            }
            else
            {
                Debug.LogError("TeleportSystem not found! Please add a TeleportSystem to the scene.");
            }
        }
    }
}