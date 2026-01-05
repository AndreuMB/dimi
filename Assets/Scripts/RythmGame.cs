using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RythmGame : MonoBehaviour
{
    [SerializeField] private GameObject noteSpawnsGO;
    private List<GameObject> noteSpawns = new();
    [SerializeField] private GameObject notePrefab;
    // [SerializeField] private float speed;
    // to control the speed
    [SerializeField] private float secondsNoteToString;
    [SerializeField] private int limit;
    [SerializeField] private List<SongSO> songs;
    private float timer = 0;
    [SerializeField] private TMP_Text timerGO;
    private SongSO currentSong;
    private NoteData currentNote;
    [SerializeField] GameObject triggerString;
    int noteIndexToSpawn = 0;
    int noteIndexToPlay = 0;
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
        noteIndexToPlay = 0;
        currentSong = songs[0];
        StartCoroutine(TimerCount(currentSong));
    }

    void SpawnNote(int noteString)
    {
        GameObject noteGO = Instantiate(notePrefab, noteSpawns[noteString - 1].transform);
        Note note = noteGO.GetComponent<Note>();
        note.triggerStringPosition = triggerString.transform.position;
        // note.SetSpeed(speed);
        note.SetSecondsToReachTarget(secondsNoteToString);
        note.SetLimit(limit);
    }

    IEnumerator TimerCount(SongSO song)
    {
        float step = 0.1f;

        while (song.songDurationSeconds > timer)
        {
            yield return new WaitForSeconds(step);
            timerGO.text = timer.ToString();
            timer += step;
            CheckForNote(song);
            if (currentNote == null) currentNote = currentSong.notes[noteIndexToPlay];
            if (timer > currentNote.secondToPlay)
            {
                noteIndexToPlay++;
                if (noteIndexToPlay < currentSong.notes.Length)
                {
                    currentNote = currentSong.notes[noteIndexToPlay];
                }
                else
                {
                    Debug.Log("No more notes");
                    yield break;
                }
            }
        }
    }

    void CheckForNote(SongSO song)
    {
        // NoteData note = song.notes.FirstOrDefault(note =>
        // {
        //     if (note.secondToSpawn >= timer && !note.spawned)
        //     {
        //         note.spawned = true;
        //         return true;
        //     }
        //     return false;
        // }
        // );
        if (noteIndexToSpawn >= currentSong.notes.Length) return;
        NoteData note = song.notes[noteIndexToSpawn];
        if (note.secondToSpawn <= timer)
        {
            note.secondToPlay = note.secondToSpawn + secondsNoteToString;
            SpawnNote(note.stringNum);
            noteIndexToSpawn++;
        }


    }

    public void HandlePlayerInput(InputAction.CallbackContext callbackContext)
    {
        if (currentNote == null) return;
        if (!callbackContext.performed) return;


        if (callbackContext.control.displayName == currentNote.stringNum.ToString())
        {
            Debug.Log("timer = " + timer);
            Debug.Log("secondToPlay = " + currentNote.secondToPlay);
            ScoreSystem();
            // if (timer == currentNote.secondToPlay)
            // {
            //     Debug.Log("perfect!!!");
            //     return;
            // }

        }

    }

    void ScoreSystem()
    {
        float accuracy = currentNote.secondToPlay - timer;
        Debug.Log("accuracy = " + accuracy);
        if (accuracy > 1)
        {
            Debug.Log("Bad");
        }
        else if (accuracy > 0.5)
        {
            Debug.Log("Meh");
        }
        else if (accuracy > 0.2)
        {
            Debug.Log("Good");
        }
        else
        {
            Debug.Log("Perfect!");
        }
    }
}
