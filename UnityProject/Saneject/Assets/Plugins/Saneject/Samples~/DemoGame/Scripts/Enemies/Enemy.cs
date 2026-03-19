using System;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Enemies
{
    /// <summary>
    /// Enemy behaviour for the demo game's enemy prefab.
    /// Wanders until the injected <see cref="IEnemyEvadeTarget" /> comes close, then flees and
    /// raises <see cref="OnEnemyCaught" /> when the player catches it.
    /// Marked <c>partial</c> so Saneject can serialize the injected interface reference.
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

        public event Action<IEnemy> OnEnemyCaught;

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
            characterController.Move(direction * (moveSpeed * Time.deltaTime));
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

            OnEnemyCaught?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
