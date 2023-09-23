using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{

    [SerializeField]
    MissionsUI missionsUI;

    public void OnOpenMissionsUI()
    { 
        missionsUI.gameObject.SetActive(true);
    }

    public void OnCloseMissionsUI()
    {
        missionsUI.gameObject.SetActive(false); ;
    }
}
