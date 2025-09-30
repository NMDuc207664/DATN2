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
    void Update()
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