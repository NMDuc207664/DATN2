using System;
using VContainer.Unity;

namespace DATN2.Assets.Scripts
{
    public class HelloWorldPresenter : IStartable, IDisposable
    {
        private readonly HelloWorldService _helloWorldService;
        private readonly HelloWorldModel _helloWorldModel;

        public HelloWorldPresenter(HelloWorldService helloWorldService, HelloWorldModel helloWorldModel)
        {
            _helloWorldService = helloWorldService;
            _helloWorldModel = helloWorldModel;
        }

        public void Dispose()
        {
            _helloWorldModel.HelloWorldButton.onClick.RemoveListener(Print);
        }

        public void Start()
        {
            _helloWorldModel.HelloWorldButton.onClick.AddListener(Print);
        }

        private void Print() => _helloWorldService.Print();

    }
}