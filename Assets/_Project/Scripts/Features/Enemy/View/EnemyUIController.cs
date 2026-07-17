using System.Collections;
using GameplaySystemsAndTools.Shared.Gameplay.Health;
using GameplaySystemsAndTools.Shared.Gameplay.Perception;
using UnityEngine;
using VContainer;

namespace GameplaySystemsAndTools.Features.Enemy
{
    /// <summary>
    /// Per-enemy HUD presenter: rents a HUD widget from the injected EnemyHUDPool,
    /// tracks the enemy's head position on screen and mirrors health/alert state.
    /// The pool arrives via VContainer (GameplayLifetimeScope injects every instance).
    /// </summary>
    public class EnemyUIController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private EnemyHealth enemyHealth;

        [SerializeField] private EnemyPerceptionController perception;

        [Header("Health and Alert Images Positioning")] [SerializeField]
        private Transform headPoint;

        [SerializeField] private Vector3 offset;

        [Header("UI Elements")] private EnemyHUDView currentHud;
        private Camera mainCamera;

        [Header("UI Show Range value")] [SerializeField]
        private float showRange = 20f;

        private float showRangeSqr;

        private Coroutine deathRoutine;
        private WaitForSeconds deathWait = new WaitForSeconds(4f);

        private bool isDead;

        private EnemyHUDPool hudPool;

        [Inject]
        public void Construct(EnemyHUDPool pool)
        {
            hudPool = pool;
        }

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            InitHUD();

            enemyHealth.OnTakeDamage += HandleTakeDamage;
            enemyHealth.OnDeath += HandleDeath;
            perception.OnPerceptionChanged += HandlePerceptionChange;
        }

        private void Start()
        {
            showRangeSqr = showRange * showRange;

            // Injection happens after OnEnable for scene objects, so retry here once.
            InitHUD();
        }

        private void LateUpdate()
        {
            if (!currentHud || !mainCamera) return;

            Vector3 targetPos = offset + headPoint.position;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(targetPos);

            Vector3 distanceToPlayer = transform.position - mainCamera.transform.position;
            float distanceSqr = distanceToPlayer.sqrMagnitude;

            bool isVisible = screenPos.z > 0 && distanceSqr < showRangeSqr && !isDead;

            currentHud.UpdatePosition(screenPos, isVisible);
        }

        private void InitHUD()
        {
            if (currentHud != null) return;
            if (hudPool == null) return;

            currentHud = hudPool.GetHUD();
            if (currentHud != null)
            {
                currentHud.ResetHUD();
                isDead = false;
            }
        }

        private void HandleTakeDamage(DamageInfo evt)
        {
            if (currentHud == null) return;

            currentHud.SetHealth(enemyHealth.CurrentHealth, enemyHealth.MaxHealth);
        }

        private void HandleDeath()
        {
            isDead = true;
            if (currentHud && hudPool)
            {
                currentHud.SetAlertState(false, 0);
                currentHud.SetSuspiciousState(false, 0);

                if (deathRoutine != null) StopCoroutine(deathRoutine);
                deathRoutine = StartCoroutine(DeathRoutine());
            }
        }

        private void HandlePerceptionChange(PerceptionState state, float time)
        {
            if (isDead) return;
            switch (state)
            {
                case PerceptionState.Alerted:
                    currentHud?.SetAlertState(true, time);
                    currentHud?.SetSuspiciousState(false, time);
                    break;
                case PerceptionState.Suspicious:
                    currentHud?.SetAlertState(false, time);
                    currentHud?.SetSuspiciousState(true, time);
                    break;
                case PerceptionState.Calm:
                    currentHud?.SetAlertState(false, time);
                    currentHud?.SetSuspiciousState(false, time);
                    break;
            }
        }

        private IEnumerator DeathRoutine()
        {
            // Keep the HUD around briefly after death so the player sees the state change.
            yield return deathWait;
            if (currentHud && hudPool)
            {
                hudPool.ReturnHUD(currentHud);
                currentHud = null;
            }
        }


        private void OnDisable()
        {
            enemyHealth.OnTakeDamage -= HandleTakeDamage;
            enemyHealth.OnDeath -= HandleDeath;
            perception.OnPerceptionChanged -= HandlePerceptionChange;
        }
    }
}
