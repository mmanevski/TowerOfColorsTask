using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp Config")]
public class PowerUpConfig : ScriptableObject
{
    [SerializeField]
    int PowerUpId = 1;
    
    int NumberOfUses = 3;

    string SaveNumberOfUses = "NumberOfUsesPU";



    public void SetUpPowerUp(int numOfUses)
    {
        if (!PlayerPrefs.HasKey(SaveNumberOfUses + PowerUpId))
        {
            NumberOfUses = numOfUses;
            PlayerPrefs.SetInt(SaveNumberOfUses + PowerUpId, NumberOfUses);
        }
    }

    public bool IsAvailable()
    {
        return NumberOfUses != 0;
    }


    public void UsePowerUp()
    {
        NumberOfUses--;
        PlayerPrefs.SetInt(SaveNumberOfUses + PowerUpId, NumberOfUses);
        Debug.Log("Power up " + PowerUpId + " used. Uses left: " + NumberOfUses);

    }

    
}
