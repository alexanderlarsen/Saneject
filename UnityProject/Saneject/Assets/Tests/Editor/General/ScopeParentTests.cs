using NUnit.Framework;
using Plugins.Saneject.Editor.Extensions;
using Tests.Runtime;
using UnityEditor;
using UnityEngine;

namespace Tests.Editor.General
{
    public class ScopeParentTests : BaseBindingTest
    {
        private GameObject sceneRootObject;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }

        //---------------------------------------------------------------------
        // 1. Scene simple chain
        //---------------------------------------------------------------------
        [Test]
        public void SceneParentResolution_SimpleChain()
        {
            IgnoreErrorMessages();

            TestScope sceneRootScope = sceneRootObject.AddComponent<TestScope>();

            GameObject childObject = new("SceneChild");
            childObject.transform.SetParent(sceneRootObject.transform);
            TestScope childSceneScope = childObject.AddComponent<TestScope>();

            GameObject grandChildObject = new("SceneGrandChild");
            grandChildObject.transform.SetParent(childObject.transform);
            TestScope grandChildSceneScope = grandChildObject.AddComponent<TestScope>();

            sceneRootScope.SetParentScope();
            childSceneScope.SetParentScope();
            grandChildSceneScope.SetParentScope();

            Assert.IsNull(sceneRootScope.ParentScope);
            Assert.NotNull(childSceneScope.ParentScope);
            Assert.AreSame(sceneRootScope, childSceneScope.ParentScope);
            Assert.NotNull(grandChildSceneScope.ParentScope);
            Assert.AreSame(childSceneScope, grandChildSceneScope.ParentScope);
        }

        //---------------------------------------------------------------------
        // 2. Scene siblings share same parent
        //---------------------------------------------------------------------
        [Test]
        public void SceneParentResolution_SiblingsShareSameParent()
        {
            IgnoreErrorMessages();

            TestScope sceneRootScope = sceneRootObject.AddComponent<TestScope>();

            GameObject firstChildObject = new("SceneChildA");
            firstChildObject.transform.SetParent(sceneRootObject.transform);
            TestScope firstChildSceneScope = firstChildObject.AddComponent<TestScope>();

            GameObject secondChildObject = new("SceneChildB");
            secondChildObject.transform.SetParent(sceneRootObject.transform);
            TestScope secondChildSceneScope = secondChildObject.AddComponent<TestScope>();

            sceneRootScope.SetParentScope();
            firstChildSceneScope.SetParentScope();
            secondChildSceneScope.SetParentScope();

            Assert.IsNull(sceneRootScope.ParentScope);
            Assert.AreSame(sceneRootScope, firstChildSceneScope.ParentScope);
            Assert.AreSame(sceneRootScope, secondChildSceneScope.ParentScope);
        }

        //---------------------------------------------------------------------
        // 3. Deep 5-level scene chain
        //---------------------------------------------------------------------
        [Test]
        public void SceneParentResolution_DeepChainFiveLevels()
        {
            IgnoreErrorMessages();

            TestScope sceneRootScope = sceneRootObject.AddComponent<TestScope>();

            GameObject[] levelObjects = new GameObject[4];
            TestScope[] levelScopes = new TestScope[4];

            levelObjects[0] = new GameObject("Level1");
            levelObjects[0].transform.SetParent(sceneRootObject.transform);
            levelScopes[0] = levelObjects[0].AddComponent<TestScope>();

            for (int i = 1; i < 4; i++)
            {
                levelObjects[i] = new GameObject("Level" + (i + 1));
                levelObjects[i].transform.SetParent(levelObjects[i - 1].transform);
                levelScopes[i] = levelObjects[i].AddComponent<TestScope>();
            }

            sceneRootScope.SetParentScope();

            foreach (TestScope scope in levelScopes)
                scope.SetParentScope();

            Assert.IsNull(sceneRootScope.ParentScope);
            Assert.AreSame(sceneRootScope, levelScopes[0].ParentScope);
            Assert.AreSame(levelScopes[0], levelScopes[1].ParentScope);
            Assert.AreSame(levelScopes[1], levelScopes[2].ParentScope);
            Assert.AreSame(levelScopes[2], levelScopes[3].ParentScope);
        }

