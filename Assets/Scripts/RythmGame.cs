using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RythmGame : MonoBehaviour
{
    [SerializeField] private GameObject noteSpawnsGO;
    private List<GameObject> noteSpawns = new();
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private float speed;
    [SerializeField] private int limit;
    [SerializeField] private List<SongSO> songs;
    private int timer = 0;
    [SerializeField] private TMP_Text timerGO;
    private SongSO currentSong;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < noteSpawnsGO.transform.childCount; i++)
        {
            noteSpawns.Add(noteSpawnsGO.transform.GetChild(i).gameObject);
        }
        // StartCoroutine(PlaySong(currentSong));
    }

    void OnEnable()
    {
        timer = 0;
        currentSong = songs[0];
        StartCoroutine(TimerCount(currentSong));
    }

    void SpawnNote(int noteString)
    {
        GameObject noteGO = Instantiate(notePrefab, noteSpawns[noteString - 1].transform);
        Note note = noteGO.GetComponent<Note>();
        note.SetSpeed(speed);
        note.SetLimit(limit);
    }

    IEnumerator TimerCount(SongSO song)
    {

        while (song.songDurationSeconds > timer)
        {
            yield return new WaitForSeconds(1);
            timerGO.text = timer.ToString();
            timer++;
            CheckForNote(song);
        }
    }

    IEnumerator PlaySong(SongSO song)
    {
        foreach (NoteData note in song.notes)
        {
            yield return new WaitForSeconds(note.secondToPlay);
            SpawnNote(note.stringNum);
        }
    }

    void CheckForNote(SongSO song)
    {
        NoteData note = song.notes.FirstOrDefault(note => note.secondToPlay == timer);
        Debug.Log("note = " + note);
        if (note != null) SpawnNote(note.stringNum);
    }
}
