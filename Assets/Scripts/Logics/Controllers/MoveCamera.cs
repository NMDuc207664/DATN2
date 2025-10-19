// using Cinemachine;
// using UnityEngine;

// public class MoveCamera : MonoBehaviour
// {
//     public Transform cameraPosition;
//     public Transform playerTransform;
//     public Camera playerCamera;
//     public Vector3 cameraOffset;

//     // Smooth follow parameters
//     [SerializeField] private float followSmoothness = 10f;
//     [SerializeField] private bool useSmoothing = true;

//     [Header("Virtual Camera")]
//     // [SerializeField] private CinemachineVirtualCamera virtualCamera;
//     // [SerializeField] private bool useVirtualCamera = true;
//     private Vector3 velocity = Vector3.zero;
//     void Start()
//     {
//         if (cameraOffset == Vector3.zero)
//         {
//             cameraOffset = new Vector3(0f, 0f, -1f);
//         }
//         if (playerCamera != null)
//         {
//             playerCamera.transform.position = new Vector3(0f, 0f, 0f);
//         }
//     }




//     void Update()
//     {
//         if (cameraPosition == null) return;

//         Vector3 desiredPosition = cameraPosition.position + playerTransform.TransformDirection(cameraOffset);
//         float smoothTime = 1f / followSmoothness; // followSmoothness = độ nhạy (càng cao càng nhanh)

//         if (useSmoothing)
//         {
//             transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

//             // if (!useVirtualCamera && playerCamera != null)
//             // {
//             //     transform.rotation = Quaternion.Slerp(transform.rotation, playerCamera.transform.rotation, followSmoothness * Time.deltaTime);
//             // }
//         }
//         else
//         {
//             transform.position = desiredPosition;
//             // if (!useVirtualCamera && playerCamera != null)
//             //     transform.rotation = playerCamera.transform.rotation;
//         }
//     }
// }


using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    public Transform playerTransform;
    public Vector3 cameraOffset = new Vector3(0f, 0f, -1f);
    [SerializeField] private float followSmoothness = 10f;
    [SerializeField] private bool useSmoothing = true;

    private Vector3 velocity;

    private void LateUpdate()
    {
        if (cameraPosition == null) return;

        Vector3 desiredPos = cameraPosition.position + playerTransform.TransformDirection(cameraOffset);
        float smoothTime = 1f / followSmoothness;

        if (useSmoothing)
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);
        else
            transform.position = desiredPos;
    }
}
