using UnityEngine;
using UnityEngine.UI;

namespace DATN2.Assets.Scripts
{
    public class HelloWorldModel : MonoBehaviour
    {
        [SerializeField]
        private Button _helloWorldButton;

        public Button HelloWorldButton => _helloWorldButton;//getter
    }
}