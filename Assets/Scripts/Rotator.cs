using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 360f;

    void Update()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotationAmount, 0);
    }
}
