using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDetailsUpdater : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI numbersText;
	[SerializeField] TextMeshProUGUI code2FAText;
	[SerializeField] GameObject go2FA;
	[SerializeField] GameObject numbersCorrect;
	[SerializeField] GameObject numbersIncorrect;
	[SerializeField] GameObject codeCorrect;
	[SerializeField] GameObject codeIncorrect;
	[SerializeField] float codesChangeTime;

	string numbers = "";
	string code2FA = "";

	bool numbersOk;
	bool codeOk;

	public bool CanBuy
	{
		get 
		{
			print($"numberOk : {numbersOk}, use2FA : {GameManager.instance.use2FA}, codeOk : {codeOk}");
			return numbersOk && (!GameManager.instance.use2FA || (GameManager.instance.use2FA && codeOk)); 
		}
	}

	private void Start()
	{
		InvokeRepeating("UpdateAll", 0f, codesChangeTime);
	}

	private void UpdateAll()
	{
		UpdateNumbers();
		if (GameManager.instance.use2FA) Update2FA();
		numbersCorrect.SetActive(false);
		numbersIncorrect.SetActive(false);
		codeCorrect.SetActive(false);
		codeIncorrect.SetActive(false);
	}

	public void UpdateNumbers()
	{
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
		numbersText.text = newNumbers;

		numbers = newNumbers;
	}

	public void Update2FA()
	{
		code2FA = "";
		for (int i = 0; i < 4; i++)
		{
			code2FA += Random.Range(0, 9);
		}

		code2FAText.text = $"Your 2FA code is {code2FA}";
		go2FA.SetActive(true);
		GameManager.instance.Show2FAInput();
	}

	public void OnCardNumberInputChanged(string current)
	{
		if (current.Length >= 16)
		{
			if (current == numbers.Replace(" ", ""))
			{
				numbersOk = true;
				numbersCorrect.SetActive(true);
				numbersIncorrect.SetActive(false);
			}
			else
			{
				numbersOk = false;
				numbersCorrect.SetActive(false);
				numbersIncorrect.SetActive(true);
			}
		}
	}

	public void On2FAInputChanged(string current)
	{
		if (current.Length >= 4)
		{
			if (current == code2FA)
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
