using GameplaySystemsAndTools.Shared.Gameplay.Climbing;
using GameplaySystemsAndTools.Shared.Gameplay.Combat;
using GameplaySystemsAndTools.Shared.Gameplay.Gathering;
using GameplaySystemsAndTools.Shared.Data;
using UnityEngine;

namespace GameplaySystemsAndTools.Features.Player
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable Objects/Characters/Player Config")]
    public class PlayerConfigSo : ScriptableObject
    {
        public MovementDataSo MovementData;
        public CombatDataSo CombatData;
        public DodgeDataSo DodgeData;
        public RollDataSo RollData;
        public JumpDataSo JumpData;
        public FallLandDataSo FallLandData;
        public GatheringDataSo GatheringDataSet;
        public ClimbTypeDataSo[] ClimbTypeDataSet;
        public AttackDataSo[] AttackTypeDataSet;

        // public AnimationProfileSo AnimationProfileData;
    }
}