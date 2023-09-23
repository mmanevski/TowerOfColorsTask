using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MissionType
{ 
    DestroyTiles,
    HalfTime, 
    NoMiss
}
public enum RewardType
{ 
    TimerBoost,
    ExtraBalls,
    ExtraPowerup,
    ExtraMultiballs
}
public enum MissionDifficulty
{ 
    Easy,
    Medium,
    Hard
}

public class MissionManager : MonoBehaviour
{

    [SerializeField]
    MissionsUI missionsUI;
    [SerializeField]
    List<MissionConfig> allMissionsList;
    [SerializeField]
    List<MissionConfig> activeMissionsList = new List<MissionConfig>();

    //keeping stats for deciding rewards
    bool levelHalfTime = false;
    int destroyedTilesNum = 0;
    bool noMisses = false;


    private void Start()
    {
        DecideActiveMissions();
        ResetRewardStats();
    }

    public void LevelFinished()
    {
        DecideRewards();
        ResetRewardStats();
    }

    private void ResetRewardStats()
    {
        levelHalfTime = true;
        destroyedTilesNum = 0;
        noMisses = true;
    }

    private void DecideRewards()
    { 
        foreach (MissionConfig mission in activeMissionsList)
        {
            switch (mission.missionType)
            {
                case MissionType.DestroyTiles:
                    if (destroyedTilesNum > mission.goal)
                    {
                        mission.FinishMission();
                    }
                    break;
                case MissionType.HalfTime:
                    if (levelHalfTime)
                    {
                        mission.FinishMission();
                    }
                    break;
                case MissionType.NoMiss:
                    if (noMisses)
                    {
                        mission.FinishMission();
                    }
                    break;
                default:
                    break;

            }
        }
    }

    public void RecordHigscore(int numTiles)
    {
        destroyedTilesNum += numTiles;
    }
    public void RecordHalfTime(bool inHalfTime)
    {
        levelHalfTime = inHalfTime;
    }
    public void RecordMiss ()
    {
        noMisses = false;
    }

    private void DecideActiveMissions()
    {
        for (int i = 0; i < allMissionsList.Count; i++)
        {
            if (allMissionsList[i].IsAvailable())
            {
                activeMissionsList.Add(allMissionsList[i]);
            }
        }
    }

    public void OnOpenMissionsUI()
    { 
        missionsUI.gameObject.SetActive(true);
        missionsUI.CreateUI(activeMissionsList);
    }

    public void OnCloseMissionsUI()
    {
        missionsUI.gameObject.SetActive(false); ;
    }
}
