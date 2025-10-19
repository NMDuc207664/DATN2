using System.Collections.Generic;
using Cinemachine;
using DATN2.Assets.Scripts.Logics.Interface;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraService : ICameraService
{
    private float _xRotation = 0f;
    private float _yRotation = 0f;
    public Transform _playerTransform; // Root (xoay yaw)
    public Camera _playerCamera; // Camera (xoay pitch)
    private readonly CinemachineVirtualCamera _vcam;
    private readonly GameObject _player;
    private readonly Rigidbody _rigidbody; // Inject thêm Rigidbody từ container


    // Smooth rotation parameters
    private float _targetXRotation = 0f;
    private float _targetYRotation = 0f;

    public CameraService(Dictionary<string, Transform> transforms, Camera playerCamera, Dictionary<string, GameObject> objects, Rigidbody rigidbody, Dictionary<string, CinemachineVirtualCamera> cam)
    {
        _playerTransform = transforms["PlayerTransform"];
        _playerCamera = playerCamera;
        _player = objects["Player"];
        _rigidbody = rigidbody;
        _vcam = cam["Player_camera#1"];
        //RegisterCollisionEvents();
        Vector3 currentRotation = _playerCamera.transform.eulerAngles;
        _xRotation = _targetXRotation = currentRotation.x;
        _yRotation = _targetYRotation = currentRotation.y;
    }
    // private void RegisterCollisionEvents()
    // {
    //     // Thêm collision detector component nếu chưa có
    //     var collisionDetector = _player.GetComponent<CollisionDetector>();
    //     if (collisionDetector == null)
    //     {
    //         collisionDetector = _player.AddComponent<CollisionDetector>();
    //     }

    //     collisionDetector.OnCollisionDetected += (isColliding) => _hasCollision = isColliding;
    // }
    // ... (giữ nguyên LockCursor nếu có)

    public void RotateCamera(float mouseX, float mouseY, float sensitivity, bool smoothCamera, float rotationSmoothness)
    {

        // Pitch (xoay dọc) - không ảnh hưởng physics
        if (smoothCamera)
        {
            // Cập nhật target rotation
            _targetXRotation -= mouseY;
            _targetYRotation += mouseX;

            // Clamp pitch rotation
            _targetXRotation = Mathf.Clamp(_targetXRotation, -60f, 54f);

            // Smooth interpolation đến target rotation
            _xRotation = Mathf.LerpAngle(_xRotation, _targetXRotation, rotationSmoothness * Time.deltaTime);
            _yRotation = Mathf.LerpAngle(_yRotation, _targetYRotation, rotationSmoothness * Time.deltaTime);

            // Apply rotations
            _playerCamera.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
            _playerTransform.rotation = Quaternion.Euler(0f, _yRotation, 0f);
        }
        else
        {
            _xRotation -= mouseY;
            _yRotation += mouseX;
            _xRotation = Mathf.Clamp(_xRotation, -45f, 60f);
            _playerCamera.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
            _playerTransform.rotation = Quaternion.Euler(0f, _yRotation, 0f);

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