using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SongLoader : MonoBehaviour
{

    public Song LoadSongFromJson(Song song, string fileName)
    {
        // Run GeneratePlaceholderSong first time to genereate beats in the json file
        // GeneratePlaceholderSong(fileName);

        Debug.Log(Application.streamingAssetsPath);
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        string json = File.ReadAllText(path);

        SongData songData = JsonUtility.FromJson<SongData>(json);

        song.notes = new List<Note2>();

        foreach (var note in songData.notes)
        {
            song.notes.Add(new Note2(note.bassString, note.beat));
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
            notes = new List<NoteData>()
        };

        int[] pattern = { 1, 2, 3, 2 };

        for (int i = 0; i <= 200; i++)
        {
            data.notes.Add(new NoteData
            {
                bassString = pattern[i % pattern.Length],
                beat = i
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(
            Path.Combine(Application.streamingAssetsPath, fileName),
            json
        );
    }

}
