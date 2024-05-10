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
}