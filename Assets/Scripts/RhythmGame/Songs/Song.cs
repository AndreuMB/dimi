using System.Collections.Generic;
using UnityEngine;

public class Song
{
    public List<Note2> notes = new();
    // public Note2[] notes;
    public int speed = 4;
    public int bpm = 120;
    public int frequency = 44100;


    public Song()
    {
        // notes[0] = new(1, 1);
        // notes.Add(new Note2(1, 0.5f));
        // notes.Add(new Note2(1, 1.0f));
        // notes.Add(new Note2(2, 1.75f));
        // notes.Add(new Note2(4, 2.5f));
    }

    public float GetFpb()
    {
        // second per beat
        float spb = 60f / bpm;
        Debug.Log("spb = " + spb);
        // frequency per beat
        float fpb = spb * frequency;
        return fpb;
    }

}

public class Note2
{
    public int bassString;
    public int beat;

    public Note2(int bassString, int beat)
    {
        this.bassString = bassString;
        this.beat = beat;
    }
}
