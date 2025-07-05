using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Plugins.Saneject.Demo.Scripts.Enemies
{
    /// <summary>
    /// Represents a single enemy instance in the scene. Handles movement, catching logic, and visual feedback.
    /// Implements <see cref="IEnemyCatchNotifiable" /> to signal when it is caught, and <see cref="IEnemyEvadeTarget" /> for its position.
    /// Note: Marked as <c>partial</c> since it uses <see cref="SerializeInterfaceAttribute" />.
    /// The Roslyn generator <c>SerializeInterfaceGenerator.dll</c> generates a partial to provide the serialized backing field and assignment logic.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public partial class Enemy : MonoBehaviour
    {
        [Inject, SerializeInterface]
        private IEnemyCatchNotifiable catchNotifiable;

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

            catchNotifiable.NotifyEnemyCaught();
            Destroy(gameObject);
        }
    }

    /*
    Roslyn generated partial:

    public partial class Enemy : UnityEngine.ISerializationCallbackReceiver
    {
        [UnityEngine.SerializeField, Saneject.Runtime.Attributes.InterfaceBackingField(interfaceType: typeof(IEnemyCatchNotifiable), isInjected: true, injectId: null)]
        private UnityEngine.Object __catchNotifiable;

         [UnityEngine.SerializeField, Saneject.Runtime.Attributes.InterfaceBackingField(interfaceType: typeof(IEnemyEvadeTarget), isInjected: true, injectId: null)]
        private UnityEngine.Object __evadeTarget;

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            catchNotifiable = __catchNotifiable as IEnemyCatchNotifiable;
            evadeTarget = __evadeTarget as IEnemyEvadeTarget;
        }
    }
    */
}