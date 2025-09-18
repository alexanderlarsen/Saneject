using System;
using System.Collections.Generic;
using System.ComponentModel;
using Plugins.Saneject.Runtime.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Plugins.Saneject.Runtime.Global
{
    /// <summary>
    /// Scene singleton used by Saneject to store and serialize global bindings for the current scene.
    /// Instances are managed automatically by Saneject’s DI system.
    /// On Awake, registers its bindings with the static <see cref="GlobalScope" />.
    /// Extra containers are deleted to ensure a single instance per scene. Manual creation is not supported.
    /// </summary>
    [DefaultExecutionOrder(-10000)]
    public class SceneGlobalContainer : MonoBehaviour
    {
        [SerializeField, Attributes.ReadOnly]
        private List<GlobalBinding> globalBindings = new();

        /// <summary>
        /// Registers all serialized global <see cref="Object" />s in this container with <see cref="GlobalScope" />.
        /// </summary>
        private void Awake()
        {
            foreach (GlobalBinding binding in globalBindings)
                GlobalScope.Register(binding.Type, binding.Instance);
        }

        /// <summary>
        /// Unregisters all global bindings from <see cref="GlobalScope" /> when destroyed.
        /// </summary>
        private void OnDestroy()
        {
            foreach (GlobalBinding binding in globalBindings)
                GlobalScope.Unregister(binding.Type);
        }

#if UNITY_EDITOR
        [SerializeField]
        private bool createdByDependencyInjector;

        /// <summary>
        /// Add an <see cref="Object" /> instance to the container as a global binding. Only one binding per type.
        /// Editor-only; not used at runtime.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Add(Object obj)
        {
            Type objType = obj.GetType();

            if (globalBindings.Exists(binding => binding.Type == objType))
                return false;

            GlobalBinding binding = new(obj);
            globalBindings.Add(binding);
            return true;
        }

        private void OnValidate()
        {
            EditorApplication.delayCall += () =>
            {
                if (this == null || createdByDependencyInjector)
                    return;

                this.DestroySelfOrGameObjectIfSolo();
                Debug.LogWarning("Saneject: Manual creation of SceneGlobalContainer is not allowed and any extra instances will be removed to ensure only one container exists per scene.");
            };
        }
#endif
    }
}