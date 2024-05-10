using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
	public static float spentMoney;
	[SerializeField] float holdDuration;
	[SerializeField] float nextCost;

	float currentHoldDuration;
	bool isHeldDown;

	private void Start()
	{
		spentMoney = 0f;
	}

	private void Update()
	{
		if (!isHeldDown) return;

		currentHoldDuration += Time.deltaTime;
		if (currentHoldDuration > holdDuration) HoldSuccess();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		isHeldDown = true;
		currentHoldDuration = 0f;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHeldDown = false;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isHeldDown = false;
	}

	private void HoldSuccess()
	{
		spentMoney += nextCost;
		GameManager.instance.UpdateScore(spentMoney);
		currentHoldDuration -= holdDuration;
	}
}
