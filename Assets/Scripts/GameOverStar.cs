using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverStar : MonoBehaviour
{
    [SerializeField] RectTransform rect;
    [SerializeField] AnimationCurve curve;
    [SerializeField] float size;

    float currentTime;

    void Update()
    {
        currentTime += Time.deltaTime * 1.5f;
        float currentvalue = curve.Evaluate(currentTime);

        rect.sizeDelta = Vector2.one * currentvalue * size;
    }
}
