using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SongLoader : MonoBehaviour
{

    public Song LoadSongFromJson(Song song, string fileName)
    {
        // Run GeneratePlaceholderSong first time to genereate beats in the json file
        GeneratePlaceholderSong(fileName);

        Debug.Log(Application.streamingAssetsPath);
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        string json = File.ReadAllText(path);

        SongData songData = JsonUtility.FromJson<SongData>(json);

        song.notes = new List<NoteData>();

        foreach (NoteData note in songData.notes)
        {
            song.notes.Add(new NoteData(note.bassString, note.beat));
        }

        return song;
    }

    void GeneratePlaceholderSong(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(path))
        {
            Debug.Log("JSON already exists " + path);
            return;
        }

        SongData data = new SongData
        {
            notes = new()
        };

        // song.notes = new List<NoteData>();
        // List<NoteData> notes = new();

        int[] pattern = { 1, 2, 3, 2 };

        for (int i = 0; i <= 200; i++)
        {
            int bassString = pattern[i % pattern.Length];
            int beat = i;
            data.notes.Add(new NoteData(bassString, beat));
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(
            Path.Combine(Application.streamingAssetsPath, fileName),
            json
        );
    }

}
