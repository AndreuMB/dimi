using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject gamepadHint;
    [SerializeField] private GameObject keyboardHint;
    private GameObject currentHint;
    [SerializeField] private Material whiteMaterial;
    [SerializeField] public Rewards rewardNumber;
    [SerializeField] private RewardEffect effect;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gamepadHint.SetActive(false);
        keyboardHint.SetActive(false);
        currentHint = gamepadHint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "PlayerCollider") return;
        if (other.transform.parent.GetComponent<Player>().IsUsingGamepad())
        {
            currentHint = gamepadHint;
        }
        else
        {
            currentHint = keyboardHint;

        }
        Debug.Log(currentHint.name);
        currentHint.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag != "PlayerCollider") return;

        currentHint.SetActive(false);

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
