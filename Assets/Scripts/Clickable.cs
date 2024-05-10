using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour, IPointerClickHandler
{
    public string clickableName;
    public UnityEvent eventWhenClicked;
    public bool debug;

    void Awake()
    {
        if (clickableName != "") GetComponent<SpriteRenderer>().enabled = debug;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseEnter()
    {
        GameManager.instance.SetActionText(clickableName);
    }

    private void OnMouseExit()
    {
        GameManager.instance.SetActionText(GameManager.NONE_INTERACTABLE);
    }

    private void OnMouseDown()
    {
        if (debug) Debug.Log(gameObject.name + " got clicked!");
        eventWhenClicked.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
	{
		if (debug) Debug.Log(gameObject.name + " got clicked!");
		eventWhenClicked.Invoke();
	}
}