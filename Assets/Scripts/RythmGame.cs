using System.Collections.Generic;
using UnityEngine;

public class RythmGame : MonoBehaviour
{
    [SerializeField] private GameObject noteSpawnsGO;
    private List<GameObject> noteSpawns = new();
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private float speed;
    [SerializeField] private int limit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < noteSpawnsGO.transform.childCount; i++)
        {
            Debug.Log(noteSpawnsGO.transform.GetChild(i).gameObject.name);
            noteSpawns.Add(noteSpawnsGO.transform.GetChild(i).gameObject);
        }
        SpawnNote();
    }

    void SpawnNote()
    {
        GameObject noteGO = Instantiate(notePrefab, noteSpawns[1].transform);
        Note note = noteGO.GetComponent<Note>();
        note.speed = speed;
        note.limit = limit;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
