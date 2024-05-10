using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mom : MonoBehaviour
{
    [SerializeField] private GameObject momCouchBG;
    public bool momActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableMomAtCouch()
    {
        momActive = true;
        momCouchBG.SetActive(true);
    }

    public void DisableMomAtCouch()
    {
        momActive = false;
        momCouchBG.SetActive(false);
    }

    public void ToggleMomAtCouch()
    {
        momActive = !momActive;
        momCouchBG.SetActive(momActive);
    }
}
