using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
	[SerializeField] bool MAIN_MENU;
	[SerializeField] UnityEvent tutoOnBuyComplete;
	bool tutoBoughtOne;
	bool tutoBoughtMultiple;

	[SerializeField] float holdDuration;
	[SerializeField] float nextCost;
	[SerializeField] Image fill;
	[SerializeField] CardDetailsUpdater cardDetailsUpdater;

	[Header("Popup")]
	[SerializeField] RectTransform popup;
	[SerializeField] TextMeshProUGUI popupText;
	[SerializeField] float popupAnimationTime;
	[SerializeField] float popupAnimationPositionMultiplier;
	[SerializeField] List<string> boughtPopups;
	[SerializeField] AnimationCurve popupCurve;
	[SerializeField] AnimationCurve popupOpacityCurve;

	float currentHoldDuration;
	float currentPopupAnimationTime;
	bool isHeldDown;
	bool animatingPopup;

	private void Start()
	{
		if (MAIN_MENU) return;

		GameManager.instance.spentMoney = 0f;
		popupText.color = new Color(popupText.color.r, popupText.color.g, popupText.color.b, 0);
	}

	private void Update()
	{
		if (animatingPopup && currentPopupAnimationTime < popupAnimationTime)
		{
			// print(currentPopupAnimationTime);
			currentPopupAnimationTime += Time.deltaTime;
			popup.anchoredPosition = Vector3.up * popupCurve.Evaluate(currentPopupAnimationTime) * popupAnimationPositionMultiplier;
			popupText.color = new Color(popupText.color.r, popupText.color.g, popupText.color.b, popupOpacityCurve.Evaluate(currentPopupAnimationTime));
			
			if (currentPopupAnimationTime >= popupAnimationTime)
			{
				animatingPopup = false;
				popupText.color = new Color(popupText.color.r, popupText.color.g, popupText.color.b, 0);
			}
		}

		if (!tutoBoughtMultiple && isHeldDown)
		{
			if (!MAIN_MENU && !cardDetailsUpdater.CanBuy)
			{
				isHeldDown = false;
				currentHoldDuration = 0f;
				return;
			}

			currentHoldDuration += Time.deltaTime;
			fill.fillAmount = currentHoldDuration / holdDuration;
			if (currentHoldDuration > holdDuration) HoldSuccess();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!MAIN_MENU && !cardDetailsUpdater.CanBuy) return;

		isHeldDown = true;
		currentHoldDuration = 0f;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		fill.fillAmount = 0;
		isHeldDown = false;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		fill.fillAmount = 0;
		isHeldDown = false;
	}

	private void HoldSuccess()
	{
		fill.fillAmount = 0;
		animatingPopup = true;
		currentPopupAnimationTime = 0f;
		currentHoldDuration -= holdDuration;
		if (MAIN_MENU)
		{
			popupText.text = "B-bucks $19.99";

			if(tutoBoughtOne)
			{
				tutoBoughtMultiple = true;
                tutoOnBuyComplete.Invoke();
            }
            tutoBoughtOne = true;
		}
		else
		{
			popupText.text = GameManager.instance.BuyItem();
		}
	}
}
