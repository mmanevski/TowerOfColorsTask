using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mission Config")]
public class MissionConfig : ScriptableObject
{
    public int id;
    public MissionType missionType;
    public Sprite image;
    public string descriptionText;
    public int goal = 0;
    public MissionDifficulty dificulty;
    public RewardConfig reward;

    private string SaveKey = "Mission";

    public void SetUpMission()
    {
        if (!PlayerPrefs.HasKey(SaveKey + id))
        {
            PlayerPrefs.SetInt(SaveKey + id, 0);
        }
    }

    public bool IsAvailable()
    {
        return PlayerPrefs.GetInt(SaveKey + id, 0) == 0;
    }


    public void FinishMission()
    {
        PlayerPrefs.SetInt(SaveKey + id, 1);
    }

    internal void Reset()
    {
        PlayerPrefs.SetInt(SaveKey + id, 0);
    }
}
