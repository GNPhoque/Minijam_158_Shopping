using TMPro;
using UnityEngine;

public class PhoneUI : MonoBehaviour
{
    [System.Serializable]
    class TextMessage
    {
        public GameObject parentObj;
        public TMP_Text textObj;
        [TextArea(2, 3)] public string message;

        public void SetTextObject()
        { textObj = parentObj.GetComponentInChildren<TMP_Text>(); }
        public void SetMessageText()
        { textObj.text = message; }
        public void SetTextObjectAndText()
        { SetTextObject(); SetMessageText(); }
    }

    [SerializeField] TextMessage message1;
    [SerializeField] TextMessage message3;
    [SerializeField] GameObject authMessage;

    bool reachedFirstCriteriaSpent = false;

    [SerializeField] TMP_Text timeText;
    [SerializeField] [TextArea(2, 5)] string reachedFirstCriteriaMessage =
        "Credit Card Company:\nThere has been some suspicios activity on your account";

    // Start is called before the first frame update
    void Start()
    {
        message1.SetTextObjectAndText();
        message3.SetTextObjectAndText();
    }

    // Update is called once per frame
    void Update()
    {
        timeText.text = System.DateTime.Now.ToString("HH:mm");

        if (AudioManager.instance.firstCriteriaReached && !reachedFirstCriteriaSpent)
        {
            reachedFirstCriteriaSpent = true;
            message3.message = reachedFirstCriteriaMessage;
            message3.SetMessageText();
        }

        if (GameManager.instance.GetComponent<CardDetailsUpdater>().rotated2FA) authMessage.SetActive(true);
    }
}
