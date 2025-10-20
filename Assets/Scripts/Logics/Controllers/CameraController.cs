using Cinemachine;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        public Transform playerTransform; // Transform của player
        public Transform cameraPosition;  // Vị trí camera (có thể là một điểm trên đầu player)

        private CinemachineVirtualCamera virtualCamera;

        [Header("Look Targets")]
        public Transform tieuThuTarget;
        public Transform gaGiangHoTarget;

        [Header("Camera Settings")]
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 0.5f, -1f); // Offset so với player
        [SerializeField] private float followSmoothness = 10f; // Độ mượt khi theo dõi
        [SerializeField] private bool useSmoothing = true;      // Bật/tắt smoothing
        [SerializeField] private float mouseSensitivity = 100f; // Độ nhạy chuột
        [SerializeField] private float maxLookAngle = 80f;     // Góc nhìn tối đa lên/xuống

        private float yaw = 0f;   // Góc quay ngang (yaw)
        private float pitch = 0f; // Góc quay dọc (pitch)
        private Vector3 velocity; // Vận tốc cho SmoothDamp

        private void Awake()
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Start()
        {
            // Khóa con trỏ chuột vào giữa màn hình
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Khởi tạo góc quay ban đầu dựa trên hướng của player
            if (playerTransform != null)
            {
                yaw = playerTransform.eulerAngles.y;
            }
        }

        private void LateUpdate()
        {
            if (playerTransform == null || cameraPosition == null) return;



            if (KeyGameStateManager.Instance.LockMouseInput == false)
            {
                // Xử lý input chuột để quay camera
                HandleRotation();

            }
            Vector3 desiredPos = cameraPosition.position + playerTransform.TransformDirection(cameraOffset);

            // Cập nhật vị trí camera
            float smoothTime = 1f / followSmoothness;
            if (useSmoothing)
            {
                transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);
            }
            else
            {
                transform.position = desiredPos;
            }

            // Cập nhật góc quay của camera
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            UpdateVirtualCameraTarget();
        }

        private void HandleRotation()
        {
            // Lấy input từ chuột
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // Cập nhật góc quay
            yaw += mouseX;
            pitch -= mouseY;

            // Giới hạn góc nhìn dọc (pitch) để tránh lật camera
            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            // Quay player theo trục Y (yaw)
            if (playerTransform != null)
            {
                playerTransform.rotation = Quaternion.Euler(0f, yaw, 0f);
            }
        }
        private void UpdateVirtualCameraTarget()
        {
            if (virtualCamera == null) return;

            var gsm = KeyGameStateManager.Instance;

            // 🔄 Chọn LookAt dựa theo trạng thái
            if (gsm.LookAtTieuThu && tieuThuTarget != null)
            {
                virtualCamera.LookAt = tieuThuTarget;
                KeyGameStateManager.Instance.SetLockMouseInput(true);
            }
            else if (gsm.LookAtGaGiangHo && gaGiangHoTarget != null)
            {
                virtualCamera.LookAt = gaGiangHoTarget;
                KeyGameStateManager.Instance.SetLockMouseInput(true);
            }
            else
            {
                // Mặc định nhìn theo player
                virtualCamera.LookAt = null;
                KeyGameStateManager.Instance.SetLockMouseInput(false);
            }
        }
    }
}