        //---------------------------------------------------------------------
        // 4. Prefab root under scene root is isolated
        //---------------------------------------------------------------------
        [Test]
        public void ParentResolution_PrefabUnderSceneRoot_IsIsolated()
        {
            IgnoreErrorMessages();

            TestScope sceneRootScope = sceneRootObject.AddComponent<TestScope>();

            GameObject prefabInstance =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            prefabInstance.transform.SetParent(sceneRootObject.transform);
            TestScope prefabScope = prefabInstance.GetComponent<TestScope>();

            sceneRootScope.SetParentScope();
            prefabScope.SetParentScope();

            Assert.IsNull(sceneRootScope.ParentScope);
            Assert.IsNull(prefabScope.ParentScope);
        }

        //---------------------------------------------------------------------
        // 5. SceneScope under prefab jumps to nearest SceneScope ancestor
        //---------------------------------------------------------------------
        [Test]
        public void ParentResolution_SceneScopeUnderPrefab_JumpsToSceneRoot()
        {
            IgnoreErrorMessages();

            TestScope sceneRootScope = sceneRootObject.AddComponent<TestScope>();

            GameObject prefabInstance =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            prefabInstance.transform.SetParent(sceneRootObject.transform);
            TestScope prefabScope = prefabInstance.GetComponent<TestScope>();

            GameObject sceneChildObject = new("SceneChild");
            sceneChildObject.transform.SetParent(prefabInstance.transform);
            TestScope sceneChildScope = sceneChildObject.AddComponent<TestScope>();

            sceneRootScope.SetParentScope();
            prefabScope.SetParentScope();
            sceneChildScope.SetParentScope();

            Assert.IsNull(prefabScope.ParentScope);
            Assert.AreSame(sceneRootScope, sceneChildScope.ParentScope);
        }

        //---------------------------------------------------------------------
        // 6. Scene chain under prefab jumps correctly
        //---------------------------------------------------------------------
        [Test]
        public void ParentResolution_SceneChainUnderPrefab_JumpsCorrectly()
        {
            IgnoreErrorMessages();

            TestScope sceneRootScope = sceneRootObject.AddComponent<TestScope>();

            GameObject prefabInstance =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            prefabInstance.transform.SetParent(sceneRootObject.transform);
            TestScope prefabScope = prefabInstance.GetComponent<TestScope>();

            GameObject sceneChildObjectA = new("SceneChildA");
            sceneChildObjectA.transform.SetParent(prefabInstance.transform);
            TestScope sceneChildScopeA = sceneChildObjectA.AddComponent<TestScope>();

            GameObject sceneChildObjectB = new("SceneChildB");
            sceneChildObjectB.transform.SetParent(sceneChildObjectA.transform);
            TestScope sceneChildScopeB = sceneChildObjectB.AddComponent<TestScope>();

            sceneRootScope.SetParentScope();
            prefabScope.SetParentScope();
            sceneChildScopeA.SetParentScope();
            sceneChildScopeB.SetParentScope();

            Assert.AreSame(sceneRootScope, sceneChildScopeA.ParentScope);
            Assert.AreSame(sceneChildScopeA, sceneChildScopeB.ParentScope);
        }

        //---------------------------------------------------------------------
        // 7. Prefab under scene chain remains isolated
        //---------------------------------------------------------------------
        [Test]
        public void ParentResolution_PrefabUnderSceneChain_IsIsolated()
        {
            IgnoreErrorMessages();

            TestScope sceneRootScope = sceneRootObject.AddComponent<TestScope>();

            GameObject midSceneObject = new("MidScene");
            midSceneObject.transform.SetParent(sceneRootObject.transform);
            TestScope midSceneScope = midSceneObject.AddComponent<TestScope>();

            GameObject prefabInstance =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 2"));

            prefabInstance.transform.SetParent(midSceneObject.transform);
            TestScope prefabScope = prefabInstance.GetComponent<TestScope>();

            sceneRootScope.SetParentScope();
            midSceneScope.SetParentScope();
            prefabScope.SetParentScope();

            Assert.AreSame(sceneRootScope, midSceneScope.ParentScope);
            Assert.IsNull(prefabScope.ParentScope);
        }

