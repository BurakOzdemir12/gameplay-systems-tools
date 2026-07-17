
namespace GameplaySystemsAndTools.Shared.Gameplay.Items
{
    public interface IPickupable
    {
        ItemData Data { get; }
        int Amount { get; }
        bool OnPickedUp();
    }
}