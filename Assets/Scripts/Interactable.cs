using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject buttonHint;
    [SerializeField] private Material whiteMaterial;
    [SerializeField] public Rewards rewardNumber;
    [SerializeField] private RewardEffect effect;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonHint.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag != "Player") return;

        buttonHint.SetActive(true);
        // interactableClose = true;
        // interactable = other.gameObject;

    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag != "Player") return;

        buttonHint.SetActive(false);

        // interactableClose = false;
        // interactable = null;

    }

    public Material GetInteractableWhiteColor()
    {
        return whiteMaterial;
    }

    public bool EnableInteractableHeatband()
    {
        return true;
    }

    public void Interact(Player characterController)
    {
        effect.Give(characterController);
    }
}

public enum Rewards
{
    WhiteMaterial,
    Headband,

}
