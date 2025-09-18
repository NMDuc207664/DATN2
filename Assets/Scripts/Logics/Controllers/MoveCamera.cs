// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MoveCamera : MonoBehaviour
// {
//     public Transform cameraPosition; // Vị trí gốc của camera (thường là đầu nhân vật)
//     public Transform playerTransform; // Transform của nhân vật (để lấy rotation yaw)
//     public Camera playerCamera; // Camera để đồng bộ rotation
//     public Vector3 cameraOffset; // Offset tương đối từ nhân vật đến camera (có thể điều chỉnh trong Inspector)

//     void Start()
//     {
//         // Nếu không gán offset trong Inspector, mặc định đặt camera trước nhân vật
//         if (cameraOffset == Vector3.zero)
//         {
//             cameraOffset = new Vector3(0f, 0f, -1f); // Ví dụ: camera cách nhân vật 1 đơn vị phía trước
//         }
//     }

//     void Update()
//     {
//         // Tính toán vị trí camera dựa trên rotation của playerTransform
//         Vector3 desiredPosition = cameraPosition.position + playerTransform.TransformDirection(cameraOffset);
//         transform.position = desiredPosition;

//         // Đồng bộ rotation của camera với playerCamera
//         transform.rotation = playerCamera.transform.rotation;
//     }
// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    public Transform playerTransform;
    public Camera playerCamera;
    public Vector3 cameraOffset;

    // Smooth follow parameters
    [SerializeField] private float followSmoothness = 10f;
    [SerializeField] private bool useSmoothing = true;

    void Start()
    {
        if (cameraOffset == Vector3.zero)
        {
            cameraOffset = new Vector3(0f, 0f, -1f);
        }
    }

    // Sử dụng LateUpdate để đảm bảo camera được cập nhật sau tất cả movement
    void LateUpdate()
    {
        // Tính toán vị trí mong muốn
        Vector3 desiredPosition = cameraPosition.position + playerTransform.TransformDirection(cameraOffset);

        if (useSmoothing)
        {
            // Smooth camera movement
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSmoothness * Time.deltaTime);

            // Smooth rotation sync
            transform.rotation = Quaternion.Slerp(transform.rotation, playerCamera.transform.rotation, followSmoothness * Time.deltaTime);
        }
        else
        {
            // Direct positioning (có thể gây giật)
            transform.position = desiredPosition;
            transform.rotation = playerCamera.transform.rotation;
        }
    }
}