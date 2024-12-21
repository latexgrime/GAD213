using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214.CombatAnalytics
{
    public class CombatAnalytics : MonoBehaviour
    {

        private static CombatAnalytics _instance;
        public static CombatAnalytics CombatAnalyticsInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CombatAnalytics>();
                    if (_instance == null)
                    {
                        var combatAnalyticsGameObject = new GameObject("CombatAnalytics");
                        _instance = combatAnalyticsGameObject.AddComponent<CombatAnalytics>();
                    }
                }

                return _instance;
            }
        }


        private readonly List<KillData> killLog = new();
        private float lastKillTime;
        private float sessionStartTime;
        private PlayerData playerData;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                sessionStartTime = Time.time;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }

            ;
        }

        private void Start()
        {
            playerData = FindObjectOfType<PlayerData>();
        }

        public void LogKill(Vector3 positionOfKill)
        {
            var currentTime = Time.time - sessionStartTime;
            var killData = new KillData
            {
                killPosition = positionOfKill,
                playerHealthWhenKill = playerData.GetCurrentPlayerHealth(),
                timeOfKill = currentTime
            };

            killLog.Add(killData);
            lastKillTime = currentTime;

            Debug.Log(
                $"Kill logged at position: {positionOfKill}. Player health: {killData.playerHealthWhenKill}. Time: {killData.timeOfKill}");
        }

        public float GetTimeSinceLastKill()
        {
            if (killLog.Count == 0) return 0f;
            return Time.time - sessionStartTime - lastKillTime;
        }

        public List<KillData> GetKillLog()
        {
            return new List<KillData>(killLog);
        }
    }
    
    /// <summary>
    ///     This class can be seen as a "report" of a single kill.
    /// </summary>
    public class KillData
    {
        public Vector3 killPosition;
        public int playerHealthWhenKill;
        public float timeOfKill;
    }

}