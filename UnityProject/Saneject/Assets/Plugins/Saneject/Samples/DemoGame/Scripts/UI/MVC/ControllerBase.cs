using UnityEngine;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.UI.MVC
{
    /// <summary>
    /// Abstract base class for MVC controllers.
    /// Manages the lifetime and GameObject binding of a <see cref="ViewBase" /> instance.
    /// </summary>
    public abstract class ControllerBase<TView> : MonoBehaviour where TView : ViewBase, new()
    {
        [SerializeField]
        protected TView view = new();

        protected virtual void OnValidate()
        {
            view.SetGameObject(gameObject);
        }

        protected virtual void OnDestroy()
        {
            view.Dispose();
        }
    }
}