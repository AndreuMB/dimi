using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    // [SerializeField] private List<SongSO> songs;
    // private float timer = 0;
    [SerializeField] private TMP_Text timerTMP;
    // private SongSO currentSong;
    private Song currentSong;
    private NoteData currentNote;
    // private NoteData currentNote;
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
    [SerializeField] SongLoader songLoader;


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
        UpdateStringsHint(player.GetComponent<Player>().IsUsingGamepad());
        scoreScreen.SetActive(false);
    }


    public void StartSong(Song song)
    {
        songLoader.LoadSongFromJson(song, song.jsonFilename + ".json");
        StopAllCoroutines();
        Time.timeScale = 1;
        scoreScreen.SetActive(false);
        score = 0;
        scoreTMP.text = score.ToString();
        currentSong = song;
        noteIndexToPlay = startFromNote < song.notes.Count ? startFromNote : 0;
        noteIndexToSpawn = startFromNote < song.notes.Count ? startFromNote : 0;
        currentNote = song.notes[noteIndexToPlay];
        EmptyAllStringsNotes();
        // StartCoroutine(StartSongAudio(song));
        StartSongAudio(song);
    }

    public void RestartSong()
    {
        songSource.Stop();
        StartSong(currentSong);
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

    async void StartSongAudio(Song song)
    {
        songSource.clip = song.songFile;
        int startTime = song.notes[startFromNote].beat * song.GetFpb() - currentSong.beatsDelay * song.GetFpb();



        songSource.timeSamples = startTime;
        StartCoroutine(TimerCount());
        await GenerateFalseBeats();

        // Debug.Log("startTime = " + startTime);


        songSource.Play();
        timerTMP.text = TimerToMinutes(songSource.time);




        // yield return null;
    }

    async Awaitable GenerateFalseBeats()
    {
        while (songSource.timeSamples < 0)
        {
            await Task.Delay(Mathf.FloorToInt(currentSong.spb * 1000));
            // WaitForSeconds(currentSong.spb);
            songSource.timeSamples += currentSong.GetFpb();
        }
    }

    IEnumerator TimerCount()
    {

        while (songSource.clip.length > songSource.time)
        {
            int currentBeat = GetCurrentBeat(currentSong);
            float timer = songSource.time;
            // timer = gameTime;
            timerTMP.text = TimerToMinutes(timer);
            // NextNote(currentSong.notes[noteIndexToSpawn]);
            NextNote(currentSong, currentBeat);
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


    void NextNote(Song song, int currentBeat)
    {
        // check more notes to play
        if (noteIndexToSpawn < song.notes.Count)
        {
            NoteData noteToSpawn = song.notes[noteIndexToSpawn];
            int beatToSpawn = noteToSpawn.beat - song.beatsDelay;
            // Debug.Log("beatToSpawn = " + beatToSpawn);
            // Debug.Log("currentBeat = " + currentBeat);
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
        float secondsDelay = currentSong.beatsDelay * currentSong.spb;
        note.SetSecondsToReachTarget(secondsDelay);
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


            if (keyNum == currentNote.bassString)
            {
                ScoreSystem(currentNote, GetCurrentBeat(currentSong));
            }
        }


    }

    void ScoreSystem(NoteData currentNote, int currentBeat)
    {
        float currentTime = songSource.time;
        float noteTime = currentNote.beat * currentSong.spb;
        float accuracy = Mathf.Abs(noteTime - currentTime);

        // Debug.Log("currentTime = " + currentTime);
        // Debug.Log("noteTime = " + noteTime);
        // Debug.Log("accuracy = " + accuracy);
        if (accuracy > 0.6)
        {
            // above 0.6 doesnt destroy note
            Debug.Log("No valid");
            return;
        }
        else if (accuracy > 0.3)
        {
            Debug.Log("Meh");
            score += 25;
        }
        else if (accuracy > 0.1)
        {
            score += 50;
            Debug.Log("Good");
        }
        else if (accuracy > 0.5)
        {
            score += 75;
            Debug.Log("Nice!");
        }
        else
        {
            score += 100;
            Debug.Log("Perfect!");
        }

        scoreTMP.text = score.ToString();

        DestroyImmediate(notesGOList[noteIndexToPlay]);
        NextCurrentNote();
    }

    public void NextCurrentNote()
    {
        noteIndexToPlay++;
        if (noteIndexToPlay >= currentSong.notes.Count) return;
        currentNote = currentSong.notes[noteIndexToPlay];
    }

    string TimerToMinutes(float timer)
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
            ResumeGame();
        }
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        songSource.Play();
    }

}