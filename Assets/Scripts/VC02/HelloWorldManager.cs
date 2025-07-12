using System;
using VContainer.Unity;
using UnityEngine;

namespace DATN2.Assets.Scripts.VC02
{
    public class HelloWorldManager : IInitializable, IDisposable
    {
        public void Dispose()
        {
            Debug.Log("Dispose");
        }

        public void Print()
        {
            Debug.Log("Print");
        }

        public void Initialize()
        {
            Debug.Log("Initialize");
        }
    }
}