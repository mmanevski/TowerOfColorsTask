using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MissionType
{ 
    DestroyTiles,
    HalfTime, 
    HalfBalls
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
    bool LevelHalfTime = false;
    int DestroyedTilesNum = 0;
    bool IsHalfBalls = false;


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
        LevelHalfTime = true;
        DestroyedTilesNum = 0;
        IsHalfBalls = true;
    }

    private void DecideRewards()
    { 
        foreach (MissionConfig mission in activeMissionsList)
        {
            switch (mission.missionType)
            {
                case MissionType.DestroyTiles:
                    if (DestroyedTilesNum > mission.goal)
                    {
                        FinishMission(mission);
                    }
                    break;
                case MissionType.HalfTime:
                    if (LevelHalfTime)
                    {
                        FinishMission(mission);
                    }
                    break;
                case MissionType.HalfBalls:
                    if (IsHalfBalls)
                    {
                        FinishMission(mission);
                    }
                    break;
                default:
                    break;

            }
        }
    }

    private void FinishMission(MissionConfig mission)
    {
        mission.FinishMission();
        GameManager.Instance.HandleReward(mission.reward);
    }
         
    public void RecordHigscore(int numTiles)
    {
        DestroyedTilesNum += numTiles;
    }
    public void RecordHalfTime(bool inHalfTime)
    {
        LevelHalfTime = inHalfTime;
    }
    public void RecordHalfBalls (bool isHalfBalls)
    {
        IsHalfBalls = isHalfBalls;
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
