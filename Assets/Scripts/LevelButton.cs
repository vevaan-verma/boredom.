using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : Button {

    [Header("References")]
    [SerializeField] private TMP_Text levelNumText;
    private Level level;

    public void Initialize(int levelNum, Level level) {

        this.level = level;
        levelNumText.text = levelNum + "";

    }

    public Level GetLevel() => level;

}
