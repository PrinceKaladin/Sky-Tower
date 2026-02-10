using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttons : MonoBehaviour
{
   public Sprite check;
   public Sprite notcheck;
   public GameObject music;
   public GameObject sound;
   
    void Start()
    {
        if (!PlayerPrefs.HasKey("sound")) {
            PlayerPrefs.SetInt("sound", 1);
            PlayerPrefs.SetInt("music", 1);

        }
        if (PlayerPrefs.GetInt("sound") == 0) {
            sound.GetComponent<Image>().sprite = notcheck;

        }
        if (PlayerPrefs.GetInt("music") == 0)
        {
            music.GetComponent<Image>().sprite = notcheck;

        }

    }
    public void musicoffon() {
        if (PlayerPrefs.GetInt("music") == 0)
        {
            music.GetComponent<Image>().sprite = check;
            PlayerPrefs.SetInt("music", 1);
            GameObject.Find("audioline").GetComponent<AudioSource>().volume = 1;

        }
        else if (PlayerPrefs.GetInt("music") == 1) {
            music.GetComponent<Image>().sprite = notcheck;
            PlayerPrefs.SetInt("music", 0);
            GameObject.Find("audioline").GetComponent<AudioSource>().volume = 0;

        }

    }
    public void soundonoff() {
        if (PlayerPrefs.GetInt("sound") == 0)
        {
            sound.GetComponent<Image>().sprite = check;
            PlayerPrefs.SetInt("sound", 1);

        }
        else if (PlayerPrefs.GetInt("sound") == 1)
        {
            sound.GetComponent<Image>().sprite = notcheck;
            PlayerPrefs.SetInt("sound", 0);

        }
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
