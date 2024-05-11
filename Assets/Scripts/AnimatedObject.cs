using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable

public class AnimatedObject : MonoBehaviour
{
    Animator anim;

    [SerializeField] string playAnimationOnStart = "None";

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        if (playAnimationOnStart != "None") anim.Play(playAnimationOnStart);
    }

    public void PlayAnimation(string name)
    {
        anim.Play(name);
    }
}
