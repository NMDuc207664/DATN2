using VContainer;
using UnityEngine;
using VContainer.Unity;
namespace DATN2.Assets.Scripts.VC02
{
    public class HelloWorldMonoLifeTimeScope : LifetimeScope
    {
        [SerializeField] private HelloWorldMonoInstance _instance1;
        [SerializeField] private HelloWorldMonoInstance _instance2;
        protected override void Configure(IContainerBuilder builder)
        {

        }
        void OnEnable()
        {
            Debug.Log($"Is Same? {_instance1.HelloWorldManager == _instance2.HelloWorldManager}");
        }

    }
}