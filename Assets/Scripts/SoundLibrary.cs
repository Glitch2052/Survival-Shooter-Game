using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    public SoundGroup[] group;
    Dictionary<string, AudioClip[]> groupDictionary = new Dictionary<string, AudioClip[]>();

    private void Awake()
    {
        foreach(SoundGroup group in group)
        {
            groupDictionary.Add(group.groupID, group.audioGroup);
        }    
    }

    public AudioClip GetClipFromName(string clipName)
    {
        if (groupDictionary.ContainsKey(clipName))
        {
            AudioClip[] sounds = groupDictionary[clipName];
            return sounds[Random.Range(0, sounds.Length)];
        }
        return null;
    }

    [System.Serializable]
   public class SoundGroup
    {
        public string groupID;
        public AudioClip[] audioGroup;
    }
}
