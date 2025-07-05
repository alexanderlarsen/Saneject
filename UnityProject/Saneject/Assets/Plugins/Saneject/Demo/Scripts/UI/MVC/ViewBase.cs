using System;
using UnityEngine;

namespace Plugins.Saneject.Demo.Scripts.UI.MVC
{
    /// <summary>
    /// Base class for all UI views in the MVC pattern.
    /// Encapsulates logic for showing, hiding, and disposing of a bound <see cref="GameObject" />.
    /// </summary>
    [Serializable]
    public abstract class ViewBase
    {
        [SerializeField, HideInInspector]
        private GameObject gameObject;

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Dispose()
        {
            gameObject = null;
        }

        public void SetGameObject(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }
    }
}