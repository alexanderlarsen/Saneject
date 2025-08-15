using System;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Enemies
{
    /// <summary>
    /// Represents a single enemy instance in the scene. Handles movement, catching logic, and visual feedback.
    /// Implements <see cref="IEnemyCatchNotifiable" /> to signal when it is caught, and <see cref="IEnemyEvadeTarget" /> for its position.
    /// Note: Marked as <c>partial</c> since it uses <see cref="SerializeInterfaceAttribute" />.
    /// The Roslyn generator <c>SerializeInterfaceGenerator.dll</c> generates a partial to provide the serialized backing field and assignment logic.
    /// </summary>
    public partial class Enemy : MonoBehaviour, IEnemy
    {
        [Inject, SerializeInterface]
        private IEnemyEvadeTarget evadeTarget;

        [Inject, SerializeField]
        private CharacterController characterController;

        [SerializeField]
        private float wanderRadius = 5f;

        [SerializeField]
        private float walkSpeed = 3f;

        [SerializeField]
        private float runSpeed = 5;

        [SerializeField]
        private float evadeDistance = 3f;

        private Vector3 currentTarget;
        private float timer;

        public event Action OnEnemyCaught;

        public Transform Transform => transform;

        private void Start()
        {
            PickNewTarget();
        }

        private void Update()
        {
            RoamAndFlee();
        }

        private void RoamAndFlee()
        {
            timer += Time.deltaTime;
            Vector3 toEvade = transform.position - evadeTarget.Position;

            float moveSpeed = walkSpeed;

            if (toEvade.magnitude < evadeDistance)
            {
                currentTarget = transform.position + toEvade.normalized * wanderRadius;
                moveSpeed = runSpeed;
            }
            else if (timer > 2f || Vector3.Distance(transform.position, currentTarget) < 0.5f)
            {
                PickNewTarget();
            }

            Vector3 direction = (currentTarget - transform.position).normalized;
            characterController.SimpleMove(direction * moveSpeed);
        }

        private void PickNewTarget()
        {
            Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
            currentTarget = new Vector3(transform.position.x + randomCircle.x, transform.position.y, transform.position.z + randomCircle.y);
            timer = 0f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IEnemyEvadeTarget _))
                return;

            OnEnemyCaught?.Invoke();
            Destroy(gameObject);
        }
    }

    /*
    Roslyn generated partial:

    public partial class Enemy : ISerializationCallbackReceiver
    {
        [SerializeField, InterfaceBackingField(interfaceType: typeof(IEnemyEvadeTarget), isInjected: true, injectId: null)]
        private Object __evadeTarget;

        public void OnBeforeSerialize()
        {
    #if UNITY_EDITOR
            __evadeTarget = evadeTarget as Object;
    #endif
        }

        public void OnAfterDeserialize()
        {
            evadeTarget = __evadeTarget as IEnemyEvadeTarget;
        }
    }
    */
}