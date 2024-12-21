using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214.CombatAnalytics
{
    /// <summary>
    /// This class can be seen as a "report" of a single kill. 
    /// </summary>
    public class KillData
    {
        public Vector3 killPosition;
        public int playerHealthWhenKill;
        public float timeOfKill;
    }
    
    public class CombatAnalytics : MonoBehaviour
    {
        private static CombatAnalytics instance;
        public static CombatAnalytics Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<CombatAnalytics>();
                    if (instance == null)
                    {
                        GameObject combatAnalyticsGameObject = new GameObject("CombatAnalytics");
                        instance = combatAnalyticsGameObject.AddComponent<CombatAnalytics>();
                    }
                }

                return instance;
            }
        }
        
        
        private List<KillData> killLog = new List<KillData>();
        private float lastKillTime = 0f;
        private float sessionStartTime;
        private PlayerData playerData;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                sessionStartTime = Time.time;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            } ;
        }

        private void Start()
        {
            playerData = FindObjectOfType<PlayerData>();
        }

        public void LogKill(Vector3 positionOfKill)
        {
            float currentTime = Time.time - sessionStartTime;
            KillData killData = new KillData()
            {
                killPosition = positionOfKill,
                playerHealthWhenKill = playerData.GetCurrentPlayerHealth(),
                timeOfKill = currentTime
            };
            
            killLog.Add(killData);
            lastKillTime = currentTime;
            
            Debug.Log($"Kill logged at position: {positionOfKill}. Player health: {killData.playerHealthWhenKill}. Time elapsed since last kill: {killData.timeOfKill}");
        }
    }
    
}