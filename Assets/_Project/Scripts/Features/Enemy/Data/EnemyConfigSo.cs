using GameplaySystemsAndTools.Shared.Gameplay.Combat;
using GameplaySystemsAndTools.Shared.Data;
using UnityEngine;

namespace GameplaySystemsAndTools.Features.Enemy
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable Objects/Characters/Enemy Config")]
    public class EnemyConfigSo : ScriptableObject
    {
        public EnemyMovementDataSo MovementData;
        public EnemyCombatDataSo CombatData;
        public DodgeDataSo DodgeData;
        public EnemyAIBrainDataSo AIBrainData;
    }
}