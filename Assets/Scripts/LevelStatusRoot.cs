using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelStatusRoot {

    public List<LevelStatus> levels { get; set; }

    public LevelStatusRoot() => levels = new List<LevelStatus>();

}
