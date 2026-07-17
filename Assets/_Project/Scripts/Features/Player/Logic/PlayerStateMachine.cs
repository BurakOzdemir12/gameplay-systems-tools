using GameplaySystemsAndTools.Shared.Gameplay;
using GameplaySystemsAndTools.Shared.Input;
using GameplaySystemsAndTools.Shared.Gameplay.StateMachine;
using GameplaySystemsAndTools.Shared.Gameplay.Climbing;
using GameplaySystemsAndTools.Shared.Gameplay.Combat;
using GameplaySystemsAndTools.Shared.Gameplay.Gathering;
using GameplaySystemsAndTools.Shared.Gameplay.Health;
using GameplaySystemsAndTools.Shared.Gameplay.Perception;
using UnityEngine;

namespace GameplaySystemsAndTools.Features.Player
{
    /// <summary>
    /// Hub of the player's hierarchical FSM: exposes every component the states need
    /// and reacts to health/input events by switching states. Inventory concerns are
    /// intentionally NOT referenced here — the Inventory feature listens to input on
    /// its own (feature isolation rule).
    /// </summary>
    public class PlayerStateMachine : StateMachineBase
    {
        [field: SerializeField] public PlayerInputHandler InputHandler { get; private set; }
        [field: SerializeField] public CharacterController Controller { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public Targeter Targeter { get; private set; }
        [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
        [field: SerializeField] public WeaponHandler WeaponHandler { get; private set; }
        [field: SerializeField] public ShieldHandler ShieldHandler { get; private set; }
        [field: SerializeField] public ToolLogic ToolLogic { get; private set; }
        [field: SerializeField] public PlayerHealth Health { get; private set; }
        [field: SerializeField] public Ragdoll Ragdoll { get; private set; }
        [field: SerializeField] public ClimbController ClimbController { get; private set; }
        [field: SerializeField] public GatheringController GatheringController { get; private set; }
        [field: SerializeField] public GroundChecker GroundChecker { get; private set; }
        [field: SerializeField] public PlayerConfigSo PlayerConfigSo { get; private set; }
        [field: SerializeField] public PlayerAttackSignal PlayerAttackSignal { get; private set; }
        [field: SerializeField] public NoiseEmitter NoiseEmitter { get; private set; }

        // [Header("Weapon Transforms")] [Tooltip("Sword Holder Transform")] [field: SerializeField]
        // public Transform swordHolderR;

        [Header("Upper Body Layer Wight")] [field: SerializeField]
        public bool isWeaponEquipped = false;

        [Header("Upper Body Layer Wight")] [field: SerializeField]
        public float upperBodyLayerWeight;

        [Header("Upper Body Layer Wight")] [field: SerializeField]
        public float armedLayerWeight;

        [Header("Blocking Settings")] [field: SerializeField]
        public float blockLayerWeight = 1;

        [Header("Animation Settings")] [Tooltip("The duration time of the locomotion blend tree ")] [SerializeField]
        private float crossFadeDuration = 0.1f;

        public float CrossFadeDuration => crossFadeDuration;

        [Header("Dodge Settings")] [Tooltip("Dodge Press Previous Time ")]
        private float previousDodgeTime;

        public float PreviousDodgeTime
        {
            get => previousDodgeTime;
            set => previousDodgeTime = value;
        }

        [Tooltip("Previous time of roll animation")]
        private float previousRollTime;

        public float PreviousRollTime
        {
            get => previousRollTime;
            set => previousRollTime = value;
        }

        [Header("Jump Settings")]
        [field: Tooltip("Previous time of jump animation")]
        public bool PendingJump { get; private set; }

        private float previousJumpTime;

        public float PreviousJumpTime
        {
            get => previousJumpTime;
            set => previousJumpTime = value;
        }

        [Header("Interact Allowance Time")] private float nextInteractAllowedTime = 0.5f;

        public float NextInteractAllowedTime => nextInteractAllowedTime;
        public readonly int Mirror = Animator.StringToHash("Mirror");

        public int BlockingLayerIndex { get; private set; }
        public int UpperBodyLayerIndex { get; private set; }
        public int ArmedLayerIndex { get; private set; }

        public Transform MainCameraTransform { get; private set; }

        private PlayerGroundedState CurrentGrounded => CurrentState as PlayerGroundedState;
        public StateBase CurrentSubState => (CurrentState as StateBase)?.GetSubState();

        [Space(10)] [Header("Ground Check Settings")] [Tooltip("Grounded Grace Period")] [SerializeField]
        private float groundedGrace = 0.1f;

        public float GroundedGrace => groundedGrace;

        public bool IsGrounded => GroundChecker != null && GroundChecker.IsGrounded;

        [Tooltip("If ground is this close below, we stay in GroundedState (useful for stairs/step-down).")]
        [SerializeField]
        private float groundedSnapDistance = 0.2f;

        public float GroundedSnapDistance => groundedSnapDistance;

        #region Testing Debugging

        [Header("For Testing Purposes")]
        [Tooltip("is Climbing Free Flow")]
        [field: SerializeField]
        public bool IsFreeFlowClimb { get; private set; }


        [Tooltip("İf Your animations works with the root motion, set this to true")] [SerializeField]
        public bool workWithRootMotion = false;

        [Tooltip("In Combat or alert mode, its gonna roll but in normal mode jump will work ")] [SerializeField]
        private bool inAlertMode = false;

        public bool IsInAlertMode
        {
            get => inAlertMode;
            set => inAlertMode = value;
        }

        #endregion


        private void Awake()
        {
            BlockingLayerIndex = Animator.GetLayerIndex("Block Layer");
            UpperBodyLayerIndex = Animator.GetLayerIndex("Upper Body");
            ArmedLayerIndex = Animator.GetLayerIndex("Armed Layer");
        }

        private void OnEnable()
        {
            Health.OnTakeDamage += HandleTakeDamage;
            Health.OnDeath += HandleDeath;
            Health.OnStunned += HandleStunned;
        }


        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (Camera.main == null) Debug.LogError("No main camera found!");
            if (Camera.main != null) MainCameraTransform = Camera.main.transform;
            SwitchState(new PlayerGroundedState(this));
        }

        // Damage/death/stun override whatever the FSM is doing, so they switch the
        // root state directly instead of going through sub-state transitions.
        private void HandleTakeDamage(DamageInfo damageInfo)
        {
            SwitchState(new PlayerImpactState(this));
        }

        private void HandleDeath()
        {
            SwitchState(new PlayerDeadState(this));
        }

        public void SetDodgeCooldownTime(float time)
        {
            previousDodgeTime = time;
        }

        public void SetRollCooldownTime(float time)
        {
            previousRollTime = time;
        }

        public void PendingJumpState(float time)
        {
            PendingJump = true;
            previousJumpTime = time;
        }

        public bool ConsumeJump() // if Jumping turn true
        {
            if (!PendingJump) return false;
            PendingJump = false;
            return true;
        }

        public bool CanUseInteractNow(float cooldown)
        {
            if (Time.time < nextInteractAllowedTime) return false;
            nextInteractAllowedTime = Time.time + cooldown;
            return true;
        }

        private void HandleStunned(float duration)
        {
            SwitchState(new PlayerStunnedState(this, 2f));
        }

        private void OnDisable()
        {
            Health.OnTakeDamage -= HandleTakeDamage;
            Health.OnDeath -= HandleDeath;
            Health.OnStunned -= HandleStunned;
        }
    }
}