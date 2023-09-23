using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionButton : MonoBehaviour
{
    [SerializeField]
    Image missionImage;
    [SerializeField]
    TextMeshProUGUI missionText;
    [SerializeField]
    TextMeshProUGUI missionReward;
    [SerializeField]
    TextMeshProUGUI difficultyMarker;
    [SerializeField]
    Image notAvailable;

    MissionConfig setMission;

    public void SetRewardButton(MissionConfig mission)
    {
        missionImage.sprite = mission.image;
        missionText.text = mission.descriptionText;
        missionReward.text = " + " + mission.reward.amount + " " + mission.reward.descriptionText;

        setMission = mission;

        switch (mission.dificulty)
        {
            case MissionDifficulty.Easy:
                difficultyMarker.text = "Easy";
                difficultyMarker.color = Color.green;
                break;
            case MissionDifficulty.Medium:
                difficultyMarker.text = "Medium";
                difficultyMarker.color = Color.blue;
                break;
            case MissionDifficulty.Hard:
                difficultyMarker.text = "Hard";
                difficultyMarker.color = Color.red;
                break;
            default:
                break;
        } 
    }

    void Start()
    {
        if (setMission != null)
        {
            notAvailable.gameObject.SetActive(!setMission.IsAvailable());
        }
    }
}
