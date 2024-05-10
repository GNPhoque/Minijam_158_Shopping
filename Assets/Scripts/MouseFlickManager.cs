using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFlickManager : MonoBehaviour
{
    bool mouseOver = false;

    private void OnMouseEnter()
    {
        if (!mouseOver)
        {
            mouseOver = true;
            GameManager.instance.FlickBack();
        }
    }

    private void OnMouseExit()
    {
        mouseOver = false;
    }
}
