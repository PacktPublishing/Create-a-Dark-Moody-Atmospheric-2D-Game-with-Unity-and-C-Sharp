using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectables : MonoBehaviour
{
    int Scroll = 0;
    public Text ScrollCounter;

    AudioSource src;
    public AudioClip CollectSfx;


    public GameObject PausePanel;
    public bool paused;
    private void Start()
    {
        src = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            PausePanel.SetActive(paused);
        }

        if(paused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Scroll")
        {
            Scroll++;
            ScrollCounter.text = " x" + Scroll.ToString();
            src.PlayOneShot(CollectSfx);
            Destroy(collision.gameObject);
        }
    }
}
