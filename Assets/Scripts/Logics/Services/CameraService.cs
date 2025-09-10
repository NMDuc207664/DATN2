using System.Collections.Generic;
using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;

public class CameraService : ICameraService
{
    private float _xRotation = 0f;
    public Transform _playerTransform;   // thân nhân vật (để xoay ngang)
    public Camera _playerCamera;    // mắt (để xoay dọc)
    private readonly GameObject _player;
    public CameraService(Transform playerTransform, Camera playerCamera, Dictionary<string, GameObject> objects)
    {
        _playerTransform = playerTransform;
        _playerCamera = playerCamera;
        _player = objects["Player"];

    }
    public void LockCursor(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }


    // public void RotateCamera(float mouseX, float mouseY, float sensitivity)
    // {
    //     // xử lý pitch (xoay dọc)
    //     _xRotation -= mouseY * sensitivity * Time.deltaTime;
    //     _xRotation = Mathf.Clamp(_xRotation, -45f, 60f);
    //     _playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

    //     // xử lý yaw (xoay ngang)
    //     _playerTransform.Rotate(Vector3.up * mouseX * sensitivity * Time.deltaTime);
    //     Transform head = _player.transform.Find("root/torso/head");
    //     // xử lý yaw (xoay ngang) cho player body
    //     head.Rotate(Vector3.up * mouseX * sensitivity * Time.deltaTime);
    //     //xử lý pitch (xoay dọc) cho player head
    //     head.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    // }
    public void RotateCamera(float mouseX, float mouseY, float sensitivity)
    {
        // Pitch (xoay dọc) - cho Camera và Head
        _xRotation -= mouseY * sensitivity * Time.deltaTime;
        _xRotation = Mathf.Clamp(_xRotation, -45f, 60f);

        // Xoay Camera theo pitch
        _playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        // Xoay Head theo pitch
        Transform head = _player.transform.Find("root/torso/head");
        if (head != null)
        {
            head.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }
        Debug.Log(head);

        // Yaw (xoay ngang) - cho toàn thân
        _playerTransform.Rotate(Vector3.up * mouseX * sensitivity * Time.deltaTime);
    }
}
