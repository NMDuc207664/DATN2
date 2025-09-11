// using System.Collections.Generic;
// using DATN2.Assets.Scripts.Logics.Interface;
// using UnityEngine;

// public class CameraService : ICameraService
// {
//     private float _xRotation = 0f;
//     public Transform _playerTransform;   // thân nhân vật (để xoay ngang)
//     public Camera _playerCamera;    // mắt (để xoay dọc)
//     private readonly GameObject _player;
//     public CameraService(Transform playerTransform, Camera playerCamera, Dictionary<string, GameObject> objects)
//     {
//         _playerTransform = playerTransform;
//         _playerCamera = playerCamera;
//         _player = objects["Player"];

//     }
//     public void LockCursor(bool isLocked)
//     {
//         Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
//         Cursor.visible = !isLocked;
//     }


//     public void RotateCamera(float mouseX, float mouseY, float sensitivity)
//     {
//         // xử lý pitch (xoay dọc)
//         _xRotation -= mouseY * sensitivity * Time.deltaTime;
//         _xRotation = Mathf.Clamp(_xRotation, -45f, 60f);
//         _playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

//         // xử lý yaw (xoay ngang)
//         _playerTransform.Rotate(Vector3.up * mouseX * sensitivity * Time.deltaTime);
//         //Transform head = _player.transform.Find("root/torso/head");
//         // xử lý yaw (xoay ngang) cho player body
//         //head.Rotate(Vector3.up * mouseX * sensitivity * Time.deltaTime);
//         //xử lý pitch (xoay dọc) cho player head
//         //head.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
//     }
// }

using System.Collections.Generic;
using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;

public class CameraService : ICameraService
{
    private float _xRotation = 0f;
    public Transform _playerTransform; // Root (xoay yaw)
    public Camera _playerCamera; // Camera (xoay pitch)
    private readonly GameObject _player;
    private readonly Rigidbody _rigidbody; // Inject thêm Rigidbody từ container
    private bool _hasCollision = false;

    public CameraService(Transform playerTransform, Camera playerCamera, Dictionary<string, GameObject> objects, Rigidbody rigidbody)
    {
        _playerTransform = playerTransform;
        _playerCamera = playerCamera;
        _player = objects["Player"];
        _rigidbody = rigidbody;
        RegisterCollisionEvents();
    }
    private void RegisterCollisionEvents()
    {
        // Thêm collision detector component nếu chưa có
        var collisionDetector = _player.GetComponent<CollisionDetector>();
        if (collisionDetector == null)
        {
            collisionDetector = _player.AddComponent<CollisionDetector>();
        }

        collisionDetector.OnCollisionDetected += (isColliding) => _hasCollision = isColliding;
    }
    // ... (giữ nguyên LockCursor nếu có)

    public void RotateCamera(float mouseX, float mouseY, float sensitivity)
    {
        // Pitch (xoay dọc) - không ảnh hưởng physics
        _xRotation -= mouseY * sensitivity * Time.deltaTime;
        _xRotation = Mathf.Clamp(_xRotation, -45f, 60f);
        _playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        Transform head = _player.transform.Find("root/torso/head");
        if (head != null)
        {
            head.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }

        // Yaw (xoay ngang) - xử lý khác nhau tùy theo trạng thái collision
        if (Mathf.Abs(mouseX) > 0.01f)
        {
            float yawDelta = mouseX * sensitivity * Time.deltaTime;

            if (_hasCollision)
            {
                // Khi va chạm: sử dụng transform rotation trực tiếp để tránh jitter
                _playerTransform.Rotate(0f, yawDelta, 0f);
                // Sync lại rigidbody rotation
                _rigidbody.rotation = _playerTransform.rotation;
            }
            else
            {
                // Khi không va chạm: dùng physics rotation bình thường
                Quaternion yawRotation = Quaternion.Euler(0f, yawDelta, 0f);
                _rigidbody.MoveRotation(_rigidbody.rotation * yawRotation);
            }
        }
    }

    public void LockCursor(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }
}
public class CollisionDetector : MonoBehaviour
{
    public System.Action<bool> OnCollisionDetected;
    private bool _isColliding = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            _isColliding = true;
            OnCollisionDetected?.Invoke(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            _isColliding = false;
            OnCollisionDetected?.Invoke(false);
        }
    }
}