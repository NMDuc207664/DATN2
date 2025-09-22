
using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class CharacterRegisterManager : MonoBehaviour
    {
        public Vector2 MoveInput { get; set; } = Vector2.zero;
        public bool MoveIsPress = false;
        public bool RunIsPress = false;
        public bool JumpIsPress = false;
        public Vector2 LookInput { get; set; } = Vector2.zero;
        PlayerController _input = null;


        private void OnEnable()
        {
            _input = new PlayerController();
            _input.Movement.Enable();

            _input.Movement.Move.performed += SetMove;
            _input.Movement.Move.canceled += SetMove;

            _input.Movement.Run.started += SetRun;
            _input.Movement.Run.canceled += SetRun;

            _input.Movement.Look.performed += SetLook;
            _input.Movement.Look.canceled += SetLook;

            _input.Movement.Jump.started += SetJump;
            _input.Movement.Jump.canceled += SetJump;
        }

        private void SetRun(InputAction.CallbackContext context)
        {
            RunIsPress = context.started;
        }

        private void OnDisable()
        {
            _input.Movement.Move.performed -= SetMove;
            _input.Movement.Move.canceled -= SetMove;

            _input.Movement.Run.started -= SetRun;
            _input.Movement.Run.canceled -= SetRun;

            _input.Movement.Look.performed -= SetLook;
            _input.Movement.Look.canceled -= SetLook;

            _input.Movement.Jump.started -= SetJump;
            _input.Movement.Jump.canceled -= SetJump;

            _input.Movement.Disable();
        }

        private void SetJump(InputAction.CallbackContext context)
        {
            JumpIsPress = context.started;
        }

        private void SetLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
            MoveIsPress = !(MoveInput == Vector2.zero);
        }

        private void SetMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();

        }
    }
}
