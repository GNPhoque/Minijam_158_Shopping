<<<<<<< HEAD
using UnityEngine;
using UnityEngine.Events;

public class Clickable : MonoBehaviour
{
    public string clickableName;
    public UnityEvent eventWhenClicked;
    public bool debug;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        GameManager.instance.SetAction(clickableName);
    }

    private void OnMouseExit()
    {
        GameManager.instance.SetAction(GameManager.NONE_INTERACTABLE);
    }

    private void OnMouseDown()
    {
        if (debug) Debug.Log(gameObject.name + " got clicked!");
        eventWhenClicked.Invoke();
    }
=======
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
>>>>>>> 32ef386500c1cc1c97a92597ab7adb957f42f413
}
