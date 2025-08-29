using DATN2.Assets.Scripts.Modals.Enum;

namespace DATN2.Assets.Scripts.Logics.Interface
{
    public interface ICameraService
    {
        [RequireGameState(StateType.Ingame)]
        void RotateCamera(ref float xRotation, float mouseX, float mouseY, float sensitivity);
        void LockCursor(bool isLocked);
    }
}
