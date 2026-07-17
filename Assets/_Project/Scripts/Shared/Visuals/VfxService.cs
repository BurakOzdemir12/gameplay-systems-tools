using GameplaySystemsAndTools.Shared.Data;
using GameplaySystemsAndTools.Shared.Events;
using GameplaySystemsAndTools.Shared.Gameplay.Feedback;
using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Visuals
{
    /// <summary>
    /// Spawns one-shot visual effects for gameplay feedback. Fully event-driven —
    /// listens to feedback events plus VfxPlayRequestedEvent on the EventBus.
    /// Scene service registered in the GameplayLifetimeScope (no more static Instance).
    /// </summary>
    public class VfxService : MonoBehaviour
    {
        private EventBinding<CharacterTraversalEvent> interactionBinding;
        private EventBinding<CharacterCombatActionEvent> combatBinding;
        private EventBinding<CharacterGatheringActionEvent> gatheringBinding;
        private EventBinding<WeaponImpactActionEvent> weaponImpactBinding;
        private EventBinding<ToolImpactActionEvent> toolImpactBinding;
        private EventBinding<VfxPlayRequestedEvent> vfxRequestBinding;

        [SerializeField] private Transform vfxParent;

        private void OnEnable()
        {
            vfxRequestBinding = new EventBinding<VfxPlayRequestedEvent>(HandleVfxPlayRequested);
            EventBus<VfxPlayRequestedEvent>.Subscribe(vfxRequestBinding);

            interactionBinding = new EventBinding<CharacterTraversalEvent>(HandleTraversalEvent);
            EventBus<CharacterTraversalEvent>.Subscribe(interactionBinding);

            combatBinding = new EventBinding<CharacterCombatActionEvent>(HandleCombatActionEvent);
            EventBus<CharacterCombatActionEvent>.Subscribe(combatBinding);

            gatheringBinding = new EventBinding<CharacterGatheringActionEvent>(HandleGatheringActionEvent);
            EventBus<CharacterGatheringActionEvent>.Subscribe(gatheringBinding);

            weaponImpactBinding = new EventBinding<WeaponImpactActionEvent>(HandleWeaponImpactEvent);
            EventBus<WeaponImpactActionEvent>.Subscribe(weaponImpactBinding);

            toolImpactBinding = new EventBinding<ToolImpactActionEvent>(HandleToolImpact);
            EventBus<ToolImpactActionEvent>.Subscribe(toolImpactBinding);
        }


        private void OnDisable()
        {
            EventBus<VfxPlayRequestedEvent>.Unsubscribe(vfxRequestBinding);
            EventBus<CharacterTraversalEvent>.Unsubscribe(interactionBinding);
            EventBus<CharacterCombatActionEvent>.Unsubscribe(combatBinding);
            EventBus<CharacterGatheringActionEvent>.Unsubscribe(gatheringBinding);
            EventBus<WeaponImpactActionEvent>.Unsubscribe(weaponImpactBinding);
            EventBus<ToolImpactActionEvent>.Unsubscribe(toolImpactBinding);
        }

        #region Event Bus Handlers

        private void HandleTraversalEvent(CharacterTraversalEvent @evt)
        {
            if (!evt.Source.TryGetComponent(out CharacterFeedbackProfileHolder holder)) return;
            var profile = holder.Profile;
            if (profile == null) return;

            if (!profile.TryGetTraversalFeedback(evt.Surface, evt.Type, evt.ActionTag, out var clip, out var vfx,
                    out var volume)) return;
            SpawnVfx(vfx, evt.Position, Quaternion.identity);
        }

        private void HandleCombatActionEvent(CharacterCombatActionEvent @evt)
        {
            if (!evt.Source.TryGetComponent(out CharacterFeedbackProfileHolder holder)) return;
            var profile = holder.Profile;
            if (profile == null) return;

            if (!profile.TryGetCombatActionFeedback(evt.Surface, evt.Type, evt.WeaponType, evt.ActionTag,
                    out var clip, out var vfx,
                    out var volume)) return;
            SpawnVfx(vfx, evt.Position, Quaternion.identity);
        }

        private void HandleWeaponImpactEvent(WeaponImpactActionEvent evt)
        {
            if (evt.SourceTool == null) return;

            var weaponData = evt.WeaponData;
            if (weaponData == null) return;

            var profile = weaponData.weaponImpactFeedbackProfile;
            if (profile == null) return;

            WeaponType impactType = weaponData.weaponType;

            if (!profile.TryGetWeaponImpactActionFeedback(
                    evt.Surface,
                    impactType,
                    evt.Tag,
                    out _,
                    out var vfx,
                    out _)) return;

            SpawnVfx(vfx, evt.Position, Quaternion.LookRotation(evt.Normal));
        }

        private void HandleToolImpact(ToolImpactActionEvent evt)
        {
            if (evt.SourceTool == null) return;

            var toolData = evt.ToolData;
            if (toolData == null) return;

            var profile = toolData.toolImpactFeedbackProfile;
            if (profile == null) return;

            ToolType impactType = toolData.toolType;

            if (!profile.TryGetToolImpactActionFeedback(
                    evt.Surface,
                    impactType,
                    evt.Tag,
                    out _,
                    out var vfx,
                    out _)) return;

            SpawnVfx(vfx, evt.Position, Quaternion.LookRotation(evt.Normal));
        }

        private void HandleGatheringActionEvent(CharacterGatheringActionEvent evt)
        {
            if (!evt.Source.TryGetComponent(out CharacterFeedbackProfileHolder holder)) return;
            var profile = holder.Profile;
            if (profile == null) return;


            if (!profile.TryGetGatherActionFeedback(evt.Type, evt.ToolType, evt.ActionTag,
                    out var clip, out var vfx,
                    out var volume)) return;
            SpawnVfx(vfx, evt.Position, Quaternion.identity);
        }

        #endregion


        // Entry point for decoupled one-shot requests published from gameplay code.
        private void HandleVfxPlayRequested(VfxPlayRequestedEvent evt)
        {
            SpawnVfx(evt.VfxPrefab, evt.Position, evt.Rotation);
        }

        private void SpawnVfx(GameObject vfx, Vector3 position, Quaternion rotation = default)
        {
            if (vfx == null) return;

            Instantiate(vfx, position, rotation, vfxParent);
        }
    }
}