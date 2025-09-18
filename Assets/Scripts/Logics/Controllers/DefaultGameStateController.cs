using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class DefaultGameStateController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                GameStateManager.Instance.SetState(StateType.Ingame);
            }
        }
    }
}