        //---------------------------------------------------------------------
        // 8. Single prefab root is isolated
        //---------------------------------------------------------------------
        [Test]
        public void PrefabParentResolution_SinglePrefabRoot_Isolated()
        {
            IgnoreErrorMessages();

            GameObject prefabInstance =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope prefabScope = prefabInstance.GetComponent<TestScope>();
            prefabScope.SetParentScope();

            Assert.IsNull(prefabScope.ParentScope);
        }

        //---------------------------------------------------------------------
        // 9. Nested same-prefab instance does not inherit parent
        //---------------------------------------------------------------------
        [Test]
        public void PrefabParentResolution_NestedSamePrefab_IsIsolated()
        {
            IgnoreErrorMessages();

            GameObject parentPrefabInstance =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope parentPrefabScope = parentPrefabInstance.GetComponent<TestScope>();

            GameObject childPrefabInstance =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            childPrefabInstance.transform.SetParent(parentPrefabInstance.transform);
            TestScope childPrefabScope = childPrefabInstance.GetComponent<TestScope>();

            parentPrefabScope.SetParentScope();
            childPrefabScope.SetParentScope();

            Assert.IsNull(parentPrefabScope.ParentScope);
            Assert.IsNull(childPrefabScope.ParentScope);
        }

        //---------------------------------------------------------------------
        // 10. Nested different-prefab instance is isolated
        //---------------------------------------------------------------------
        [Test]
        public void PrefabParentResolution_NestedDifferentPrefab_IsIsolated()
        {
            IgnoreErrorMessages();

            GameObject prefabInstanceA =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope prefabScopeA = prefabInstanceA.GetComponent<TestScope>();

            GameObject prefabInstanceB =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 2"));

            prefabInstanceB.transform.SetParent(prefabInstanceA.transform);
            TestScope prefabScopeB = prefabInstanceB.GetComponent<TestScope>();

            prefabScopeA.SetParentScope();
            prefabScopeB.SetParentScope();

            Assert.IsNull(prefabScopeA.ParentScope);
            Assert.IsNull(prefabScopeB.ParentScope);
        }

        //---------------------------------------------------------------------
        // 11. Sibling prefab instances of same prefab are isolated
        //---------------------------------------------------------------------
        [Test]
        public void PrefabParentResolution_SiblingSamePrefab_Isolated()
        {
            IgnoreErrorMessages();

            GameObject firstPrefabInstance =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope firstPrefabScope = firstPrefabInstance.GetComponent<TestScope>();

            GameObject secondPrefabInstance =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope secondPrefabScope = secondPrefabInstance.GetComponent<TestScope>();

            firstPrefabScope.SetParentScope();
            secondPrefabScope.SetParentScope();

            Assert.IsNull(firstPrefabScope.ParentScope);
            Assert.IsNull(secondPrefabScope.ParentScope);
        }

        //---------------------------------------------------------------------
        // 12. Sibling prefab instances of different prefabs are isolated
        //---------------------------------------------------------------------
        [Test]
        public void PrefabParentResolution_SiblingDifferentPrefabs_Isolated()
        {
            IgnoreErrorMessages();

            GameObject prefabInstanceA =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope prefabScopeA = prefabInstanceA.GetComponent<TestScope>();

            GameObject prefabInstanceB =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 2"));

            TestScope prefabScopeB = prefabInstanceB.GetComponent<TestScope>();

            prefabScopeA.SetParentScope();
            prefabScopeB.SetParentScope();

            Assert.IsNull(prefabScopeA.ParentScope);
            Assert.IsNull(prefabScopeB.ParentScope);
        }

