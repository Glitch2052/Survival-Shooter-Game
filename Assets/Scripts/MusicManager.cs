using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme;
    public AudioClip musicTheme;
    // Update is called once per frame

    private void Start()
    {
        AudioManager.instance.PlayMusic(musicTheme, 2);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.instance.PlayMusic(mainTheme, 2);

        }
    }
}
