using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDetailsUpdater : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI numbers;

	private void Start()
	{
		UpdateNumbers();
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
		numbers.text = newNumbers;
	}
}
