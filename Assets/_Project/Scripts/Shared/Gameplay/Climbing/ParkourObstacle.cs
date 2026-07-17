using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Climbing
{
    public class ParkourObstacle : MonoBehaviour
    {
        [SerializeField] private ParkourActionType actionType;
        public ParkourActionType ActionType => actionType;
    }
}