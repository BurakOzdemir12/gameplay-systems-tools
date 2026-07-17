using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Combat
{
    public class ToolHandler : MonoBehaviour
    {
        [Header("Assign ToolRoot here (not Hitbox)")] [SerializeField]
        private GameObject currentToolRoot;
        public GameObject CurrentToolRoot => currentToolRoot;

        [Header("ToolLogic ")] private GameObject currentToolHitbox;
        public GameObject CurrentToolHitBox => currentToolHitbox;

        private ToolLogic currentToolLogic;

        public ToolLogic CurrentToolLogic => currentToolLogic;

        private void Start()
        {
            ToolLogic toolLogic = currentToolRoot.GetComponentInChildren<ToolLogic>(true);
            if (toolLogic == null)
            {
                Debug.LogError($"{name}: ToolLogic couldn't find in the children!", this);
                return;
            }

            currentToolLogic = toolLogic;
            currentToolHitbox = toolLogic.gameObject;
            currentToolHitbox.SetActive(false);
        }

        private void EnableTool()
        {
            if (currentToolHitbox != null)
                currentToolLogic.PerformLootAction();
        }

        private void DisableTool()
        {
            if (currentToolHitbox != null)
                currentToolLogic.EndLootAction();
        }
    }
}