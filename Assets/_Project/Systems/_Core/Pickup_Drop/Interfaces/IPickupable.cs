using _Project.Systems._Core.Weapon_Tool_Handlers;
using UnityEngine;

namespace _Project.Systems._Core.Pickup_Drop.Interfaces
{
    public struct PickupContext
    {
        public GameObject Picker;
        public WeaponHandler WeaponHandler;
        public ToolHandler ToolHandler;

       
    }

    public interface IPickupable
    {
        void PickUp();
    }
}