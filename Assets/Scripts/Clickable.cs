using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] UnityEvent action;

	public void OnPointerClick(PointerEventData eventData)
	{
		action?.Invoke();
	}
}
