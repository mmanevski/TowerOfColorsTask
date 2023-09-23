using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Reward Config")]
public class RewardConfig : ScriptableObject
{
    public string descriptionText;
    public int amount;
    public RewardType type;
}
