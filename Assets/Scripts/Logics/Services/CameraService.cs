using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;

public class CameraService : ICameraService
{
    public Transform _playerTransform;   // thân nhân vật (để xoay ngang)
    public Camera _playerCamera;    // mắt (để xoay dọc)
    public CameraService(Transform playerTransform, Camera playerCamera)
    {
        _playerTransform = playerTransform;
        _playerCamera = playerCamera;

    }
    public void LockCursor(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }

    public void RotateCamera(ref float xRotation, float mouseX, float mouseY, float sensitivity)
    {
        xRotation -= mouseY * sensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        _playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // xử lý yaw (xoay ngang) cho player body
        _playerTransform.Rotate(Vector3.up * mouseX * sensitivity * Time.deltaTime);
    }
}
