using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Note : MonoBehaviour
{
    private float speed = 2.5f;
    private float secondsToReachTarget = 4;
    private float limit = 500;
    public int stringNum;
    public float duration;
    float t;
    Vector3 startPosition;
    Vector3 targetPosition;
    public Vector3 triggerStringPosition;
    [SerializeField] Color keyDefaultColor;
    [SerializeField] Color keyPressColor;
    [SerializeField] Image insideKeyImg;
    public event Action OnNoteMissed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        targetPosition = new(transform.position.x, triggerStringPosition.y);
    }

    void Update()
    {
        t += Time.deltaTime / secondsToReachTarget;
        transform.position = Vector3.Lerp(startPosition, targetPosition, t);
        if (transform.position.y <= targetPosition.y)
        {
            OnNoteMissed.Invoke();
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetSecondsToReachTarget(float newSeconds)
    {
        secondsToReachTarget = newSeconds;
    }

    public void SetLimit(float newLimit)
    {
        limit = newLimit;
    }

    public void KeyPress()
    {
        insideKeyImg.color = keyPressColor;
    }

    public void KeyRelease()
    {
        insideKeyImg.color = keyDefaultColor;
    }
}



// [System.Serializable]
// public class NoteData
// {
//     public int stringNum;
//     [NonSerialized] public float secondToSpawn;
//     public float secondToPlay;
// }
