using System;
using UnityEngine;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.UI.MVC
{
    /// <summary>
    /// Base class for the sample's serializable UI views.
    /// Encapsulates common operations for the bound <see cref="GameObject" />.
    /// </summary>
    [Serializable]
    public abstract class ViewBase
    {
        [SerializeField, HideInInspector]
        private GameObject gameObject;

        /// <summary>
        /// Shows the bound UI object.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the bound UI object.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Releases references owned by the view.
        /// </summary>
        public virtual void Dispose()
        {
            gameObject = null;
        }

        /// <summary>
        /// Binds this view to the controller's <see cref="GameObject" />.
        /// </summary>
        /// <param name="gameObject">The object that the view should control.</param>
        public void SetGameObject(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }
    }
}
