using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Feedback
{
    public class CharacterFeedbackProfileHolder : MonoBehaviour
    {
        [SerializeField] private CharacterFeedbackProfile profile;
        public CharacterFeedbackProfile Profile => profile;
    }
}