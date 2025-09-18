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

        void Update()
        {
            HandleCameraInput();
        }
        private void HandleCameraInput()
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * senX * Time.deltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * senY * Time.deltaTime;
            GameStateInvoker.TryInvoke(_cameraService, nameof(_cameraService.RotateCamera), mouseX, mouseY, senX, rotationSmoothness);
        }

    }
}