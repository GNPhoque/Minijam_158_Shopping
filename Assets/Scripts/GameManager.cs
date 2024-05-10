using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Enums
public enum PlayerState
{
    Couch,
    Table,
    Kitchen
}

public enum PlayerActivity
{
    ShoppingComputer,
    GamingComputer,
    SittingOnCouch,
    AtTable,
    StealingPhone
}

public class GameManager : MonoBehaviour
{
    [Header("Player Settings")]
    public Player player;
    public int playerChances = 3;

    // Mom Settings
    [Header("Mom Settings")]
    public Mom momObject;
    public int startMomDelay;
    public int endMomDelay;

    // How much time the player has to react to mom
    public float momAggresion = 2f;

    // Start is called before the first frame update
    void Start()
    {
        player.state = PlayerState.Couch;
        Debug.Log("Start Game!");

        StartCoroutine(MomTick());
    }

    IEnumerator MomTick()
    {
        int spawnIn = UnityEngine.Random.Range(startMomDelay, endMomDelay);
        yield return new WaitForSeconds(spawnIn);

        // If player is at couch, then spawn mom at couch
        if (player.IsSittingOnCouch()) StartCoroutine(MomAtCouch());

        // ...
    }

    IEnumerator MomAtCouch()
    {
        momObject.EnableMomAtCouch();

        // Give Player time to react
        yield return new WaitForSeconds(momAggresion);

        // Check if player is at shopping window
        if (player.IsShopping())
        {
            playerChances -= 1;

            // .. mom jumpscare, or reaction time
        }
    }

    public void CheckIfGameOver()
    {
        if (playerChances > 0) return;

        Debug.Log("you lost");
        SceneManager.LoadScene(0);
    }
}
