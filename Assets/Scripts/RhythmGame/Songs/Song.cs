using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SongSO", menuName = "Scriptable Objects/Song")]
public class Song : ScriptableObject
{
    // [NonSerialized] public List<Note> notes = new();
    // public Note2[] notes;
    public int beatsDelay = 4;
    public int bpm = 120;
    public int frequency = 44100;
    public int scoreToPass;
    public string jsonFilename;
    public AudioClip songFile;

    [NonSerialized] public float spb;

    public List<NoteData> notes = new();

    public int GetFpb()
    {
        // second per beat
        spb = 60f / bpm;
        // frequency per beat
        int fpb = Mathf.FloorToInt(spb * frequency);
        return fpb;
    }

}

// public class Note2
// {
//     public int bassString;
//     public int beat;
//     public int beatToPlay;

//     public Note2(int bassString, int beat)
//     {
//         this.bassString = bassString;
//         this.beat = beat;
//     }
// }

[Serializable]
public class NoteData
{
    public int bassString;
    public int beat;
    public NoteData(int bassString, int beat)
    {
        this.bassString = bassString;
        this.beat = beat;
    }
}

[Serializable]
public class SongData
{
    public List<NoteData> notes;
}