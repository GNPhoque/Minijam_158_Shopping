using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerState state;
    public PlayerActivity activity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsOnComputer()
    {
        // If player is in shopping mode or gaming mode at the computer
        if (activity == PlayerActivity.ShoppingComputer || activity == PlayerActivity.GamingComputer) return true;
        return false;
    }

    public bool IsShopping()
    {
        return activity == PlayerActivity.ShoppingComputer;
    }

    public bool IsSittingOnCouch()
    {
        // If the player is sitting on the couch
        return activity == PlayerActivity.SittingOnCouch;
    }

    public bool IsNotAtCouch()
    {
        // If the player isn't sitting on couch, or on computer
        bool sittingOnCouch = IsSittingOnCouch();
        bool isOnComputer = IsOnComputer();

        if (sittingOnCouch || isOnComputer) return false;
        return true;
    }

    public bool IsAtTable()
    {
        return activity == PlayerActivity.AtTable;
    }
}
