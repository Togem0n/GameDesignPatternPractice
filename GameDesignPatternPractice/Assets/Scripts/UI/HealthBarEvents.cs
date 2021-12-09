using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthBarEvents : MonoBehaviour
{
    public static HealthBarEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action onHealthDown;
    public event Action onHealthUp;

    public void HealthDown()
    {
        if(onHealthDown != null)
        {
            onHealthDown();
        }
    }

    public void HealthUp()
    {
        if (onHealthUp != null)
        {
            onHealthUp();
        }
    }
}
