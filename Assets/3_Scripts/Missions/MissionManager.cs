using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    List<MissionConfig> missionsList;

    public void OnOpenMissionsUI()
    { 
        missionsUI.gameObject.SetActive(true);
        missionsUI.CreateUI(missionsList);
    }

    public void OnCloseMissionsUI()
    {
        missionsUI.gameObject.SetActive(false); ;
    }
}
