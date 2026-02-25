using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Key : MonoBehaviour
{
    // [SerializeField] Color keyDefaultColor;
    // [SerializeField] Color keyPressColor;
    // [SerializeField] GameObject reactiveKeyboardGO;
    // [SerializeField] GameObject reactiveGamepadGO;
    // [SerializeField] GameObject keyboardHint;
    // [SerializeField] GameObject gamepadHint;
    // GameObject currentReactiveGO;
    [SerializeField] Sprite eyeOpen;
    [SerializeField] Sprite eyeClosed;
    // Vector3 insideKeyRT;
    // Animator keyAnimator;

    void Start()
    {
        // keyAnimator = gameObject.GetComponent<Animator>();
        GetComponent<Image>().sprite = eyeOpen;
        // insideKeyRT = insideKeyGO.GetComponent<RectTransform>().localScale;
        // reactiveKeyboardGO.GetComponent<Image>().color = keyDefaultColor;
    }


    public void KeyPress()
    {
        // keyAnimator.SetTrigger("Close");
        GetComponent<Image>().sprite = eyeClosed;

        // currentReactiveGO.GetComponent<Image>().color = keyPressColor;
        // if (gamepadHint.activeInHierarchy) 
        // currentReactiveGO.gameObject.GetComponent<RectTransform>().localScale = new(0.9f, 0.9f, 0.9f);
    }

    public void KeyRelease()
    {
        // keyAnimator.SetTrigger("Open");
        GetComponent<Image>().sprite = eyeOpen;

        // currentReactiveGO.GetComponent<Image>().color = keyDefaultColor;
        // currentReactiveGO.gameObject.GetComponent<RectTransform>().localScale = new(1, 1, 1);

    }

    public void SetGamepad(bool gamepad)
    {
        // gamepadHint.SetActive(gamepad);
        // keyboardHint.SetActive(!gamepad);
        // currentReactiveGO = gamepad ? reactiveGamepadGO : reactiveKeyboardGO;
    }
}