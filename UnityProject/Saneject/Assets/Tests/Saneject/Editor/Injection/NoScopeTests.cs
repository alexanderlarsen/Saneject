using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Plugins.Saneject.Runtime.Settings;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Saneject.Editor.Injection
{
    public class NoScopeTests
    {
        private bool originalLogInjectionSummary;

        [SetUp]
        public void SetUp()
        {
            originalLogInjectionSummary = UserSettings.LogInjectionSummary;
            UserSettings.LogInjectionSummary = true;
        }

        [TearDown]
        public void TearDown()
        {
            UserSettings.LogInjectionSummary = originalLogInjectionSummary;
        }

        [Test]
        public void Inject_WHEN_NoScopesExist_LogsNoScopesFound_DoesNotThrowAndLeavesMembersUnchanged()
        {
            // Expect logs
            LogAssert.Expect(LogType.Log, new Regex("^Saneject: Injection complete\\. No scopes were found\\. Nothing was injected\\.$"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");

            // Inject
            Assert.That(() => InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects), Throws.Nothing);

            // Assert
            Assert.That(target.dependency, Is.Null);
            LogAssert.NoUnexpectedReceived();
        }
    }
}
