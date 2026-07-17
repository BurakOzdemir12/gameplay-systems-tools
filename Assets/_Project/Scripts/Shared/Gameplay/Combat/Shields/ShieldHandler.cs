using GameplaySystemsAndTools.Shared.Audio;
using GameplaySystemsAndTools.Shared.Events;
using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Combat
{
    public class ShieldHandler : MonoBehaviour
    {
        [Header("Shield Root")] [SerializeField]
        private GameObject currentShieldRoot;

        public GameObject CurrentShieldRoot => currentShieldRoot;

        [Header("Current Shield Model")] [SerializeField]
        private GameObject currentShieldModel;

        public GameObject CurrentShieldModel => currentShieldModel;

        [Header("Shield Hitbox")] [SerializeField]
        private GameObject currentShieldHitbox;

        public GameObject CurrentShieldHitbox => currentShieldHitbox;

        [Header("Shield Data")] [SerializeField]
        private ShieldDataSo currentShieldData;

        public ShieldDataSo CurrentShieldData => currentShieldData;

        [Header("Shield Logic")] [SerializeField]
        private ShieldLogic currentShieldLogic;

        public ShieldLogic CurrentShieldLogic => currentShieldLogic;


        private void Awake()
        {
            if (currentShieldLogic) return;
            ShieldLogic shieldLogic = currentShieldRoot.GetComponentInChildren<ShieldLogic>(true);
            if (shieldLogic == null)
            {
                Debug.LogError($"{name}: ShieldLogic couldn't find in the children!", this);
                return;
            }

            currentShieldLogic = shieldLogic;
            currentShieldHitbox = shieldLogic.gameObject;
            currentShieldModel = shieldLogic.transform.parent.gameObject;
            currentShieldData = shieldLogic.ShieldData;
        }

        private void OnEnable()
        {
            currentShieldLogic.OnShieldBreak += HandleShieldBreak;
        }

        public void EnableShield()
        {
            if (CurrentShieldHitbox != null)
            {
                currentShieldLogic.PerformBlock();
            }
        }

        public void DisableShield()
        {
            if (CurrentShieldHitbox != null)
            {
                currentShieldLogic.EndBlock();
            }
        }

        private void HandleShieldBreak()
        {
            if (!currentShieldLogic.ShieldData.TryGetShieldActionFeedback(
                    out var clip, out var vfx, out var volume
                )) return;

            // Shield handlers exist per character (player AND every enemy), so they
            // request feedback via events instead of referencing scene services.
            Vector3 breakPosition = CurrentShieldHitbox.transform.position;
            EventBus<SoundPlayRequestedEvent>.Publish(new SoundPlayRequestedEvent(
                clip, breakPosition, SoundChannel.Impact, volume));
            EventBus<VfxPlayRequestedEvent>.Publish(new VfxPlayRequestedEvent(
                vfx, breakPosition, Quaternion.identity));

            Destroy(currentShieldModel);
        }

        private void OnDisable()
        {
            currentShieldLogic.OnShieldBreak -= HandleShieldBreak;
        }
    }
}