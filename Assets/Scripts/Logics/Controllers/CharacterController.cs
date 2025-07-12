using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
using VContainer;

namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class CharacterController : MonoBehaviour
    {
        [Inject]
        private IMovement _movementService;

        [SerializeField] private float moveSpeed = 5f;

        private void Update()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

            if (direction.magnitude >= 0.1f)
            {
                _movementService.Move(direction, moveSpeed);
            }
        }
    }
}