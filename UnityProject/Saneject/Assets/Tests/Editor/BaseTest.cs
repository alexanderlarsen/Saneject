using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor
{
    public class BaseTest
    {
        [SetUp]
        public virtual void SetUp()
        {
        }

        [TearDown]
        public virtual void TearDown()
        {
            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList()
                .ForEach(Object.DestroyImmediate);
        }

        protected void IgnoreErrorMessages(bool ignore = true)
        {
            LogAssert.ignoreFailingMessages = ignore;
        }
    }
}