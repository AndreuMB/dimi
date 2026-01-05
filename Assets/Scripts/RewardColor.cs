using UnityEngine;

[CreateAssetMenu(fileName = "RewardColor", menuName = "Scriptable Objects/RewardsSO/RewardColor")]
public class RewardColor : RewardEffect
{
    // public string rewardName;
    [SerializeField] private Material newMaterial;


    public override void Give(Player player)
    {
        player.SetNewMaterial(newMaterial);
    }
}
