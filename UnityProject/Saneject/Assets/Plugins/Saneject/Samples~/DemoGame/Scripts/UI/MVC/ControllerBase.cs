using UnityEngine;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.UI.MVC
{
    /// <summary>
    /// Base class for the sample's UI controllers.
    /// Owns a serializable <see cref="ViewBase" /> instance and keeps it bound to the controller's <see cref="GameObject" />.
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
