using UnityEngine;

public class Note : MonoBehaviour
{
    public float speed = 0.5f;
    public float limit = 500;
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
}
