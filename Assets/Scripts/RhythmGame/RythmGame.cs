using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject keysContainer;
    private List<GameObject> keysList = new();
    public GameObject scoreScreen;
    public GameObject player;
    private Transform lastTransformPlayerMap;



    void Start()
    {
        // scoreScreen.SetActive(false);
        for (int i = 0; i < noteSpawnsGO.transform.childCount; i++)
        {
            noteSpawns.Add(noteSpawnsGO.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < keysContainer.transform.childCount; i++)
        {
            keysList.Add(keysContainer.transform.GetChild(i).gameObject);
        }
    }

    public void StartSong()
    {
        scoreScreen.SetActive(false);
        timer = 0;
        score = 0;
        noteIndexToPlay = 0;
        noteIndexToSpawn = 0;
        currentSong = songs[1];
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
        bool notesLeft = true;
        // while (song.songDurationSeconds > timer)
        while (notesLeft)
        {
            yield return new WaitForSeconds(step);
            timerTMP.text = TimerToMinutes();
            timer += step;
            CheckForNote(song);
            if (currentNote == null) currentNote = currentSong.notes[noteIndexToPlay];
            if (timer > currentNote.secondToPlay)
            {
                if (!NextNote()) notesLeft = false;
            }
        }

        FinishSong();
    }

    bool NextNote()
    {
        noteIndexToPlay++;
        if (noteIndexToPlay < currentSong.notes.Length)
        {
            currentNote = currentSong.notes[noteIndexToPlay];
        }
        else
        {
            return false;
        }
        return true;
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

        int keyNum = 0;

        if (callbackContext.canceled)
        {
            if (int.TryParse(callbackContext.control.displayName, out keyNum))
            {
                // was successful key exist
                keysList[keyNum - 1].GetComponent<Key>().KeyRelease();
            }
        }

        if (callbackContext.started)
        {
            if (int.TryParse(callbackContext.control.displayName, out keyNum))
            {
                // was successful key exist
                keysList[keyNum - 1].GetComponent<Key>().KeyPress();
            }


            if (callbackContext.control.displayName == currentNote.stringNum.ToString())
            {
                ScoreSystem();
            }
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

        if (notesGOList.Count > noteIndexToPlay) DestroyImmediate(notesGOList[noteIndexToPlay]);
        NextNote();
    }

    string TimerToMinutes()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);

        string timeFormat = string.Format("{0:0}:{1:00}", minutes, seconds);
        return timeFormat;
    }

    void FinishSong()
    {
        scoreScreen.SetActive(true);
        scoreScreen.GetComponent<ScoreScreen>().SetScore(score + "");
        if (score >= currentSong.scoreToPass)
        {
            Debug.Log("pass song");
        }
        else
        {
            Debug.Log("Try Again");
            scoreScreen.GetComponent<ScoreScreen>().ShowSubText();

        }

    }

    public void ExitSong()
    {
        player.GetComponent<Player>().ReturnPlayer();
    }

}
