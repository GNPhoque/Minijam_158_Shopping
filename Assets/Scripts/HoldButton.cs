using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
	public static float spentMoney;
	[SerializeField] float holdDuration;
	[SerializeField] float nextCost;
	[SerializeField] Image fill;

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
		spentMoney = 0f;
		popupText.color = new Color(popupText.color.r, popupText.color.g, popupText.color.b, 0);
	}

	private void Update()
	{
		if (isHeldDown)
		{
			currentHoldDuration += Time.deltaTime;
			fill.fillAmount = currentHoldDuration / holdDuration;
			if (currentHoldDuration > holdDuration) HoldSuccess();
		}

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
	}

	public void OnPointerDown(PointerEventData eventData)
	{
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
		popupText.text = GameManager.instance.BuyItem();
		currentHoldDuration -= holdDuration;
	}
}
