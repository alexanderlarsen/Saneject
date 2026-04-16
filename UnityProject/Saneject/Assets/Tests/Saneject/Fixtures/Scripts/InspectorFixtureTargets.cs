using System;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts
{
    public partial class InspectorFieldInfoBaseTarget : MonoBehaviour
    {
        public ComponentDependency basePublic;

        [SerializeField]
        private ComponentDependency baseSerialized;

        [SerializeInterface]
        private IDependency baseInterface;

        [HideInInspector]
        public ComponentDependency hidden;

        [NonSerialized]
        public ComponentDependency nonSerialized;

        [ReadOnly]
        public ComponentDependency readOnlyDependency;
    }

    public class InspectorFieldInfoTarget : InspectorFieldInfoBaseTarget
    {
        public ComponentDependency derivedPublic;

        [Inject]
        public ComponentDependency injectedDependency;
    }

    public class InspectorScriptableTarget : ScriptableObject
    {
        [Inject]
        public AssetDependency dependency;
    }
}
