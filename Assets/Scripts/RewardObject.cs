using UnityEngine;

[CreateAssetMenu(fileName = "RewardObject", menuName = "Scriptable Objects/RewardsSO/RewardObject")]
public class RewardObject : RewardEffect
{
    public RewardObjectName rewardObjectName;

    public override void Give(CharacterController player)
    {
        player.EnableObjectReward(rewardObjectName);
    } 
}

public enum RewardObjectName
{
    headband,
    wings
}