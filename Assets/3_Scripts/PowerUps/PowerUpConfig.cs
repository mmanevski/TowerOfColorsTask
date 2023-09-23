using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp Config")]
public class PowerUpConfig : ScriptableObject
{
    [SerializeField]
    int PowerUpId = 1;

    public PowerUpType type;

    public Sprite image;
  
    public int value;

    int NumberOfUses = 3;

 

    string SaveNumberOfUses = "NumberOfUsesPU";
    string SaveValue = "ValueOfPU";

    public void SetUpPowerUp(int numOfUses)
    {
        if (!PlayerPrefs.HasKey(SaveNumberOfUses + PowerUpId))
        {
            NumberOfUses = numOfUses;
            PlayerPrefs.SetInt(SaveNumberOfUses + PowerUpId, NumberOfUses);
        }
        else
        {
            NumberOfUses = PlayerPrefs.GetInt(SaveNumberOfUses + PowerUpId, NumberOfUses); //TODO: test!!!
        }
    }

    public void SetUpPowerUp(int numOfUses, int val)
    {
        if (!PlayerPrefs.HasKey(SaveValue + PowerUpId))
        {
            value = val;
            PlayerPrefs.SetInt(SaveValue + PowerUpId, val);
        }
        else
        {
            NumberOfUses = PlayerPrefs.GetInt(SaveNumberOfUses + PowerUpId, NumberOfUses); //TODO: test!!!
        }


    }

    public bool IsAvailable()
    {
        return NumberOfUses != 0;
    }

    public int GetNumOfUses()
    { 
       return NumberOfUses; 
    }

    public void UsePowerUp()
    {
        NumberOfUses--;
        PlayerPrefs.SetInt(SaveNumberOfUses + PowerUpId, NumberOfUses);
        Debug.Log("Power up " + PowerUpId + " used. Uses left: " + NumberOfUses);

    }

    
}
