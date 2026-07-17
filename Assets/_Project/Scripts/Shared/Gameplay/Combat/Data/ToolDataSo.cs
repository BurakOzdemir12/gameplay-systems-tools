using GameplaySystemsAndTools.Shared.Data;
using GameplaySystemsAndTools.Shared.Gameplay.Feedback;
using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Combat
{
    [CreateAssetMenu(fileName = "ToolData", menuName = "Scriptable Objects/Tools/Tool Data")]
    public class ToolDataSo : ScriptableObject
    {
        public ToolImpactFeedbackProfile toolImpactFeedbackProfile;
        public ToolType toolType;

        public GameObject trailPrefab;
        public GameObject toolPrefab;
        public float staminaCost;
        public float durability;
    }
}