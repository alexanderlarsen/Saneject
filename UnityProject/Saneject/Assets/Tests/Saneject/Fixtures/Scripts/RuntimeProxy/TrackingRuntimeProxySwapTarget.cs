using Plugins.Saneject.Runtime.Proxy;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.RuntimeProxy
{
    public class TrackingRuntimeProxySwapTarget : MonoBehaviour, IRuntimeProxySwapTarget
    {
        public int swapCount;

        public void SwapProxiesWithRealInstances()
        {
            swapCount++;
        }
    }
}
