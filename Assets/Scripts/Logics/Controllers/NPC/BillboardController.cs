using UnityEngine;

public class BillboardController : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private bool smoothRotation = true;
    [SerializeField] private float rotationSpeed = 8f;

    private void LateUpdate()
    {
        if (playerCamera == null) return;

        // Lấy hướng từ NPC -> camera (ngược lại để NPC "nhìn" camera)
        Vector3 directionToCamera = playerCamera.position - transform.position;
        directionToCamera.y = 0f; // Giữ NPC đứng thẳng (không ngửa cổ)

        // Tính hướng quay
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

        // Xoay mượt hoặc lập tức
        if (smoothRotation)
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        else
            transform.rotation = targetRotation;
    }

}
