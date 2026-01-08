using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SongSO", menuName = "Scriptable Objects/SongSO")]
public class SongSO : ScriptableObject
{
    public string songName;
    // public int songDurationSeconds;
    public NoteData[] notes;
    public int scoreToPass;
}
