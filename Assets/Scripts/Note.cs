using UnityEngine;

[System.Serializable]
public class Note : MonoBehaviour
{
    private float speed = 0.5f;
    private float limit = 500;
    public int stringNum;
    public float duration;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0,-speed,0);
        float yRectPosition = GetComponent<RectTransform>().localPosition.y;
        if (yRectPosition<-limit)
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
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
    public float secondToPlay;
}
