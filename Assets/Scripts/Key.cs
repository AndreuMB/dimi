using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Key : MonoBehaviour
{
    [SerializeField] Color keyDefaultColor;
    [SerializeField] Color keyPressColor;
    [SerializeField] GameObject insideKeyGO;
    Vector3 insideKeyRT;

    void Start()
    {
        insideKeyRT = insideKeyGO.GetComponent<RectTransform>().localScale;
        insideKeyGO.GetComponent<Image>().color = keyDefaultColor;
    }


    public void KeyPress()
    {
        insideKeyGO.GetComponent<Image>().color = keyPressColor;
        insideKeyGO.gameObject.GetComponent<RectTransform>().localScale = new(0.75f, 0.75f, 0.75f);
    }

    public void KeyRelease()
    {
        insideKeyGO.GetComponent<Image>().color = keyDefaultColor;
        insideKeyGO.gameObject.GetComponent<RectTransform>().localScale = new(0.80f, 0.80f, 0.80f);

    }
}