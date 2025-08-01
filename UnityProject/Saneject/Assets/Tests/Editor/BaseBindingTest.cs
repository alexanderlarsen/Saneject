using System;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Runtime.Bindings;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Editor
{
    public abstract class BaseBindingTest : BaseTest
    {
        public override void Setup()
        {
            base.Setup();
            CreateHierarchy();
        }

        protected abstract void CreateHierarchy();

        protected ComponentBindingBuilder<T> BindComponent<T>(Scope scope) where T : class
        {
            return Call<Scope, ComponentBindingBuilder<T>>(scope, nameof(BindComponent), typeof(T));
        }

        protected ComponentBindingBuilder<T> BindMultipleComponents<T>(Scope scope) where T : class
        {
            return Call<Scope, ComponentBindingBuilder<T>>(scope, nameof(BindMultipleComponents), typeof(T));
        }

        protected ComponentBindingBuilder<TConcrete> BindComponent<TInterface, TConcrete>(Scope scope)
            where TInterface : class
            where TConcrete : Component, TInterface
        {
            return Call<Scope, ComponentBindingBuilder<TConcrete>>(scope, nameof(BindComponent), typeof(TInterface), typeof(TConcrete));
        }

        protected ComponentBindingBuilder<TConcrete> BindMultipleComponents<TInterface, TConcrete>(Scope scope)
            where TInterface : class
            where TConcrete : Component, TInterface
        {
            return Call<Scope, ComponentBindingBuilder<TConcrete>>(scope, nameof(BindMultipleComponents), typeof(TInterface), typeof(TConcrete));
        }

        protected ComponentBindingBuilder<TConcrete> BindGlobal<TConcrete>(Scope scope)
            where TConcrete : Component
        {
            return Call<Scope, ComponentBindingBuilder<TConcrete>>(scope, nameof(BindGlobal), typeof(TConcrete));
        }

        protected AssetBindingBuilder<T> BindAsset<T>(Scope scope)
            where T : Object
        {
            return Call<Scope, AssetBindingBuilder<T>>(scope, nameof(BindAsset), typeof(T));
        }

        protected AssetBindingBuilder<T> BindMultipleAssets<T>(Scope scope)
            where T : Object
        {
            return Call<Scope, AssetBindingBuilder<T>>(scope, nameof(BindMultipleAssets), typeof(T));
        }

        protected AssetBindingBuilder<TConcrete> BindAsset<TInterface, TConcrete>(Scope scope)
            where TInterface : class
            where TConcrete : Object, TInterface
        {
            return Call<Scope, AssetBindingBuilder<TConcrete>>(scope, nameof(BindAsset), typeof(TInterface), typeof(TConcrete));
        }

        protected AssetBindingBuilder<TConcrete> BindMultipleAssets<TInterface, TConcrete>(Scope scope)
            where TInterface : class
            where TConcrete : Object, TInterface
        {
            return Call<Scope, AssetBindingBuilder<TConcrete>>(scope, nameof(BindMultipleAssets), typeof(TInterface), typeof(TConcrete));
        }

        private static TReturn Call<TTarget, TReturn>(
            TTarget target,
            string methodName,
            params Type[] typeArgs)
        {
            MethodInfo method = typeof(TTarget)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(m => m.Name == methodName && m.IsGenericMethod && m.GetGenericArguments().Length == typeArgs.Length)
                ?.MakeGenericMethod(typeArgs);

            if (method == null)
                throw new MissingMethodException(typeof(TTarget).Name, methodName);

            return (TReturn)method.Invoke(target, null);
        }
    }
}