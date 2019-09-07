using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public GameObject flashHolder;
    public Sprite[] flashSprite;
    public SpriteRenderer[] spriteRenderers;
    public float flashTime;

    public void Start()
    {
        DeActivate();
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            int spriteIndex = Random.Range(0, flashSprite.Length);
            spriteRenderers[i].sprite = flashSprite[spriteIndex];
        }
    }
    public void Activate()
    {
        flashHolder.SetActive(true);
        Invoke("DeActivate", flashTime);
    }

    public void DeActivate()
    {
        flashHolder.SetActive(false);
    }
}
