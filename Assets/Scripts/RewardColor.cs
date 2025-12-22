using UnityEngine;

[CreateAssetMenu(fileName = "RewardColor", menuName = "RewardsSO/RewardColor")]
public class RewardColor : RewardEffect
{
    // public string rewardName;
    [SerializeField] private Material newMaterial;


    public override void Give(CharacterController player)
    {
        // Debug.Log("Run sppecific reward" + rewardName);
        player.SetNewMaterial(newMaterial);
    } 
}
