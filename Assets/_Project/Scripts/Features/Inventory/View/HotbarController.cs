using System;
using System.Collections.Generic;
using GameplaySystemsAndTools.Shared.Input;
using UnityEngine;

namespace GameplaySystemsAndTools.Features.Inventory
{
    public class HotbarController : MonoBehaviour
    {
        [field: SerializeField] public PlayerInputHandler InputHandler { get; private set; }
        [SerializeField] private GameObject hotbarParent;
        public List<SlotUI> hotbarSlots = new List<SlotUI>();

        private int selectedHotbarIndex;

        private void Awake()
        {
            hotbarSlots.AddRange(hotbarParent.GetComponentsInChildren<SlotUI>());
        }

        private void OnEnable()
        {
            InputHandler.HotbarSelectEvent += HandleHotbarSelection;
            InputHandler.HotbarScrollEvent += HandleScroll;
        }

        private void HandleScroll(int dir)
        {
            selectedHotbarIndex = (selectedHotbarIndex + dir + hotbarSlots.Count) % hotbarSlots.Count;
            ToggleSelectedHighLight();
        }

        private void HandleHotbarSelection(int index)
        {
            if (index < 0 || index >= hotbarSlots.Count) return;

            selectedHotbarIndex = index;
            ToggleSelectedHighLight();
        }

        private void ToggleSelectedHighLight()
        {
            foreach (var slot in hotbarSlots)
                if (slot != hotbarSlots[selectedHotbarIndex])
                    slot.SetSelected(false);

            hotbarSlots[selectedHotbarIndex].SetSelected(true);
        }

        private void OnDisable()
        {
            InputHandler.HotbarSelectEvent -= HandleHotbarSelection;
            InputHandler.HotbarScrollEvent -= HandleScroll;
        }
    }
}