using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelStatus {

    public int levelNum { get; set; }
    public bool completed { get; set; }

    public LevelStatus(int levelNum, bool completed) {

        this.levelNum = levelNum;
        this.completed = completed;

    }
}
