using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionsUI : MonoBehaviour
{
    [SerializeField]
    List<MissionButton> missionButtonList;

    internal void CreateUI(List<MissionConfig> missionsList)
    {
        for (int i = 0; i < missionButtonList.Count; i++)
        {
            missionButtonList[i].SetRewardButton(missionsList[i]);
        }
    }
}
