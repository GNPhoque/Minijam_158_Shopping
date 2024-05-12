using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable CS0162

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
    public BuyableItemsHolder buyableItems;
	public float spentMoney = 0f;
    public bool use2FA = false;
    bool firstItemBought = false;

    [Header("Criterias")]
    public float FIRST_SPENT_CRITERIA = 15_000;
    public float SECOND_SPENT_CRITERIA = 50_000;
    public float THIRD_SPENT_CRITERIA = 75_000;
    public float FOURTH_SPENT_CRITERIA = 100_000;
    public bool SCALE_CRITERIAS = true;
    public int SCALE_FROM_TIME = 150;

	// Mom Settings
	[Header("Mom Settings")]
    public Mom momObject;
    public int startMomDelay;
    public int endMomDelay;
    // How much time the player has to react to mom
    public float momAggresion = 2f;

    bool momAttacking = false;

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
    [SerializeField] GameObject kitchenPhoneUI;

    [Header("Clickables")]
    [SerializeField] GameObject couchClickables;
    [SerializeField] GameObject tableClickables;
    [SerializeField] GameObject kitchenClickables;

    [Header("Scene References")]
    [SerializeField] GameObject gameoverPanel;
    [SerializeField] GameObject caughtText;
    [SerializeField] GameObject dinerText;
    [SerializeField] GameObject input2FA;

	[Header("Timer Settings")]
	public float timeLeft;
    [SerializeField] bool eligibleToStartTimer = false;

    // Debug Values
    const bool DEBUG_MOM_COUCH_THINKING = false;
    const bool DEBUG_MOM_THINKING = false;
    const bool DEBUG_FLICKING = false;

    private void Awake()
    {
        Time.timeScale = 1f;
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (buyableItems == null) GameObject.Find("BuyableItemsHolder").GetComponent<BuyableItemsHolder>();

        if (!eligibleToStartTimer) timerText.gameObject.SetActive(false);

		spentMoney = 0;
        UpdateScore();

        player.state = PlayerState.Couch;
        player.activity = PlayerActivity.SittingOnCouch;
        
        // Scale the criterias based on time
        if (SCALE_CRITERIAS)
        {
            // Criterias should be three times as hard as initial scaling time
            float scalingFactor = (timeLeft / SCALE_FROM_TIME);
            FIRST_SPENT_CRITERIA *= scalingFactor;
            SECOND_SPENT_CRITERIA *= scalingFactor;
            THIRD_SPENT_CRITERIA *= scalingFactor;
            FOURTH_SPENT_CRITERIA *= scalingFactor;
        }
        
        Debug.Log("Start Game!");

        StartCoroutine(MomTick(bypassWaiting: true));

        AudioManager.instance.PlayLoopPreIntro();
        AudioManager.instance.ResetLoop();
        AudioManager.instance.ForcePlayMusicLoop();

        ValueBank.totalTime = timeLeft;
    }

    private void Update()
    {
        if (eligibleToStartTimer)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                DisplayTime(timeLeft);

                if (timeLeft <= 20 && !AudioManager.instance.endingPlayed) AudioManager.instance.PlayLoopEnding();
            }
            else
            {
                GameOver();
            }
        }

        if (!momAttacking) MomAI(false);
        momObject.momCouchAnim.gameObject.GetComponent<SpriteRenderer>().enabled = player.state == PlayerState.Couch;
    }

    public void SetActionText(string actionName)
    {
        actionText.text = actionName;
    }

    #region Mom Functions

    public void MomAI(bool bypassWaiting = false)
    {
        if (DEBUG_MOM_THINKING) print("MOMAI: AI");

        if (DEBUG_MOM_THINKING) print("MOMAI: Attacking");
        if (!momAttacking)
        {
            momAttacking = true;
            StartCoroutine(MomTick(bypassWaiting: bypassWaiting));
        }
    }

    int GetRandomMomDelay()
    {
        return UnityEngine.Random.Range(startMomDelay, endMomDelay);
    }

    // Mom Functions
    IEnumerator MomTick(bool bypassWaiting)
    {
        /*if (momAttacking)
        {
            print("MOMAI: Tried to spawn another instance");
            yield break; // this, not yield return null;
        }*/

        if (!bypassWaiting)
        {
            if (DEBUG_MOM_THINKING) print("waiting for movement opportunity");
            int spawnIn = GetRandomMomDelay();
            yield return new WaitForSeconds(spawnIn);
        }
        else if (DEBUG_MOM_THINKING) print("bypassed waiting");

        if (DEBUG_MOM_THINKING) print("MOMAI: Movement");

        // If player is at couch, then spawn mom at couch
        if (player.IsSittingOnCouch()) StartCoroutine(MomAtCouch());
        else
        {
            while (!player.IsSittingOnCouch())
            {
                // Wait a few seconds for the player to get back to the couch area
                if (DEBUG_MOM_THINKING) print("MOMAI: Stalled");
                yield return new WaitForSeconds(1.5f);
            }
            StartCoroutine(MomAtCouch());
        }
    }

    IEnumerator MomAtCouch()
    {
        if (DEBUG_MOM_THINKING) print("MOMAI: Successful movement");

        momObject.EnableMomAtCouch();

        string momAction = "NONE";
        float delayBeforeLeave = 0.5f;

        // Give Player time to react
        yield return new WaitForSeconds(momAggresion);

        // Check if player is at shopping window
        if (player.IsOnComputer())
        {
            if (DEBUG_MOM_THINKING) print("MOMAI: Player is on computer");
            if (player.IsShopping()) // If the player is on the shopping tab
            {
                DecreaseChancesAndCheckIfGameOver();

                if (DEBUG_MOM_COUCH_THINKING) print("MOMAI: Caught player shopping");

                // .. mom jumpscare, or reaction time
                // for now she'll just leave
                momAction = "LEAVE";
                delayBeforeLeave = 0.5f;
            }
            else // Player is on the gaming tab
            {
                // .. mom might look at what you're doing, then go away
                momAction = "LEAVE";
            }
        }
        else if (player.activity == PlayerActivity.SittingOnCouch)
        {
            // You're just sitting on the couch doing nothing
            momAction = "LEAVE";
        }
        else if (player.state != PlayerState.Couch)
        {
            // the player moved to look at credit card, or steal phone WHILE mom was looking, it's game over
            if (DEBUG_MOM_COUCH_THINKING) print("MOMAI: You got caught in the act of naughty behaviour, game over");
            // ..mom jumpscare, and show game over screen.
            GameOver(caught: true);
            yield return null;
        }
        
        if (DEBUG_MOM_COUCH_THINKING) print("MOMAI: Action: " + momAction);
        
        if (delayBeforeLeave == 0f) delayBeforeLeave = GetRandomMomDelay();
        yield return new WaitForSeconds(delayBeforeLeave);

        if (momAction == "LEAVE") momObject?.DisableMomAtCouch();
        else if (momAction == "NONE") Debug.LogWarning("Mom didn't get an action?");

        yield return new WaitForSeconds(3f); // small delay to finish animations

        if (DEBUG_MOM_THINKING) print("MOMAI: Not attacking");
        momAttacking = false;
    }

    #endregion

    public void DecreaseChancesAndCheckIfGameOver()
    {
        playerChances -= 1;
        if (playerChances > 0)
        {
            AudioManager.instance.PlayCaughtShopping();
            return;
        }

        Debug.Log("you lost");
        GameOver(caught: true);
        //SceneManager.LoadScene(0);
    }

    void DisplayTime(float time)
    {
        time++;
        float minutes = Mathf.FloorToInt(time / 60f);
        float seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateScore(float spent = 0f)
    {
        if (spent == 0f) spent = spentMoney;
        float roundedSpent = Mathf.Round(spent);
        scoreText.text = $"Money spent:\n${roundedSpent}";
    }

    public string BuyItem()
    {
        if (!firstItemBought)
        {
            firstItemBought = true;
            eligibleToStartTimer = true;
            timerText.gameObject.SetActive(true);
            GetComponent<CardDetailsUpdater>().Start2FA();
        }

        Item itemToBuy = buyableItems.GetRandomItem();
		spentMoney += itemToBuy.cost;
        UpdateScore(spentMoney);
        TriggerCardFirstUsed();
        if (spentMoney >= FOURTH_SPENT_CRITERIA) TriggerFourthCriteriaReached();
        else if (spentMoney >= THIRD_SPENT_CRITERIA) TriggerThirdCriteriaReached();
        else if (spentMoney >= SECOND_SPENT_CRITERIA) TriggerSecondCriteriaReached();
        else if (spentMoney >= FIRST_SPENT_CRITERIA) TriggerFirstCriteriaReached();

        AudioManager.instance.PlayRandomBuy();

        string nameAndCost = itemToBuy.name + " $" + itemToBuy.cost;

        return nameAndCost;
    }

    public void Show2FAInput()
    {
        input2FA.SetActive(true);
    }

    public void GameOver(bool caught = false)
    {
        ValueBank.gameOverType = caught ? 1 : 0;
        ValueBank.moneySpent = spentMoney;

        if (caught)
        {
            if (player.state == PlayerState.Couch) ValueBank.reasonOfLoss = "You can't be on the shop tab while mom is looking! You've established theese rules already..";
            else if (player.state == PlayerState.Table) ValueBank.reasonOfLoss = "Mom can see that you take her card, she has eyes just like you do";
            else if (player.state == PlayerState.Kitchen) ValueBank.reasonOfLoss = "I don't think you would be happy if someone stole your phone either. Oh wait, you don't have one";
        }
        else
        {
            ValueBank.reasonOfLoss = "I mean, the timer ran out? Not much you can do about that..";
        }

        SceneManager.LoadScene("GameOver");

        /*Time.timeScale = 0f;
        gameoverPanel.SetActive(true);
        if (caught) caughtText.SetActive(true);
        else dinerText.SetActive(true);
        string spent = spentMoney.ToString();
        gameOverScoreText.text = gameOverScoreText.text.Replace("*score*", spent);*/
	}

    public void FlickBack()
    {
        if (DEBUG_FLICKING) print("Flicked Back");

        if (player.state != PlayerState.Couch) InitializeArea(Areas.Couch);

        if (player.IsOnComputer())
        {
            computerUI.SetActive(false);
            couchClickables.SetActive(true);

            player.activity = PlayerActivity.SittingOnCouch;
        }
    }

    public void TriggerCardFirstShown()
    {
        if (!AudioManager.instance.cardShown) AudioManager.instance.PlayLoopPreIntroWithBass();
			AudioManager.instance.cardShown = true;
    }

    public void TriggerCardFirstUsed()
    {
        if (!AudioManager.instance.firstBuy) AudioManager.instance.PlayLoopIntro();
			AudioManager.instance.firstBuy = true;
	}

	public void TriggerFirstCriteriaReached()
	{
		if (!AudioManager.instance.firstCriteriaReached) AudioManager.instance.PlayLoopPart1WOSnare();
		AudioManager.instance.firstCriteriaReached = true;
        ValueBank.criteria1Reached = true;
	}

	public void TriggerSecondCriteriaReached()
	{
		if (!AudioManager.instance.secondCriteriaReached) AudioManager.instance.PlayLoopPart1WSnare();
		AudioManager.instance.secondCriteriaReached = true;
        ValueBank.criteria2Reached = true;
	}

	public void TriggerThirdCriteriaReached()
	{
		if (!AudioManager.instance.thirdCriteriaReached) AudioManager.instance.PlayLoopPart2();
		AudioManager.instance.thirdCriteriaReached = true;
        ValueBank.criteria3Reached = true;
	}

	public void TriggerFourthCriteriaReached()
	{
        ValueBank.criteria4Reached = true;
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
                player.state = PlayerState.Couch;
                break;
            case Areas.Table:
                tableUI.SetActive(true);
                tableCardUI.SetActive(false);
                tableClickables.SetActive(true);
                InitializeTable();
                break;
            case Areas.Kitchen:
                kitchenUI.SetActive(true);
                kitchenPhoneUI.SetActive(false);
                kitchenClickables.SetActive(true);
                InitializeKitchen();
                break;
        }
    }

    public void InitializeArea(int area)
    {
        Areas selectedArea = (Areas)area;
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
