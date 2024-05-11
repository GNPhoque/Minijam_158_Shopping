using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mom : MonoBehaviour
{
    [SerializeField] private AnimatedObject momCouchAnim;
    public bool momActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableMomAtCouch()
    {
        momActive = true;
        momCouchAnim.gameObject.SetActive(true);
        momCouchAnim.PlayAnimation("StartPeeking");
    }

    public void DisableMomAtCouch()
    {
        momActive = false;
        StartCoroutine(WaitForMomEndPeeking(2f));
    }

    IEnumerator WaitForMomEndPeeking(float delay)
    {
        yield return new WaitForSeconds(delay);
        momCouchAnim.PlayAnimation("GoAway");
    }
}
