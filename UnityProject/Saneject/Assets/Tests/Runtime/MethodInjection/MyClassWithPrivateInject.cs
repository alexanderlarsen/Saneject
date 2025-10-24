﻿using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime.MethodInjection
{
    public class MyClassWithPrivateInject : MonoBehaviour
    {
        [ReadOnly, SerializeField]
        public bool privateInjected, protectedInjected;

        [Inject]
        private void InjectPrivate(MyDependency dep)
        {
            privateInjected = dep != null;
        }

        [Inject]
        protected void InjectProtected(MyDependency dep)
        {
            protectedInjected = dep != null;
        }
    }
}