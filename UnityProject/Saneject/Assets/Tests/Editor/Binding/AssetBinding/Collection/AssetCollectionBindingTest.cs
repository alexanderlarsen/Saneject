using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.AssetBinding.Collection
{
    public class AssetCollectionBindingTest : BaseTest
    {
        private GameObject root;

        [Test]
        public void ListsAndArraysAreNotNull()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetCollectionRequester requester = root.AddComponent<AssetCollectionRequester>();

            InjectableScriptableObject assetA = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            InjectableScriptableObject assetB = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            InjectableScriptableObject assetC = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            InjectableScriptableObject assetD = ScriptableObject.CreateInstance<InjectableScriptableObject>();

            // Set up bindings
            BindMultipleAssets<InjectableScriptableObject>(scope)
                .FromMethod(() => new[] { assetA, assetB });

            BindMultipleAssets<IInjectable, InjectableScriptableObject>(scope)
                .FromMethod(() => new List<InjectableScriptableObject> { assetC, assetD });

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.NotNull(requester.assetsConcreteArray);
            Assert.NotNull(requester.assetsConcreteList);
            Assert.NotNull(requester.assetsInterfaceArray);
            Assert.NotNull(requester.assetsInterfaceList);
        }

        [Test]
        public void InterfaceAndConcreteCollectionsAreDistinct()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetCollectionRequester requester = root.AddComponent<AssetCollectionRequester>();

            InjectableScriptableObject assetA = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            assetA.name = "A";

            InjectableScriptableObject assetB = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            assetB.name = "B";

            InjectableScriptableObject assetC = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            assetC.name = "C";

            InjectableScriptableObject assetD = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            assetD.name = "D";

            // Set up bindings
            BindMultipleAssets<InjectableScriptableObject>(scope)
                .FromMethod(() => new[] { assetA, assetB });

            BindMultipleAssets<IInjectable, InjectableScriptableObject>(scope)
                .FromMethod(() => new List<InjectableScriptableObject> { assetC, assetD });

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            HashSet<string> concreteSet = requester.assetsConcreteArray.Select(x => x.name).ToHashSet();
            HashSet<string> interfaceSet = requester.assetsInterfaceArray.Select(x => ((InjectableScriptableObject)x).name).ToHashSet();

            Assert.IsTrue(concreteSet.SetEquals(new[] { "A", "B" }));
            Assert.IsTrue(interfaceSet.SetEquals(new[] { "C", "D" }));
            CollectionAssert.AreNotEquivalent(concreteSet, interfaceSet);
        }

        [Test]
        public void SingleFieldsRemainNull()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetCollectionRequester requester = root.AddComponent<AssetCollectionRequester>();

            InjectableScriptableObject assetA = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            InjectableScriptableObject assetB = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            InjectableScriptableObject assetC = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            InjectableScriptableObject assetD = ScriptableObject.CreateInstance<InjectableScriptableObject>();

            // Set up bindings
            BindMultipleAssets<InjectableScriptableObject>(scope)
                .FromMethod(() => new[] { assetA, assetB });

            BindMultipleAssets<IInjectable, InjectableScriptableObject>(scope)
                .FromMethod(() => new List<InjectableScriptableObject> { assetC, assetD });

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.assetConcreteSingle);
            Assert.IsNull(requester.assetInterfaceSingle);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
        }
    }
}