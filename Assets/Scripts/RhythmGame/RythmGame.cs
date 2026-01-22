using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] AudioSource songSource;
    [SerializeField] int startFromNote = 0;
    // bool startingSong;
    // float spawnTime;
    // float songStartDSPTime;




    void Start()
    {
        for (int i = 0; i < noteSpawnsGO.transform.childCount; i++)
        {
            noteSpawns.Add(noteSpawnsGO.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < keysContainer.transform.childCount; i++)
        {
            keysList.Add(keysContainer.transform.GetChild(i).gameObject);
        }
    }

    void OnEnable()
    {
        scoreScreen.SetActive(false);
    }

    public void StartSong()
    {
        StopAllCoroutines();
        scoreScreen.SetActive(false);
        timer = 0;
        score = 0;
        scoreTMP.text = score.ToString();
        currentSong = songs[0];
        noteIndexToPlay = startFromNote < currentSong.notes.Length ? startFromNote : 0;
        noteIndexToSpawn = startFromNote < currentSong.notes.Length ? startFromNote : 0;
        currentNote = currentSong.notes[noteIndexToPlay];
        EmptyAllStringsNotes();
        UpdateStringsHint(player.GetComponent<Player>().IsUsingGamepad());
        timerTMP.text = TimerToMinutes();
        StartCoroutine(StartSongAudio(currentSong));
    }

    void EmptyAllStringsNotes()
    {
        foreach (GameObject bassString in noteSpawns)
        {
            EmptyStringNotes(bassString);
        }
    }

    void EmptyStringNotes(GameObject bassString)
    {
        foreach (Transform child in bassString.transform)
        {
            Destroy(child.gameObject);
        }
    }

    IEnumerator StartSongAudio(SongSO song)
    {
        // startingSong = true;
        songSource.clip = song.songFile;
        float startTime = currentSong.notes[startFromNote].secondToPlay - currentSong.speed;
        StartCoroutine(TimerCount());

        if (startTime <= 0)
        {
            songSource.time = 0;
            // float secondToSpawnFirstNote = currentSong.notes[0].secondToPlay - currentSong.speed;
            // float offset = 0 - startTime;
            Debug.Log(Math.Abs(startTime));
            yield return new WaitForSeconds(Math.Abs(startTime));
        }
        else
        {
            songSource.time = startTime;
        }

        songSource.Play();
        // gameTime = -currentSong.speed;

        // songStartDSPTime = (float)AudioSettings.dspTime;

        yield return null;
    }

    IEnumerator TimerCount()
    {
        NoteData currentNoteToSpawn;

        while (songSource.clip.length > songSource.time)
        {
            // gameTime = (float)(AudioSettings.dspTime - songStartDSPTime) - currentSong.speed;
            timer = songSource.time;
            // timer = gameTime;
            timerTMP.text = TimerToMinutes();
            // no more notes to play
            if (noteIndexToSpawn < currentSong.notes.Length)
            {
                currentNoteToSpawn = currentSong.notes[noteIndexToSpawn];
                float secondToSpawn = currentNoteToSpawn.secondToPlay - currentSong.speed;

                if (secondToSpawn <= timer)
                {
                    // CheckForNote(currentSong, noteIndex);
                    SpawnNote(currentNoteToSpawn.stringNum);
                    noteIndexToSpawn++;
                }
            }
            yield return null;

        }
        FinishSong();
    }

    void SpawnNote(int noteString)
    {
        GameObject noteGO = Instantiate(notePrefab, noteSpawns[noteString - 1].transform);
        Note note = noteGO.GetComponent<Note>();
        note.triggerStringPosition = triggerString.transform.position;
        note.SetSecondsToReachTarget(currentSong.speed);
        note.SetLimit(limit);
        notesGOList.Add(noteGO);
        note.OnNoteMissed += NextCurrentNote;
    }

    // IEnumerator TimerCount(SongSO song)
    // {
    //     float step = 0.1f;
    //     bool notesLeft = true;
    //     float offset = currentSong.notes[noteIndexToSpawn].secondToSpawn;
    //     timer = offset;

    //     while (notesLeft)
    //     {
    //         yield return new WaitForSeconds(step);
    //         timerTMP.text = TimerToMinutes();
    //         timer += step;
    //         CheckForNote(song);
    //         if (currentNote == null) currentNote = currentSong.notes[noteIndexToPlay];
    //         float secondToPlay = currentNote.secondToSpawn + currentSong.speed;
    //         currentNote.secondToPlay = secondToPlay;
    //         if (timer > secondToPlay)
    //         {
    //             if (!NextNote()) notesLeft = false;
    //         }
    //     }

    //     FinishSong();
    // }

    // bool NextNote()
    // {
    //     noteIndexToPlay++;
    //     if (noteIndexToPlay < currentSong.notes.Length)
    //     {
    //         currentNote = currentSong.notes[noteIndexToPlay];
    //     }
    //     else
    //     {
    //         return false;
    //     }
    //     return true;
    // }

    // void CheckForNote(SongSO song, int noteIndex)
    // {
    //     if (noteIndex >= currentSong.notes.Length) return;
    //     NoteData note = song.notes[noteIndex];
    //     if (note.secondToSpawn <= timer)
    //     {
    //         // note.secondToPlay = note.secondToSpawn + secondsNoteToString;
    //         // note.secondToPlay = note.secondToSpawn + currentSong.speed;
    //         SpawnNote(note.stringNum);
    //         // noteIndexToSpawn++;
    //     }


    // }

    public void HandlePlayerInput(InputAction.CallbackContext callbackContext)
    {
        if (currentNote == null) return;

        int keyNum;
        bool gamepad = false;

        switch (callbackContext.action.name)
        {
            case "Key1":
                keyNum = 1;
                break;
            case "Key2":
                keyNum = 2;
                break;
            case "Key3":
                keyNum = 3;
                break;
            case "Key4":
                keyNum = 4;
                break;
            default:
                return;
        }

        if (callbackContext.control.device is Gamepad) gamepad = true;

        if (callbackContext.canceled)
        {
            keysList[keyNum - 1].GetComponent<Key>().KeyRelease();
        }

        if (callbackContext.started)
        {
            UpdateStringsHint(gamepad);
            keysList[keyNum - 1].GetComponent<Key>().KeyPress();


            if (keyNum == currentNote.stringNum)
            {
                ScoreSystem();
            }
        }


    }

    void ScoreSystem()
    {
        float accuracy = currentNote.secondToPlay - timer;
        // Debug.Log("currentNote.secondToPlay = " + currentNote.secondToPlay);
        Debug.Log("timer = " + timer);
        Debug.Log("accuracy = " + accuracy);
        if (accuracy > 0.6)
        {
            // above 0.6 doesnt destroy note
            Debug.Log("No valid");
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

        DestroyImmediate(notesGOList[noteIndexToPlay]);
        NextCurrentNote();

        // if (notesGOList.Count > noteIndexToPlay) DestroyImmediate(notesGOList[noteIndexToPlay]);
        // NextNote();
    }

    public void NextCurrentNote()
    {
        noteIndexToPlay++;
        if (noteIndexToPlay >= currentSong.notes.Length) return;
        currentNote = currentSong.notes[noteIndexToPlay];
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

    void UpdateStringsHint(bool gamepad)
    {
        keysList.ForEach(key =>
        {
            key.GetComponent<Key>().SetGamepad(gamepad);
        });
    }
}