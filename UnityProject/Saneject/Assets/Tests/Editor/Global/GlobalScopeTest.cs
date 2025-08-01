using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Runtime.Global;
using UnityEngine;

namespace Tests.Editor.Global
{
    public class GlobalScopeTest : BaseTest
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            SetEditAccess(true);
            GlobalScope.Clear();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            SetEditAccess(false);
        }

        [Test]
        public void RegisterAndGet_ReturnsInstance()
        {
            DummyService instance = ScriptableObject.CreateInstance<DummyService>();
            GlobalScope.Register(instance);
            Assert.AreEqual(instance, GlobalScope.Get<DummyService>());
        }

        [Test]
        public void Register_ByType_ReturnsInstance()
        {
            AnotherService instance = ScriptableObject.CreateInstance<AnotherService>();
            GlobalScope.Register(typeof(AnotherService), instance);
            Assert.AreEqual(instance, GlobalScope.Get<AnotherService>());
        }

        [Test]
        public void IsRegistered_ReturnsTrueAfterRegistration()
        {
            GlobalScope.Register(ScriptableObject.CreateInstance<DummyService>());
            Assert.IsTrue(GlobalScope.IsRegistered<DummyService>());
        }

        [Test]
        public void Unregister_RemovesInstance()
        {
            GlobalScope.Register(ScriptableObject.CreateInstance<DummyService>());
            GlobalScope.Unregister<DummyService>();
            Assert.IsFalse(GlobalScope.IsRegistered<DummyService>());
        }

        [Test]
        public void Unregister_ByType_RemovesInstance()
        {
            GlobalScope.Register(ScriptableObject.CreateInstance<AnotherService>());
            GlobalScope.Unregister(typeof(AnotherService));
            Assert.IsFalse(GlobalScope.IsRegistered<AnotherService>());
        }

        [Test]
        public void Clear_RemovesAllInstances()
        {
            GlobalScope.Register(ScriptableObject.CreateInstance<DummyService>());
            GlobalScope.Register(ScriptableObject.CreateInstance<AnotherService>());
            GlobalScope.Clear();
            Assert.IsFalse(GlobalScope.IsRegistered<DummyService>());
            Assert.IsFalse(GlobalScope.IsRegistered<AnotherService>());
        }

        [Test]
        public void Get_ReturnsNullWhenNotRegistered()
        {
            Assert.IsNull(GlobalScope.Get<DummyService>());
        }

        [Test]
        public void Register_SameTypeTwice_DoesNotReplace()
        {
            IgnoreErrorMessages();
            
            DummyService first = ScriptableObject.CreateInstance<DummyService>();
            DummyService second = ScriptableObject.CreateInstance<DummyService>();

            GlobalScope.Register(first);
            GlobalScope.Register(second); // silently ignored

            Assert.AreEqual(first, GlobalScope.Get<DummyService>());
        }

        private void SetEditAccess(bool enabled)
        {
            typeof(GlobalScope)
                .GetField("allowUseInEditMode", BindingFlags.NonPublic | BindingFlags.Static)
                ?.SetValue(null, enabled);
        }

        private class DummyService : ScriptableObject
        {
        }

        private class AnotherService : ScriptableObject
        {
        }
    }
}