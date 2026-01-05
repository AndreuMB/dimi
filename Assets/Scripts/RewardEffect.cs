using UnityEngine;

[CreateAssetMenu(fileName = "RewardEffect", menuName = "Scriptable Objects/RewardsSO/RewardEffect")]
public abstract class RewardEffect : ScriptableObject
{
    public abstract void Give(Player player);
}
