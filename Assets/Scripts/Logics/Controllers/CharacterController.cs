using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
using VContainer;

namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class CharacterController : MonoBehaviour
    {
        [Inject]
        private readonly IMovement _movementService;

        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpHeight = 2f;

        private void Update()
        {
            HandleMovementInput();
            HandleJumpInput();
        }
        private void HandleMovementInput()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

            if (direction.magnitude >= 0.1f)
            {
                GameStateInvoker.TryInvoke(_movementService, nameof(_movementService.Move), direction, moveSpeed);
            }
        }
        private void HandleJumpInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameStateInvoker.TryInvoke(_movementService, nameof(_movementService.Jump), jumpHeight);
            }
        }
    }
}