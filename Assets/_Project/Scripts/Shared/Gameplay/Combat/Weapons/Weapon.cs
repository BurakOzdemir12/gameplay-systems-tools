using GameplaySystemsAndTools.Shared.Gameplay.Items;

namespace GameplaySystemsAndTools.Shared.Gameplay.Combat
{
    public class Weapon : Item, IPickupable
    {
        
        public ItemData Data => CurrentItemData;
        public int Amount => CurrentItemAmount;

        public bool OnPickedUp()
        {
            this.gameObject.SetActive(false);
            return true;
        }
    }
}