using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Active Missions Holder")]
public class ActiveMissionsHolder : ScriptableObject
{
    public List<MissionConfig> activeMissionsList = new List<MissionConfig>();
}
