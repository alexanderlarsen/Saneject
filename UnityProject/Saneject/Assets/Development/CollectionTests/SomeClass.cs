using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Development.ReadOnlyCollectionDrawer
{
    public partial class SomeClass : MonoBehaviour
    {
        // [SerializeField, Inject]
        // private Collider[] boxColliders;
        //
        // [SerializeInterface, Inject]
        // private ITest[] testArray;
        //
        // [SerializeInterface, Inject]
        // private List<ITest> testList;
        //
        // [SerializeField, Inject]
        // private TestMono[] testMonosss;
        //
        [SerializeInterface, Inject]
        private ITest testSingle;
        
        [SerializeField,Inject]
        private TestMono testMonoSingle;
        
        [SerializeInterface, Inject]
        private ITest[] testArray;
        
        [SerializeInterface, Inject]
        private List<ITest> testList;
        
        //
        // [SerializeField, ReadOnly]
        // private List<string> stringListReadOnly = new() { "1", "2", "3" };
        //
        // [SerializeField, ReadOnly]
        // private string[] stringArrayReadOnly = { "1", "2", "3" };
        //
        // [SerializeField]
        // private List<string> stringList = new() { "1", "2", "3" };
        //
        // [SerializeField]
        // private string[] stringArray = { "1", "2", "3" };

        // private void Awake()
        // {
        //     Debug.Log("testArray");
        //
        //     foreach (ITest test in testArray)
        //         test.SaySomething();
        //
        //     Debug.Log("testList");
        //
        //     foreach (ITest test in testList)
        //         test.SaySomething();
        //
        //     Debug.Log("testSingle");
        //
        //     testSingle.SaySomething();
        // }
        //
        // [ContextMenu("FakeInject")]
        // private void FakeInject()
        // {
        //     testArray = GetComponentsInChildren<ITest>();
        //     testList = GetComponentsInChildren<ITest>().ToList();
        //     testSingle = GetComponentInChildren<ITest>();
        // }
    }
}