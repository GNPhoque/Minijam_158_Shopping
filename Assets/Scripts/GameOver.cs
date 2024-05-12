using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class GameOver : MonoBehaviour
{
    [SerializeField] TimelineAsset caught;
    [SerializeField] TimelineAsset notCaught;

    PlayableDirector director;

    // Start is called before the first frame update
    void Start()
    {
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
