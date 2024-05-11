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
    public float moneySpent = 0f;
    public BuyableItemsHolder buyableItems;

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
        if (buyableItems == null) GameObject.Find("BuyableItemsHolder").GetComponent<BuyableItemsHolder>();

        moneySpent = 0;
        UpdateScore();

        player.state = PlayerState.Couch;
        player.activity = PlayerActivity.SittingOnCouch;
        Debug.Log("Start Game!");

        StartCoroutine(MomTick(bypassWaiting: true));
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

        if (!momAttacking) MomAI(false);
    }

    public void SetActionText(string actionName)
    {
        actionText.text = actionName;
    }

    #region Mom Functions

    public void MomAI(bool bypassWaiting = false)
    {
        print("mom ai");
        momAttacking = true;
        StartCoroutine(MomTick(bypassWaiting: bypassWaiting));
    }

    int GetRandomMomDelay()
    {
        return UnityEngine.Random.Range(startMomDelay, endMomDelay);
    }

    // Mom Functions
    IEnumerator MomTick(bool bypassWaiting)
    {
        if (!bypassWaiting)
        {
            int spawnIn = GetRandomMomDelay();
            yield return new WaitForSeconds(spawnIn);
        }

        print("where to spawn");

        // If player is at couch, then spawn mom at couch
        if (player.IsSittingOnCouch()) StartCoroutine(MomAtCouch());

        // ...
    }

    IEnumerator MomAtCouch()
    {
        momObject.EnableMomAtCouch();

        string momAction = "NONE";
        float delayBeforeLeave = 0.5f;

        // Give Player time to react
        yield return new WaitForSeconds(momAggresion);

        // Check if player is at shopping window
        if (player.IsOnComputer())
        {
            if (player.IsShopping()) // If the player is on the shopping tab
            {
                DecreaseChancesAndCheckIfGameOver();
                print("player is shopping");
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
        else
        {
            // You're just sitting on the couch doing nothing
            momAction = "LEAVE";
        }
        print("mom action: " + momAction);
        if (delayBeforeLeave == 0f) delayBeforeLeave = GetRandomMomDelay();
        yield return new WaitForSeconds(delayBeforeLeave);

        if (momAction == "LEAVE") momObject.DisableMomAtCouch();
        else if (momAction == "NONE") Debug.LogWarning("Mom didn't get an action?");

        yield return new WaitForSeconds(3f); // small delay to finish animations

        momAttacking = false;
    }

    #endregion

    public void DecreaseChancesAndCheckIfGameOver()
    {
        playerChances -= 1;
        if (playerChances > 0) return;

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
        if (spent == 0f) spent = moneySpent;
        float roundedSpent = Mathf.Round(spent);
        scoreText.text = $"Money spent:\n${roundedSpent}";
    }

    public string BuyItem()
    {
        Item itemToBuy = buyableItems.GetRandomItem();
        moneySpent += itemToBuy.cost;
        UpdateScore(moneySpent);

        string nameAndCost = itemToBuy.name + " $" + itemToBuy.cost;

        return nameAndCost;
    }

    public void GameOver(bool caught = false)
    {
        Time.timeScale = 0f;
        gameoverPanel.SetActive(true);
        if (caught) caughtText.SetActive(true);
        else dinerText.SetActive(true);
        gameOverScoreText.text = gameOverScoreText.text.Replace("*score*", moneySpent.ToString());
    }

    public void FlickBack()
    {
        print("Flicked Back");

        if (player.state != PlayerState.Couch) InitializeArea(Areas.Couch);

        if (player.IsOnComputer())
        {
            computerUI.SetActive(false);
            couchClickables.SetActive(true);

            player.activity = PlayerActivity.SittingOnCouch;
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
