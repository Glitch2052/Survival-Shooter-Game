using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;
    public RectTransform Banner;
    public Text waveText;
    public Text enemyCount;
    Spawner spawner;
    void Awake()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
        spawner=FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    void OnNewWave(int WaveNumber)
    {
        string[] waveCount = { "One", "Two", "Three", "Four", "Five" };
        waveText.text = "- Wave " + waveCount[WaveNumber - 1] + " -";
        string enemyCountString = spawner.wave[WaveNumber - 1].isInfinite ? "Infinite" : spawner.wave[WaveNumber - 1].enemyCount+"";
        enemyCount.text = "Enemies: " + enemyCountString;
        StopCoroutine(AnimateBanner());
        StartCoroutine(AnimateBanner());
    }

    IEnumerator AnimateBanner()
    {
        float delayTime = 1.5f;
        float speed = 3f;
        float animatePercent = 0;
        float endDelay = Time.time + delayTime + 1 / speed;
        
        int dir=1;
        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * dir;
            if (animatePercent >= 1)
            {
                animatePercent = 1;
                if (Time.time > endDelay)
                {
                dir = -1;
                }
            }
            yield return null;
            Banner.anchoredPosition = Vector3.up * Mathf.Lerp(-525f, -325f, animatePercent);
        }
    }
    void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear,Color.black,1));
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from,Color to,float time)
    {
        float speed = 1 / time;
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }
   
    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }
}
