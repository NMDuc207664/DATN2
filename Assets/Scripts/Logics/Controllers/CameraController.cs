// using System.Collections;
// using System.Collections.Generic;
// using Cinemachine;
// using DATN2.Assets.Scripts.Logics.Interface;
// using DATN2.Assets.Scripts.Modals.Enum;
// using UnityEngine;
// using VContainer;
// namespace DATN2.Assets.Scripts.Logics.Controllers
// {
//     public class CameraController : MonoBehaviour
//     {
//         [Inject]
//         public ICameraService _cameraService;

//         public float senX;
//         public float senY;
//         public float rotationSmoothness = 15f;
//         [SerializeField] private bool smoothCamera = false;
//         public CinemachineVirtualCamera vcam;
//         // [SerializeField] private float sensitivity = 100f;

//         // Update is called once per frame
//         void Update()
//         {
//             if (KeyGameStateManager.Instance.IsInState(InGameActionType.None) || KeyGameStateManager.Instance.IsInState(InGameActionType.Interact))

//                 HandleCameraInput();

//         }
//         private void HandleCameraInput()
//         {
//             // if (!InGameControlStateManager.Instance.IsInState(InGameActionType.None))
//             // {
//             //     return;
//             // }
//             if (SimpleCinemachineLook.Instance != null &&
//             !SimpleCinemachineLook.Instance.IsMouseInputEnabled())
//             {
//                 return; // Không xử lý input nếu đã tắt
//             }
//             float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * senX;
//             float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * senY;

//             // GameStateInvoker.TryInvoke(_cameraService, nameof(_cameraService.RotateCamera), mouseX, mouseY, senX, smoothCamera, rotationSmoothness);
//             _cameraService.RotateCamera(mouseX, mouseY, 1f, smoothCamera, rotationSmoothness);
//         }

//     }
// }


using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        public Transform playerTransform; // Transform của player
        public Transform cameraPosition;  // Vị trí camera (có thể là một điểm trên đầu player)

        [Header("Camera Settings")]
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 0.5f, -1f); // Offset so với player
        [SerializeField] private float followSmoothness = 10f; // Độ mượt khi theo dõi
        [SerializeField] private bool useSmoothing = true;      // Bật/tắt smoothing
        [SerializeField] private float mouseSensitivity = 100f; // Độ nhạy chuột
        [SerializeField] private float maxLookAngle = 80f;     // Góc nhìn tối đa lên/xuống

        private float yaw = 0f;   // Góc quay ngang (yaw)
        private float pitch = 0f; // Góc quay dọc (pitch)
        private Vector3 velocity; // Vận tốc cho SmoothDamp

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

            // Xử lý input chuột để quay camera
            HandleRotation();

            // Tính vị trí mong muốn của camera
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

        // private void OnDestroy()
        // {
        //     // Mở khóa con trỏ chuột khi thoát
        //     Cursor.lockState = CursorLockMode.None;
        //     Cursor.visible = true;
        // }
    }
}