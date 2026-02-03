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
    // [SerializeField] Song songtest;
    // bool startingSong;
    // float spawnTime;
    // float songStartDSPTime;




    void Start()
    {
        Song2();
        for (int i = 0; i < noteSpawnsGO.transform.childCount; i++)
        {
            noteSpawns.Add(noteSpawnsGO.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < keysContainer.transform.childCount; i++)
        {
            keysList.Add(keysContainer.transform.GetChild(i).gameObject);
        }
    }

    Song Song2()
    {
        Song song = new();
        song.bpm = 94;
        song.notes.Add(new Note2(1, 5));
        song.notes.Add(new Note2(1, 7));
        song.notes.Add(new Note2(2, 9));
        song.notes.Add(new Note2(4, 11));
        song.notes.Add(new Note2(4, 13));
        song.notes.Add(new Note2(4, 15));
        song.notes.Add(new Note2(4, 17));
        song.notes.Add(new Note2(4, 19));
        song.notes.Add(new Note2(4, 25));
        song.notes.Add(new Note2(4, 27));
        song.notes.Add(new Note2(4, 29));
        song.notes.Add(new Note2(4, 31));
        return song;
    }

    void OnEnable()
    {
        UpdateStringsHint(player.GetComponent<Player>().IsUsingGamepad());
        scoreScreen.SetActive(false);
    }

    public void StartSong(SongSO song)
    {
        StopAllCoroutines();
        ResumeGame();
        scoreScreen.SetActive(false);
        timer = 0;
        score = 0;
        scoreTMP.text = score.ToString();
        currentSong = song;
        noteIndexToPlay = startFromNote < currentSong.notes.Length ? startFromNote : 0;
        noteIndexToSpawn = startFromNote < currentSong.notes.Length ? startFromNote : 0;
        currentNote = currentSong.notes[noteIndexToPlay];
        EmptyAllStringsNotes();
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
        songSource.clip = song.songFile;
        float startTime = currentSong.notes[startFromNote].secondToPlay - currentSong.speed;
        StartCoroutine(TimerCountV2());

        if (startTime <= 0)
        {
            songSource.time = 0;
            // Math.Abs negative to positive number
            yield return new WaitForSeconds(Math.Abs(startTime));
        }
        else
        {
            songSource.time = startTime;
        }

        songSource.Play();

        yield return null;
    }

    IEnumerator TimerCount()
    {
        while (songSource.clip.length > songSource.time)
        {
            // gameTime = (float)(AudioSettings.dspTime - songStartDSPTime) - currentSong.speed;
            timer = songSource.time;
            // timer = gameTime;
            timerTMP.text = TimerToMinutes();
            NextNote(currentSong.notes[noteIndexToSpawn]);
            yield return null;
        }
        FinishSong();
    }

    IEnumerator TimerCountV2()
    {
        Song currentSong = Song2();
        // bool wholeNum = currentBeat % 1 == 0;
        // if (wholeNum) currentBeat++;

        while (songSource.clip.length > songSource.time)
        {
            int currentBeat = GetCurrentBeat(currentSong);
            Debug.Log("currentBeat = " + currentBeat);
            // gameTime = (float)(AudioSettings.dspTime - songStartDSPTime) - currentSong.speed;
            timer = songSource.time;
            // timer = gameTime;
            timerTMP.text = TimerToMinutes();
            // NextNote(currentSong.notes[noteIndexToSpawn]);
            NextNotev2(currentSong, currentBeat);
            yield return null;

        }
        FinishSong();
    }

    int GetCurrentBeat(Song song)
    {
        float fpb = song.GetFpb();

        int currentBeat = Mathf.FloorToInt(songSource.timeSamples / fpb);
        return currentBeat;
    }

    void NextNote(NoteData currentNoteToSpawn)
    {
        if (noteIndexToSpawn < currentSong.notes.Length)
        {
            float secondToSpawn = currentNoteToSpawn.secondToPlay - currentSong.speed;

            if (secondToSpawn <= timer)
            {
                SpawnNote(currentNoteToSpawn.stringNum);
                noteIndexToSpawn++;
            }
        }

    }

    void NextNotev2(Song song, int currentBeat)
    {
        // check more notes to play
        if (noteIndexToSpawn < song.notes.Count)
        {
            Note2 noteToSpawn = song.notes[noteIndexToSpawn];
            int beatToSpawn = noteToSpawn.beat - song.speed;
            // float secondToSpawn = currentNoteToSpawn.second - currentSong.speed;

            if (beatToSpawn <= currentBeat)
            {
                SpawnNote(noteToSpawn.bassString);
                noteIndexToSpawn++;
            }
        }
    }

    void SpawnNote(int noteString)
    {
        Transform noteSpawnTransform = noteSpawns[noteString - 1].transform;
        GameObject noteGO = Instantiate(notePrefab, noteSpawnTransform);
        Note note = noteGO.GetComponent<Note>();
        note.triggerStringPosition = triggerString.transform.position;
        note.SetSecondsToReachTarget(currentSong.speed);
        note.SetLimit(limit);
        notesGOList.Add(noteGO);
        note.OnNoteMissed += NextCurrentNote;
    }

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
                ScoreSystem(currentNote);
            }
        }


    }

    void ScoreSystem(NoteData currentNote)
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

    public void ToggleMenuInput(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed) return;
        ToggleMenu();
    }

    public void ToggleMenu()
    {
        scoreScreen.SetActive(!scoreScreen.activeInHierarchy);
        if (scoreScreen.activeInHierarchy)
        {
            Time.timeScale = 0;
            scoreScreen.GetComponent<ScoreScreen>().SetScore(score + "");
            songSource.Pause();
        }
        else
        {
            Time.timeScale = 1;
            songSource.Play();
        }
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        songSource.Play();
    }

}