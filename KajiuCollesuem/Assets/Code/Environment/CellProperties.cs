﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellProperties : MonoBehaviour
{
    [SerializeField] private float fMaxHeight;
    private bool maxHeightReached = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z > fMaxHeight && !maxHeightReached)
        {
            transform.Translate(Vector3.back);
        }

        else
        {
            maxHeightReached = true;
        }
    }
}
