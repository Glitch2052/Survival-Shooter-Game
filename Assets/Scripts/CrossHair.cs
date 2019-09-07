using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    float rotateSpeed=40f;
    Transform player;
    public LayerMask targestMask;
    public SpriteRenderer dot;
    public Color dotHighLightColor;
    Color originalColor;

    private void Start()
    {
        Cursor.visible = false;
        originalColor = dot.color;
        player = FindObjectOfType<Player>().transform;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * rotateSpeed);
        if (player == null)
        {
            Cursor.visible = true;
        }
    }

    public void DetectTarget(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targestMask))
        {
            dot.color = dotHighLightColor;
        }
        else
        {
            dot.color = originalColor;
        }
    }
}