        //---------------------------------------------------------------------
        // 13. Alternating Scene-Prefab-Scene-Prefab-Scene chain
        //---------------------------------------------------------------------
        [Test]
        public void ParentResolution_AlternatingScenePrefabChain()
        {
            IgnoreErrorMessages();

            TestScope sceneRootScope = sceneRootObject.AddComponent<TestScope>();

            GameObject prefabInstanceA =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            prefabInstanceA.transform.SetParent(sceneRootObject.transform);
            TestScope prefabRootScopeA = prefabInstanceA.GetComponent<TestScope>();

            GameObject sceneScopeObjectA = new("SceneA");
            sceneScopeObjectA.transform.SetParent(prefabInstanceA.transform);
            TestScope sceneScopeA = sceneScopeObjectA.AddComponent<TestScope>();

            GameObject prefabInstanceB =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 2"));

            prefabInstanceB.transform.SetParent(sceneScopeObjectA.transform);
            TestScope prefabRootScopeB = prefabInstanceB.GetComponent<TestScope>();

            GameObject sceneScopeObjectB = new("SceneB");
            sceneScopeObjectB.transform.SetParent(prefabInstanceB.transform);
            TestScope sceneScopeB = sceneScopeObjectB.AddComponent<TestScope>();

            sceneRootScope.SetParentScope();
            prefabRootScopeA.SetParentScope();
            sceneScopeA.SetParentScope();
            prefabRootScopeB.SetParentScope();
            sceneScopeB.SetParentScope();

            Assert.IsNull(prefabRootScopeA.ParentScope);
            Assert.AreSame(sceneRootScope, sceneScopeA.ParentScope);
            Assert.IsNull(prefabRootScopeB.ParentScope);
            Assert.AreSame(sceneScopeA, sceneScopeB.ParentScope);
        }

        //---------------------------------------------------------------------
        // 14. Alternating contexts under additional scene scope
        //---------------------------------------------------------------------
        [Test]
        public void ParentResolution_AlternatingContextsUnderSceneChain()
        {
            IgnoreErrorMessages();

            TestScope sceneRootScope = sceneRootObject.AddComponent<TestScope>();

            GameObject midSceneObject = new("MidScene");
            midSceneObject.transform.SetParent(sceneRootObject.transform);
            TestScope midSceneScope = midSceneObject.AddComponent<TestScope>();

            GameObject prefabInstanceA =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            prefabInstanceA.transform.SetParent(midSceneObject.transform);
            TestScope prefabRootScopeA = prefabInstanceA.GetComponent<TestScope>();

            GameObject sceneScopeObjectA = new("SceneA");
            sceneScopeObjectA.transform.SetParent(prefabInstanceA.transform);
            TestScope sceneScopeA = sceneScopeObjectA.AddComponent<TestScope>();

            sceneRootScope.SetParentScope();
            midSceneScope.SetParentScope();
            prefabRootScopeA.SetParentScope();
            sceneScopeA.SetParentScope();

            Assert.AreSame(sceneRootScope, midSceneScope.ParentScope);
            Assert.IsNull(prefabRootScopeA.ParentScope);
            Assert.AreSame(midSceneScope, sceneScopeA.ParentScope);
        }

        //---------------------------------------------------------------------
        // 15. Scene under nested same-prefab finds scene ancestor
        //---------------------------------------------------------------------
        [Test]
        public void ParentResolution_SceneUnderNestedSamePrefab_FindsSceneAncestor()
        {
            IgnoreErrorMessages();

            TestScope sceneRootScope = sceneRootObject.AddComponent<TestScope>();

            GameObject prefabInstanceA =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            prefabInstanceA.transform.SetParent(sceneRootObject.transform);
            TestScope prefabRootScopeA = prefabInstanceA.GetComponent<TestScope>();

            GameObject prefabInstanceB =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            prefabInstanceB.transform.SetParent(prefabInstanceA.transform);
            TestScope prefabRootScopeB = prefabInstanceB.GetComponent<TestScope>();

            GameObject sceneScopeObject = new("NestedScene");
            sceneScopeObject.transform.SetParent(prefabInstanceB.transform);
            TestScope nestedSceneScope = sceneScopeObject.AddComponent<TestScope>();

            sceneRootScope.SetParentScope();
            prefabRootScopeA.SetParentScope();
            prefabRootScopeB.SetParentScope();
            nestedSceneScope.SetParentScope();

            Assert.IsNull(prefabRootScopeA.ParentScope);
            Assert.IsNull(prefabRootScopeB.ParentScope);
            Assert.AreSame(sceneRootScope, nestedSceneScope.ParentScope);
        }

        protected override void CreateHierarchy()
        {
            sceneRootObject = new GameObject("SceneRoot");
        }
    }
}