using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class GameOver : MonoBehaviour
{
    [SerializeField] TimelineAsset caught;
    [SerializeField] TimelineAsset notCaught;

    [SerializeField] TMP_Text variableText;

    PlayableDirector director;

    // Start is called before the first frame update
    void Start()
    {
        string text = variableText.text;
        text = text.Replace("*money*", ValueBank.moneySpent.ToString());
        TimeSpan timeSpan = TimeSpan.FromSeconds(ValueBank.totalTime);
        string formattedTime = timeSpan.ToString(@"mm\:ss");
        text = text.Replace("*totalTime*", formattedTime);
        variableText.text = text;
        variableText.gameObject.SetActive(false);

        director = GetComponent<PlayableDirector>();

        if (ValueBank.gameOverType == 0) // not caught
            director.Play(notCaught);
        else // caught 
            director.Play(caught);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
