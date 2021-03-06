﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableDelay : MonoBehaviour
{
    [SerializeField] private float delay = 1;

    private void OnEnable()
    {
        StopCoroutine("TurnOffRoutine");
        StartCoroutine("TurnOffRoutine");
    }

    IEnumerator TurnOffRoutine()
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
