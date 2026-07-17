using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Items
{
    public class ItemData : ScriptableObject
    {
        public int itemId;
        public string displayName;
        [TextArea(4, 4)] public string description;
        public Sprite icon;
        public int maxStackSize;
        public float weight;
        public bool isStackable;
        public GameObject itemPrefab;
        public GameObject handItemPrefab;
    }
}