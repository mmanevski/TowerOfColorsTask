﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField]
    int numberOfActiveMissions = 3;

    //keeping stats for deciding rewards
    bool LevelHalfTime = false;
    int DestroyedTilesNum = 0;
    bool IsHalfBalls = false;



    private void Start()
    {
        CheckIfEnoughMissions();
        SetUp();
    }

    private void SetUp()
    {

        DecideActiveMissions();
        ResetRewardStats();
    }

    private void CheckIfEnoughMissions()
    {
        int counter = 0;

        foreach (var mission in allMissionsList)
        {
            if (mission.IsAvailable())
            { 
                counter++;
                if (counter == numberOfActiveMissions)
                    break;
            }
        }

        if (counter < numberOfActiveMissions)
        {
            ResetMissions();
        }
    }

    private void ResetMissions()
    {
        foreach (var mission in allMissionsList)
        {
            mission.Reset();
        }
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
        MissionConfig firstMission = allMissionsList.FirstOrDefault(i => i.dificulty == MissionDifficulty.Easy && i.IsAvailable());
        if (firstMission == null)
        {
            firstMission = allMissionsList.FirstOrDefault(i => i.IsAvailable());
        }
        MissionConfig secondMission = allMissionsList.FirstOrDefault(i => i.dificulty == MissionDifficulty.Medium && i.IsAvailable());
        if (secondMission == null)
        {
            secondMission = allMissionsList.FirstOrDefault(i => i.IsAvailable());
        }
        MissionConfig thirdMission = allMissionsList.FirstOrDefault(i => i.dificulty == MissionDifficulty.Hard && i.IsAvailable());
        if (thirdMission == null)
        {
            thirdMission = allMissionsList.FirstOrDefault(i => i.IsAvailable());
        }

        activeMissionsList.Clear();
        
        activeMissionsList.Add(firstMission);
        activeMissionsList.Add(secondMission);
        activeMissionsList.Add(thirdMission);

        /*
        for (int i = 0; i < allMissionsList.Count; i++)
        {
            if (allMissionsList[i].IsAvailable())
            {
                activeMissionsList.Add(allMissionsList[i]);
            }
        }
        */
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
