using UnityEngine;

[CreateAssetMenu(fileName = "RewardEffect", menuName = "Scriptable Objects/RewardEffect")]
public abstract class RewardEffect : ScriptableObject
{
    public abstract void Give(CharacterController player);
}
