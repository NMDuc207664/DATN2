using VContainer;
using UnityEngine;
namespace DATN2.Assets.Scripts.VC02
{
    public class HelloWorldMonoInstance : MonoBehaviour
    {
        private HelloWorldManager _helloWorldManager;
        public HelloWorldManager HelloWorldManager => _helloWorldManager;
        [Inject]
        private void Construct(HelloWorldManager helloWorldManager)
        {
            _helloWorldManager = helloWorldManager;
            Debug.Log("Construct Mono Instance");
        }

        private void OnEnable()
        {
            _helloWorldManager.Print();
        }
    }
}