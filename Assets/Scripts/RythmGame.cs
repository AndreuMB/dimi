using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RythmGame : MonoBehaviour
{
    // Bass strings spawn gameobject
    [SerializeField] private GameObject noteSpawnsGO;
    // Bass strings spawn gameobject list
    private List<GameObject> noteSpawns = new();
    [SerializeField] private GameObject notePrefab;
    // [SerializeField] private float speed;
    // to control the speed
    [SerializeField] private float secondsNoteToString;
    [SerializeField] private int limit;
    [SerializeField] private List<SongSO> songs;
    private float timer = 0;
    [SerializeField] private TMP_Text timerTMP;
    private SongSO currentSong;
    private NoteData currentNote;
    private List<GameObject> notesGOList = new();
    [SerializeField] GameObject triggerString;
    int noteIndexToSpawn = 0;
    int noteIndexToPlay = 0;
    int score = 0;
    [SerializeField] private TMP_Text scoreTMP;

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
        score = 0;
        noteIndexToPlay = 0;
        currentSong = songs[0];
        StartCoroutine(TimerCount(currentSong));
    }

    void SpawnNote(int noteString)
    {
        GameObject noteGO = Instantiate(notePrefab, noteSpawns[noteString - 1].transform);
        Note note = noteGO.GetComponent<Note>();
        note.triggerStringPosition = triggerString.transform.position;
        note.SetSecondsToReachTarget(secondsNoteToString);
        note.SetLimit(limit);
        notesGOList.Add(noteGO);
    }

    IEnumerator TimerCount(SongSO song)
    {
        float step = 0.1f;

        while (song.songDurationSeconds > timer)
        {
            yield return new WaitForSeconds(step);
            timerTMP.text = timer.ToString();
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
            ScoreSystem();
        }

    }

    void ScoreSystem()
    {
        float accuracy = currentNote.secondToPlay - timer;
        Debug.Log("accuracy = " + accuracy);
        if (accuracy > 0.6)
        {
            // above 0.6 doesnt destroy note
            Debug.Log("Bad");
            return;
        }
        else if (accuracy > 0.4)
        {
            Debug.Log("Meh");
            score += 25;
        }
        else if (accuracy > 0.3)
        {
            score += 50;
            Debug.Log("Nice");
        }
        else if (accuracy > 0.15)
        {
            score += 75;
            Debug.Log("Good!");
        }
        else
        {
            score += 100;
            Debug.Log("Perfect!");
        }

        scoreTMP.text = score.ToString();

        if (notesGOList.Count > noteIndexToPlay) Destroy(notesGOList[noteIndexToPlay]);

    }
}
