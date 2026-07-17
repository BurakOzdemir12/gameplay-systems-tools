using UnityEngine;
using UnityEngine.Serialization;

namespace GameplaySystemsAndTools.Shared.Gameplay.Items
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private int currentItemAmount;
        [SerializeField] private ItemType itemType;
        public ItemType ItemType => itemType;

        public int CurrentItemAmount
        {
            get => currentItemAmount;
            set => currentItemAmount = value;
        }

        [SerializeField] private ItemData currentItemData;

        public ItemData CurrentItemData
        {
            get => currentItemData;
            set => currentItemData = value;
        }
    }
}