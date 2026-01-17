using System;
using System.Collections.Generic;
using _Project.Systems.InventorySystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace _Project.Systems.InventorySystem
{
    public class Inventory : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public ItemDataSo swordItem;
        public ItemDataSo pickaxeItem;

        public GameObject hotbarParent;
        public GameObject inventorySlotParent;

        public Image dragIcon;

        private List<Slot> inventorySlots = new List<Slot>();
        private List<Slot> hotbarSlots = new List<Slot>();
        private List<Slot> allSlots = new List<Slot>();

        private Slot draggedSlot = null;
        private bool isDragging = false;

        private void Awake()
        {
            inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());
            hotbarSlots.AddRange(hotbarParent.GetComponentsInChildren<Slot>());

            allSlots.AddRange(inventorySlots);
            allSlots.AddRange(hotbarSlots);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                AddItem(swordItem, 3);
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                AddItem(pickaxeItem, 1);
            }
        }

        public void AddItem(ItemDataSo itemToAdd, int amount)
        {
            int remaining = amount;
            foreach (Slot slot in allSlots)
            {
                if (slot.HasItem && slot.StoredItem == itemToAdd)
                {
                    int currentAmount = slot.ItemAmount;
                    int maxStack = itemToAdd.maxStackSize;
                    if (currentAmount < maxStack)
                    {
                        int spaceLeft = maxStack - currentAmount;
                        int amountToAdd = Mathf.Min(spaceLeft, remaining);
                        slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                        remaining -= amountToAdd;

                        if (remaining <= 0) return;
                    }
                }
            }

            foreach (Slot slot in allSlots)
            {
                if (!slot.HasItem)
                {
                    int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                    slot.SetItem(itemToAdd, amountToPlace);
                    remaining -= amountToPlace;
                    if (remaining <= 0)
                    {
                        return;
                    }
                }
            }

            if (remaining > 0)
            {
                Debug.LogError("Not enough space in inventory to add item: " + itemToAdd.name);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Pointer down");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("Begin drag");
            
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("End drag");
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("Dragging!!");
        }
    }
}