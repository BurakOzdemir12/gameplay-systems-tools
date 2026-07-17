using System.Collections.Generic;
using UnityEngine;

namespace GameplaySystemsAndTools.Features.Enemy
{
    /// <summary>
    /// Pre-warmed pool of enemy HUD widgets shared by every EnemyUIController.
    /// Registered in the GameplayLifetimeScope and injected — no static Instance.
    /// </summary>
    public class EnemyHUDPool : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private EnemyHUDView hudPrefab;
        [SerializeField] private Transform hudParent;
        [SerializeField] private int initialPoolSize = 20;

        private Queue<EnemyHUDView> poolQueue = new Queue<EnemyHUDView>();

        private void Awake()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewHUDAndEnqueue();
            }
        }

        private EnemyHUDView CreateNewHUDAndEnqueue()
        {
            EnemyHUDView hud = Instantiate(hudPrefab, hudParent);
            hud.gameObject.SetActive(false);
            poolQueue.Enqueue(hud);
            return hud;
        }

        public EnemyHUDView GetHUD()
        {
            if (poolQueue.Count == 0)
            {
                CreateNewHUDAndEnqueue();
            }

            EnemyHUDView hud = poolQueue.Dequeue();
            hud.ResetHUD();
            return hud;
        }

        public void ReturnHUD(EnemyHUDView hud)
        {
            if (hud == null) return;

            hud.gameObject.SetActive(false);
            poolQueue.Enqueue(hud);
        }
    }
}