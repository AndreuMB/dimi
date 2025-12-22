using UnityEngine;

[CreateAssetMenu(fileName = "RewardObject", menuName = "RewardsSO/RewardObject")]
public class RewardObject : RewardEffect
{
    public RewardObjectName rewardObjectName;

    public override void Give(CharacterController player)
    {
        Debug.Log("Run object reward " + rewardObjectName);
        player.EnableObjectReward(rewardObjectName);
    } 
}

public enum RewardObjectName
{
    headband,
    wings
}