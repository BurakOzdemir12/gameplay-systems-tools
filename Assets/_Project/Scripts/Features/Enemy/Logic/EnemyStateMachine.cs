using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GameplaySystemsAndTools.Shared.Gameplay;
using GameplaySystemsAndTools.Shared.Gameplay.Combat;
using GameplaySystemsAndTools.Shared.Gameplay.Health;
using GameplaySystemsAndTools.Shared.Gameplay.Perception;
using UnityEngine;
using UnityEngine.AI;

using GameplaySystemsAndTools.Shared.Gameplay.StateMachine;

namespace GameplaySystemsAndTools.Features.Enemy
{
    public class EnemyStateMachine : StateMachineBase
    {
        [field: SerializeField] public CharacterController Controller { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public NavMeshAgent Agent { get; private set; }
        [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
        [field: SerializeField] public WeaponHandler WeaponHandler { get; private set; }
        [field: SerializeField] public ShieldHandler ShieldHandler { get; private set; }
        [field: SerializeField] public EnemyHealth Health { get; private set; }
        [field: SerializeField] public Target Target { get; private set; }
        [field: SerializeField] public Ragdoll Ragdoll { get; private set; }
        [field: SerializeField] public EnemyConfigSo EnemyConfigSo { get; private set; }
        [field: SerializeField] public FieldOfView FieldOfView { get; private set; }
        [field: SerializeField] public EnemyDefenceBrain EnemyDefenceBrain { get; private set; }
        [field: SerializeField] public EnemyPerceptionController EnemyPerceptionController { get; private set; }
        [field: SerializeField] public NoiseSensor NoiseSensor { get; private set; }

        [Tooltip("Chase and Attack detect buffer length")] [SerializeField]
        private int bufferMax = 4;

        public int BufferMax => bufferMax;

        [Header("Blocking Settings")] [field: SerializeField]
        public float blockLayerWeight = 0;


        public Vector3 firstSpawnPoint;
        public int BlockingLayerIndex { get; private set; }

        private void Awake()
        {
            BlockingLayerIndex = Animator.GetLayerIndex("Block Layer");
        }

        private void OnEnable()
        {
            Health.OnTakeDamage += HandleTakeDamage;
            Health.OnDeath += HandleDeath;
            Health.OnStunned += HandleStunned;
            ShieldHandler.CurrentShieldLogic.OnBlocked += HandleShieldImpact;
        }

        private void Start()
        {
            firstSpawnPoint = transform.position;
            Controller = GetComponent<CharacterController>();
            //EnemyPerception Init
            if (EnemyPerceptionController == null)
            {
                EnemyPerceptionController = GetComponent<EnemyPerceptionController>();
            }

            EnemyPerceptionController.Initialize(gameObject, EnemyConfigSo, FieldOfView, NoiseSensor);

            SwitchState(new EnemyIdleState(this));
        }

        private void HandleTakeDamage(DamageInfo damageInfo)
        {
            SwitchState(new EnemyImpactState(this));
            if (damageInfo.SourceObject != null)
            {
                EnemyPerceptionController.OnDamageTaken(damageInfo.SourceObject);
            }
        }

        private void HandleDeath()
        {
            EnemyPerceptionController.OnDeath();
            SwitchState(new EnemyDeadState(this));
        }

        private void HandleShieldImpact(BlockContext ctx)
        {
            Animator.CrossFadeInFixedTime(EnemyConfigSo.CombatData.BlockImpactAnimHash,
                EnemyConfigSo.CombatData.CrossFadeDurationCombat);
            // SwitchState(new EnemyBlockParryState(this, ctx));
        }

        private void HandleStunned(float duration)
        {
            SwitchState(new EnemyStunnedState(this, duration));
        }

        private void OnDisable()
        {
            Health.OnTakeDamage -= HandleTakeDamage;
            Health.OnDeath -= HandleDeath;
            Health.OnStunned -= HandleStunned;
            ShieldHandler.CurrentShieldLogic.OnBlocked -= HandleShieldImpact;
        }
    }
}