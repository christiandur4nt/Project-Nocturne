using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBlink : MonoBehaviour
{
    public TextMeshProUGUI text;
    private Color color;

    void Start() {
        color = text.color;
    }

    // Update is called once per frame
    void Update()
    {
        color.a = Mathf.PingPong(Time.time/6, 1f);
        text.color = color;
    }
}
