using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text _normalControlText;
    [SerializeField] private TMP_Text _hardControlText;

    private void Start()
    {
        _normalControlText.gameObject.SetActive(GameplayHandler.Instance._Difficulty==0);
        _hardControlText.gameObject.SetActive(GameplayHandler.Instance._Difficulty!=0);
    }
}
