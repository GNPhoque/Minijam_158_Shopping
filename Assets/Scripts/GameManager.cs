using System.Collections;
using TMPro;
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
    // Singleton
    public static GameManager instance;

    // Constants
    public const string NONE_INTERACTABLE = "[none]";

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

	[Header("UI")]
    public TMP_Text actionText;
    public TMP_Text timerText;
    
    [Header("Timer Settings")]
	[SerializeField] float timeLeft;
	[SerializeField] float alertStartTime;
	[SerializeField] bool alertStarted;
	[SerializeField] AudioClip timeAlert;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        player.state = PlayerState.Couch;
        Debug.Log("Start Game!");

        StartCoroutine(MomTick());
    }

    private void Update()
    {
		if (timeLeft > 0)
		{
			timeLeft -= Time.deltaTime;
			DisplayTime(timeLeft);
			if (timeLeft < alertStartTime && !alertStarted)
			{
				alertStarted = true;
                //TODO : Find AudioSource to play timeAlert sound;
			}
		}
		else
		{
			//TODO : GameOver();
		}
	}

    public void SetAction(string actionName)
    {
        actionText.text = actionName;
    }

    int GetRandomMomDelay()
    {
        return UnityEngine.Random.Range(startMomDelay, endMomDelay);
    }

    IEnumerator MomTick()
    {
        int spawnIn = GetRandomMomDelay();
        yield return new WaitForSeconds(spawnIn);

        // If player is at couch, then spawn mom at couch
        if (player.IsSittingOnCouch()) StartCoroutine(MomAtCouch());

        // ...
    }

    IEnumerator MomAtCouch()
    {
        momObject.EnableMomAtCouch();

        string momAction = "NONE";

        // Give Player time to react
        yield return new WaitForSeconds(momAggresion);

        // Check if player is at shopping window
        if (player.IsOnComputer())
        {
            if (player.IsShopping()) // If the player is on the shopping tab
            {
                playerChances -= 1;

                // .. mom jumpscare, or reaction time
            }
            else // Player is on the gaming tab
            {
                // .. mom might look at what you're doing, then go away
                momAction = "LEAVE";
            }
        } else
        {
            // You're just sitting on the couch doing nothing
            momAction = "LEAVE";
        }

        yield return new WaitForSeconds(GetRandomMomDelay());

        if (momAction == "LEAVE") momObject.DisableMomAtCouch();
        else if (momAction == "NONE") Debug.LogWarning("Mom didn't get an action?");
    }

    public void CheckIfGameOver()
    {
        if (playerChances > 0) return;

        Debug.Log("you lost");
        SceneManager.LoadScene(0);
	}

	void DisplayTime(float time)
	{
		time++;
		float minutes = Mathf.FloorToInt(time / 60f);
		float seconds = Mathf.FloorToInt(time % 60f);
		timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
	}
}
