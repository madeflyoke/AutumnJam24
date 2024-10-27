using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Texter : MonoBehaviour
{
    [SerializeField] private TMP_Text _state;

    private void Update()
    {
        _state.text = $"InputX {Input.GetAxis("Mouse X")} InputY {Input.GetAxis("Mouse Y")}";
    }
}
