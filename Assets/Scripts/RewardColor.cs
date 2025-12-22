using UnityEngine;

[CreateAssetMenu(fileName = "RewardColor", menuName = "RewardsSO/RewardColor")]
public class RewardColor : RewardEffect
{
    // public string rewardName;
    [SerializeField] private Material newMaterial;


    public override void Give(CharacterController player)
    {
        player.SetNewMaterial(newMaterial);
    } 
}
