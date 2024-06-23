using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public Sprite[] sprites;
    public float animationSpeed;

    private float nextFrameTime;
    private int index;
    private float timer;

    private SpriteRenderer myRenderer;

    private void Awake() {
        timer = Time.time;
        nextFrameTime = Time.time + animationSpeed;
        myRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (sprites.Length == 0) return;

        timer += Time.deltaTime;
        if(timer > nextFrameTime)
        {
            index++;
            
            if(index >= sprites.Length) { index = 0; }
            else if (index < 0) { index = sprites.Length - 1; }

            myRenderer.sprite = sprites[index];

            nextFrameTime = Time.time + animationSpeed;
        }
    }
}
