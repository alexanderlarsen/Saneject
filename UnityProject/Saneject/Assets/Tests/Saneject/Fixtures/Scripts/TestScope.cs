using Plugins.Saneject.Runtime.Bindings.Asset;
using Plugins.Saneject.Runtime.Bindings.Component;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Fixtures.Scripts
{
    public class TestScope : Scope
    {
        protected override void DeclareBindings()
        {
        }

        public new ComponentBindingBuilder<T> BindComponent<T>() where T : class
        {
            return base.BindComponent<T>();
        }

        public new ComponentBindingBuilder<T> BindComponents<T>() where T : class
        {
            return base.BindComponents<T>();
        }

        public new ComponentBindingBuilder<T> BindMultipleComponents<T>() where T : class
        {
            return base.BindMultipleComponents<T>();
        }

        public new ComponentBindingBuilder<TConcrete> BindComponent<TInterface, TConcrete>()
            where TConcrete : Component, TInterface
            where TInterface : class
        {
            return base.BindComponent<TInterface, TConcrete>();
        }

        public new ComponentBindingBuilder<TConcrete> BindComponents<TInterface, TConcrete>()
            where TConcrete : Component, TInterface
            where TInterface : class
        {
            return base.BindComponents<TInterface, TConcrete>();
        }

        public new ComponentBindingBuilder<TConcrete> BindMultipleComponents<TInterface, TConcrete>()
            where TConcrete : Component, TInterface
            where TInterface : class
        {
            return base.BindMultipleComponents<TInterface, TConcrete>();
        }

        public new AssetBindingBuilder<TConcrete> BindAsset<TConcrete>() where TConcrete : Object
        {
            return base.BindAsset<TConcrete>();
        }

        public new AssetBindingBuilder<TConcrete> BindAssets<TConcrete>() where TConcrete : Object
        {
            return base.BindAssets<TConcrete>();
        }

        public new AssetBindingBuilder<TConcrete> BindMultipleAssets<TConcrete>() where TConcrete : Object
        {
            return base.BindMultipleAssets<TConcrete>();
        }

        public new AssetBindingBuilder<TConcrete> BindAsset<TInterface, TConcrete>()
            where TConcrete : Object, TInterface
            where TInterface : class
        {
            return base.BindAsset<TInterface, TConcrete>();
        }

        public new AssetBindingBuilder<TConcrete> BindAssets<TInterface, TConcrete>()
            where TConcrete : Object, TInterface
            where TInterface : class
        {
            return base.BindAssets<TInterface, TConcrete>();
        }

        public new AssetBindingBuilder<TConcrete> BindMultipleAssets<TInterface, TConcrete>()
            where TConcrete : Object, TInterface
            where TInterface : class
        {
            return base.BindMultipleAssets<TInterface, TConcrete>();
        }

        public new GlobalComponentBindingBuilder<TConcrete> BindGlobal<TConcrete>() where TConcrete : Component
        {
            return base.BindGlobal<TConcrete>();
        }
    }
}
