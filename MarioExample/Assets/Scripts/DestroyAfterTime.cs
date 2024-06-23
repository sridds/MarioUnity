using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float time;
    void Start()
    {
        Invoke(nameof(Death), time);
    }

    private void Death() => Destroy(gameObject);
}
