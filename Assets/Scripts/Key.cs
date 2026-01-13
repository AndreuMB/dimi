using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Key : MonoBehaviour
{
    [SerializeField] Color keyDefaultColor;
    [SerializeField] Color keyPressColor;
    [SerializeField] GameObject reactiveKeyboardGO;
    [SerializeField] GameObject reactiveGamepadGO;
    [SerializeField] GameObject keyboardHint;
    [SerializeField] GameObject gamepadHint;
    GameObject currentReactiveGO;
    Vector3 insideKeyRT;

    void Start()
    {
        // insideKeyRT = insideKeyGO.GetComponent<RectTransform>().localScale;
        // reactiveKeyboardGO.GetComponent<Image>().color = keyDefaultColor;
    }


    public void KeyPress()
    {
        currentReactiveGO.GetComponent<Image>().color = keyPressColor;
        // if (gamepadHint.activeInHierarchy) 
        currentReactiveGO.gameObject.GetComponent<RectTransform>().localScale = new(0.9f, 0.9f, 0.9f);
    }

    public void KeyRelease()
    {
        currentReactiveGO.GetComponent<Image>().color = keyDefaultColor;
        currentReactiveGO.gameObject.GetComponent<RectTransform>().localScale = new(1, 1, 1);

    }

    public void SetGamepad(bool gamepad)
    {
        gamepadHint.SetActive(gamepad);
        keyboardHint.SetActive(!gamepad);
        currentReactiveGO = gamepad ? reactiveGamepadGO : reactiveKeyboardGO;
    }
}