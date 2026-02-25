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
    // public Vector3 triggerStringPosition;
    [SerializeField] Color keyDefaultColor;
    [SerializeField] Color keyPressColor;
    [SerializeField] Image insideKeyImg;
    public event Action OnNoteMissed;

    public int bassString;
    // public int beat;
    public int beatToPlay;
    Vector3 origin;
    Vector3 destination;

    // public Note(int bassString, int beat, Vector3 origin, Vector3 destination)
    // {
    //     this.bassString = bassString;
    //     // this.beat = beat;
    //     this.origin = origin;
    //     this.destination = destination;
    // }

    public void SetNote(int bassString, int beat, Vector3 origin, Vector3 destination)
    {
        this.bassString = bassString;
        // this.beat = beat;
        this.origin = origin;
        this.destination = destination;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // startPosition = origin;
        transform.position = origin;
        // targetPosition = new(transform.position.x, triggerStringPosition.y);
    }

    void Update()
    {
        const float EXTRA_OFFSET = 0.4f;
        t += Time.deltaTime / secondsToReachTarget;
        transform.position = Vector3.Lerp(origin, destination, t);
        if (transform.position.y <= destination.y + EXTRA_OFFSET)
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
