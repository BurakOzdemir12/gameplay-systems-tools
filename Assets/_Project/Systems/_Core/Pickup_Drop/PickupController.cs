using System;
using _Project.Systems._Core.Pickup_Drop.Interfaces;
using UnityEngine;

namespace _Project.Systems._Core.Pickup_Drop
{
    public class PickupController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IPickupable>(out var pickable) && pickable != null)
            {
                
                pickable.PickUp();
            }
        }
    }
}