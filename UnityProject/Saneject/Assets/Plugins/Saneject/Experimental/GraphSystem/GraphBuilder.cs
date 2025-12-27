using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Experimental.GraphSystem.Data;
using Plugins.Saneject.Experimental.GraphSystem.Data.Nodes;
using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.GraphSystem
{
    public static class GraphBuilder
    {
        public static Graph Build(params Transform[] startTransforms)
        {
            Graph graph = new();
            HashSet<Transform> processedRoots = new();

            foreach (Transform startTransform in startTransforms.Where(startTransform => processedRoots.Add(startTransform.root)))
                graph.RootNodes.Add(BuildNode(startTransform.root));

            return graph;
        }

        private static TransformNode BuildNode(
            Transform transform,
            TransformNode parent = null)
        {
            TransformNode node = new();

            node.Parent = parent;
            node.Name = transform.name;
            node.Id = transform.GetInstanceID();
            node.Context = BuildContextKey(transform);
            node.Scope = BuildScopeData(transform, node);
            node.Components = BuildComponentData(transform);
            node.Children = transform.Cast<Transform>().Select(child => BuildNode(child, node)).ToList();

            return node;
        }

        private static List<ComponentNode> BuildComponentData(Transform transform)
        {
            List<ComponentNode> components = new();

            foreach (Component component in transform.GetComponents<Component>())
            {
                List<FieldNode> properties = BuildPropertyData(component);

                if (properties.Count == 0)
                    continue;

                components.Add(new ComponentNode
                {
                    Id = component.GetInstanceID(),
                    Name = component.GetType().FullName,
                    Properties = properties
                });
            }

            return components;
        }

        private static List<FieldNode> BuildPropertyData(Component component)
        {
            List<FieldNode> properties = new();

            foreach (FieldInfo field in component.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
            {
                if (!field.TryGetAttribute(out InjectAttribute injectAttribute))
                    continue;

                properties.Add(new FieldNode
                {
                    InjectId = injectAttribute.ID,
                    SuppressMissingErrors = injectAttribute.SuppressMissingErrors,
                    FieldInfo = field
                });
            }

            return properties;
        }

        private static ScopeNode BuildScopeData(
            Transform transform,
            TransformNode node)
        {
            if (transform.TryGetComponent(out Scope scope))
            {
                ScopeNode scopeNode = new();

                scopeNode.ParentScope = FindParentScope(node);
                scopeNode.Name = scope.GetType().Name;
                scopeNode.Id = scope.GetInstanceID();

                return scopeNode;
            }

            return null;
        }

        private static ScopeNode FindParentScope(TransformNode node)
        {
            TransformNode current = node.Parent;
            ScopeNode parentScope = null;

            while (current != null)
            {
                if (current.Scope == null || (UserSettings.UseContextIsolation && current.Context != node.Context))
                {
                    current = current.Parent;
                    continue;
                }

                parentScope = current.Scope;
                break;
            }

            return parentScope;
        }

        public static ContextNode BuildContextKey(Object obj)
        {
            GameObject gameObject = obj switch
            {
                GameObject go => go,
                Component c => c.gameObject,
                _ => null
            };

            // Non-GameObjects (ScriptableObjects, etc.)
            if (!gameObject)
                return new ContextNode
                {
                    Name = "Global",
                    Id = 0
                };

            // Prefab asset
            if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                GameObject assetRoot = gameObject.transform.root.gameObject;

                return new ContextNode
                {
                    Name = "Prefab Asset",
                    Id = assetRoot.GetInstanceID()
                };
            }

            // Prefab instance
            GameObject instanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);

            if (instanceRoot)
                return new ContextNode
                {
                    Name = "Prefab Instance",
                    Id = instanceRoot.GetInstanceID()
                };

            return new ContextNode
            {
                Name = "Scene Object",
                Id = gameObject.scene.handle
            };
        }
    }
}