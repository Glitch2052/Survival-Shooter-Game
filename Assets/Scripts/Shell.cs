using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    Rigidbody myRigidbody;
    public float forceMin;
    public float forceMax;
    float lifeTime=4f;
    float fadeTime=2f;
    // Start is called before the first frame update
    void Start()
    {
        float force = Random.Range(forceMin, forceMax);
        myRigidbody = GetComponent<Rigidbody>();
        myRigidbody.AddForce(transform.right * force);
        myRigidbody.AddTorque(Random.insideUnitSphere * force);
        StartCoroutine(Fade());
    }
    
    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifeTime);
        float speed = 1 / fadeTime;
        float percent = 0;
        Material mat = GetComponent<Renderer>().material;
        Color initialColor = mat.color;
        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            mat.color = Color.Lerp(initialColor, Color.clear, percent);
        }
        Destroy(gameObject);
    }
}
