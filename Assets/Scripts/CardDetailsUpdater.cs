using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class CardDetailsUpdater : MonoBehaviour
{
	[Header("Card")]
	[SerializeField] TextMeshProUGUI cardNumberText;
	[SerializeField] TMP_InputField cardInput;

	[SerializeField] TMP_Text cardNumberCorrect;
	[SerializeField] TMP_Text cardNumberIncorrect;

	[Header("2FA")]
	[SerializeField] TextMeshProUGUI code2FAText;
	[SerializeField] GameObject go2FA;
	[SerializeField] TMP_InputField _2FAInput;
	[SerializeField] GameObject codeCorrect;
	[SerializeField] GameObject codeIncorrect;
	[SerializeField] float codesChangeTime;

	[Header("Rotation Settings")]
	[SerializeField] bool rotateCardNumber = false;
	[SerializeField] bool rotate2FA = true;
	[HideInInspector] public bool rotated2FA = false;

	[Header("Debug")]
	[SerializeField] string numbers = "";
	[SerializeField] string code2FA = "";

	bool numbersOk;
	bool codeOk;

    public static string RemoveAllWhitespace(string input)
    {
        return Regex.Replace(input, "\\s+", "");
    }

    public bool CanBuy
	{
		get 
		{
			//print($"numberOk : {numbersOk}, use2FA : {GameManager.instance.use2FA}, codeOk : {codeOk}");
			return numbersOk && (!GameManager.instance.use2FA || (GameManager.instance.use2FA && codeOk)); 
		}
	}

    private void Start()
    {
        UpdateNumbers();
    }

    public void Start2FA()
	{
		Invoke("StartUsing2FA", codesChangeTime);
		InvokeRepeating("UpdateAll", 0f, codesChangeTime);
	}

	void StartUsing2FA()
	{
		GameManager.instance.use2FA = true;
	}

	private void UpdateAll()
	{
		if (rotateCardNumber) UpdateNumbers();

		if (GameManager.instance.use2FA)
		{
			if (!rotated2FA)
			{
                Update2FA();
				rotated2FA = true;
            }
			else if (rotated2FA && rotate2FA) Update2FA();
        }
	}

	public void UpdateNumbers()
	{
		numbersOk = false;
		string newNumbers = "";
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				newNumbers += Random.Range(0, 9);
			}
			newNumbers += " ";
		}

		newNumbers = newNumbers.Substring(0, newNumbers.Length - 1);
		cardNumberText.text = newNumbers;

		numbers = newNumbers;

		cardNumberCorrect.gameObject.SetActive(false);
		cardNumberIncorrect.gameObject.SetActive(false);
	}

	public void Update2FA()
	{
		codeOk = false;
		code2FA = "";
		for (int i = 0; i < 4; i++)
		{
			code2FA += Random.Range(0, 9);
		}

		code2FAText.text = $"Your 2FA code is {code2FA}";
		go2FA.SetActive(true);
		GameManager.instance.Show2FAInput();

		codeCorrect.SetActive(false);
		codeIncorrect.SetActive(false);
	}

	public void OnCardNumberInputChanged()
	{
		string current = cardInput.text;
		string strippedCurrent = RemoveAllWhitespace(current);
		string strippedNumbers = RemoveAllWhitespace(numbers);

		if (strippedCurrent.Length >= 16)
		{
			if (strippedCurrent == strippedNumbers)
			{
				numbersOk = true;
				cardNumberCorrect.gameObject.SetActive(true);
				cardNumberIncorrect.gameObject.SetActive(false);
			}
			else
			{
				numbersOk = false;
				cardNumberCorrect.gameObject.SetActive(false);
				cardNumberIncorrect.gameObject.SetActive(true);
			}
		}
	}

	public void On2FAInputChanged()
	{
		string current = _2FAInput.text;
		string strippedCurrent = RemoveAllWhitespace(current);

		if (strippedCurrent.Length >= 4)
		{
			if (strippedCurrent == code2FA)
			{
				codeOk = true;
				codeCorrect.SetActive(true);
				codeIncorrect.SetActive(false);
			}
			else
			{
				codeOk = false;
				codeCorrect.SetActive(false);
				codeIncorrect.SetActive(true);
			}
		}
	}
}
