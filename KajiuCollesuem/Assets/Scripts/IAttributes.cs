﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttributes
{
    void TakeDamage(int damage, bool react, GameObject pKiller);
}
