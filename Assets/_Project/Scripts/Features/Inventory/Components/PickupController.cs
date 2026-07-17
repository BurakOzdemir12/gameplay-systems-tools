using GameplaySystemsAndTools.Shared.Gameplay.Items;
using GameplaySystemsAndTools.Shared.Input;
using UnityEngine;
using VContainer;

namespace GameplaySystemsAndTools.Features.Inventory
{
    /// <summary>
    /// Picks up IPickupable items the player looks at and re-spawns dropped items in
    /// the world. Reacts to the interact input itself (injected PlayerInputHandler),
    /// keeping pickup entirely inside the Inventory feature.
    /// </summary>
    public class PickupController : MonoBehaviour
    {
        [field: SerializeField] public InventoryComponent InventoryComponent { get; private set; }

        [SerializeField] private float pickUpRange;
        [SerializeField] private LayerMask pickupableLayer;
        [SerializeField] private Vector3 dropOffset;

        private PlayerInputHandler inputHandler;

        [Inject]
        public void Construct(PlayerInputHandler input)
        {
            inputHandler = input;
            inputHandler.InteractEvent += TryPickup;
        }

        private void OnEnable()
        {
            InventoryComponent.OnItemDroppedToWorld += HandleItemDropped;
        }

        private void OnDisable()
        {
            InventoryComponent.OnItemDroppedToWorld -= HandleItemDropped;
        }

        private void OnDestroy()
        {
            if (inputHandler != null)
            {
                inputHandler.InteractEvent -= TryPickup;
            }
        }

        public void TryPickup()
        {
            if (InventoryComponent == null)
            {
                InventoryComponent = GetComponentInParent<InventoryComponent>();
                if (InventoryComponent == null)
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    Debug.LogError("[PickupController] Inventory is NULL.");
#endif
                    return;
                }
            }

            var cam = Camera.main;
            if (cam == null) return;

            // Center-of-screen ray: pick whatever the crosshair is pointing at.
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (!Physics.Raycast(ray, out var hit, pickUpRange,
                    pickupableLayer))
                return;

            var pickable = hit.collider.GetComponentInParent<IPickupable>();
            if (pickable == null) return;

            if (pickable.Data == null)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogError($"[PickupController] Pickable Data is NULL on {hit.collider.name}");
#endif
                return;
            }

            InventoryComponent.AddItem(pickable.Data, pickable.Amount);
            pickable.OnPickedUp();
        }

        private void HandleItemDropped(ItemData data, int amount)
        {
            var dropPoint = transform.TransformPoint(dropOffset);
            GameObject droppedItem = Instantiate(data.itemPrefab, dropPoint, Quaternion.identity);

            if (droppedItem.TryGetComponent<Item>(out var item))
            {
                item.CurrentItemData = data;
                item.CurrentItemAmount = amount;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.darkMagenta;
            Gizmos.DrawWireSphere(transform.position, pickUpRange);
        }
    }
}
