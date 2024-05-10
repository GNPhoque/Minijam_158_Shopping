using System.Collections;
using System.ComponentModel.Design;
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

public enum Areas
{
    Couch,
    Table, 
    Kitchen
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
    [SerializeField] TextMeshProUGUI timerText;
	[SerializeField] TextMeshProUGUI scoreText;
	[SerializeField] TextMeshProUGUI gameOverScoreText;

    [Header("Areas")]
    [SerializeField] GameObject couchUI;
    [SerializeField] GameObject computerUI;
    [SerializeField] GameObject tableUI;
    [SerializeField] GameObject tableCardUI;
    [SerializeField] GameObject kitchenUI;

    [Header("Clickables")]
    [SerializeField] GameObject couchClickables;
    [SerializeField] GameObject tableClickables;
    [SerializeField] GameObject kitchenClickables;

    [Header("Scene References")]
    [SerializeField] GameObject gameoverPanel;
    [SerializeField] GameObject caughtText;
    [SerializeField] GameObject dinerText;
    
    [Header("Timer Settings")]
	[SerializeField] float timeLeft;
	[SerializeField] float alertStartTime;
	[SerializeField] bool alertStarted;
	[SerializeField] AudioClip timeAlert;

    private void Awake()
    {
        Time.timeScale = 1f;
		if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        player.state = PlayerState.Couch;
        player.activity = PlayerActivity.SittingOnCouch;
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
			GameOver();
		}
	}

    public void SetActionText(string actionName)
    {
        actionText.text = actionName;
    }

    #region Mom Functions

    int GetRandomMomDelay()
    {
        return UnityEngine.Random.Range(startMomDelay, endMomDelay);
    }

    // Mom Functions
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

    #endregion

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

    public void UpdateScore(float spent)
    {
        scoreText.text = $"Money spent:\n${spent}";
    }

    public void GameOver(bool caught = false)
    {
        Time.timeScale = 0f;
        gameoverPanel.SetActive(true);
		if (caught) caughtText.SetActive(true);
        else dinerText.SetActive(true);
        string spentMoney = HoldButton.spentMoney.ToString();
        gameOverScoreText.text = gameOverScoreText.text.Replace("*score*", spentMoney);
	}

    public void FlickBack()
    {
        print("Flicked Back");

        if (player.state != PlayerState.Couch) InitializeArea(Areas.Couch);
        
        if (player.IsOnComputer())
        {
            computerUI.SetActive(false);
            couchClickables.SetActive(true);
        }
    }

    #region Area Functions
    public void DisableAllAreas()
    {
        couchClickables.SetActive(false);
        couchUI.SetActive(false);

        computerUI.SetActive(false);

        tableClickables.SetActive(false);
        tableUI.SetActive(false);

        kitchenClickables.SetActive(false);
        kitchenUI.SetActive(false);
    }

    public void InitializeArea(Areas area)
    {
        DisableAllAreas();

        switch (area)
        {
            case Areas.Couch:
                couchUI.SetActive(true);
                couchClickables.SetActive(true);
                SetActivity(2);
                break;
            case Areas.Table:
                tableUI.SetActive(true);
                tableCardUI.SetActive(false);
                tableClickables.SetActive(true);
                InitializeTable();
                break;
            case Areas.Kitchen:
                kitchenUI.SetActive(true);
                kitchenClickables.SetActive(true);
                InitializeKitchen();
                break;
        }
    }

    public void InitializeArea(int area)
    {
        Areas selectedArea = (Areas) area;
        InitializeArea(selectedArea);
    }

    #endregion

    #region PlayerActions
    // Player Actions
    public void InitializeComputer(int compType)
    {
        couchClickables.SetActive(false);
        computerUI.SetActive(true);

        // CompType 0 is Shopping, while 1 is Gaming
        player.activity = (PlayerActivity)compType;
    }

    public void InitializeTable()
    {
        player.state = PlayerState.Table;
        player.activity = PlayerActivity.AtTable;
    }
    
    public void InitializeKitchen()
    {
        player.state = PlayerState.Kitchen;
        player.activity = PlayerActivity.StealingPhone;
    }

    public void SetActivity(int activityInt)
    {
        player.activity = (PlayerActivity)activityInt;
    }

    #endregion

    public static void ReloadScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
	}
}
