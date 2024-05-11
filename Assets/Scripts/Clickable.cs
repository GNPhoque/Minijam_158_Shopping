using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour, IPointerClickHandler
{
    public string clickableName;
    public bool debug;

    public UnityEvent eventWhenClicked;
    public UnityEvent eventWhenHovered;
    public UnityEvent eventWhenExitHover;

    public AnimatedObject[] handleSelectedAnimationsOnAnimatedObjects;

    void Awake()
    {
        if (clickableName != "") GetComponent<SpriteRenderer>().enabled = debug;
    }

    private void OnMouseEnter()
    {
        GameManager.instance.SetActionText(clickableName);
        eventWhenHovered.Invoke();
        HandleAnimatedObjectAnimation(selected: true);
    }

    private void OnMouseExit()
    {
        GameManager.instance.SetActionText(GameManager.NONE_INTERACTABLE);
        eventWhenExitHover.Invoke();
        HandleAnimatedObjectAnimation(selected: false);
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

    void HandleAnimatedObjectAnimation(bool selected)
    {
        if (selected)
        {
            // Play the selected animation
            foreach (AnimatedObject animObj in handleSelectedAnimationsOnAnimatedObjects)
            {
                animObj.PlayAnimation("Selected");
            }
        }
        else
        {
            // Play the default animation
            foreach (AnimatedObject animObj in handleSelectedAnimationsOnAnimatedObjects)
            {
                animObj.PlayAnimation("Default");
            }
        }
    }
}