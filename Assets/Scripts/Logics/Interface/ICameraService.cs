using DATN2.Assets.Scripts.Modals.Enum;

namespace DATN2.Assets.Scripts.Logics.Interface
{
    public interface ICameraService
    {
        [RequireGameState(StateType.Ingame)]
        // void RotateCamera(ref float xRotation, float mouseX, float mouseY, float sensitivity);
        void RotateCamera(float mouseX, float mouseY, float sensitivity, bool smoothCamera, float rotationSmoothness);
        void LockCursor(bool isLocked);
    }
}
