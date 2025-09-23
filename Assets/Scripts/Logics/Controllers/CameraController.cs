using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
using VContainer;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [Inject]
        public ICameraService _cameraService;

        public float senX;
        public float senY;
        public float rotationSmoothness = 15f;
        [SerializeField] private bool smoothCamera = false;
        // [SerializeField] private float sensitivity = 100f;

        // Update is called once per frame
        void Update()
        {
            HandleCameraInput();
        }
        private void HandleCameraInput()
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * senX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * senY;
            //_iCameraService.RotateCamera(mouseX, mouseY, senX);
            // yRotation += mouseX;
            // xRotation -= mouseY;
            // xRotation = Mathf.Clamp(xRotation, -45f, 60f);
            // transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            // orientation.rotation = Quaternion.Euler(0, yRotation, 0);
            GameStateInvoker.TryInvoke(_cameraService, nameof(_cameraService.RotateCamera), mouseX, mouseY, senX, smoothCamera, rotationSmoothness);
        }

    }
}