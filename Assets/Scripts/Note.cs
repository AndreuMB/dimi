using UnityEngine;

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
}



[System.Serializable]
public class NoteData
{
    public int stringNum;
    public float secondToSpawn;
    public float secondToPlay;
}
