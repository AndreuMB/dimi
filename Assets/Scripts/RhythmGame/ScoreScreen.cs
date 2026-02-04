using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ScoreScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreTMP;
    [SerializeField] private GameObject subText;
    // public UnityEvent<SongSO> restartEvent;

    void OnEnable()
    {
        scoreTMP.text = 0 + "";
        subText.SetActive(false);
    }

    public void SetScore(string score)
    {
        scoreTMP.text = score;
    }

    // public void SetSubText(string subText)
    // {

    // }
    public void ShowSubText()
    {
        subText.SetActive(true);
    }

}
