using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsManager : MonoBehaviour
{
    [SerializeField] GameObject tab1;
    [SerializeField] GameObject tab2;

    Color GRAY = new Color(0.9f, 0.9f, 0.9f);

    public void EnableTab1()
    {
        tab1.GetComponent<Image>().color = GRAY;
        tab2.GetComponent<Image>().color = Color.white;
    }

    public void EnableTab2()
    {
        tab2.GetComponent<Image>().color = GRAY;
        tab1.GetComponent<Image>().color = Color.white;
    }
}
