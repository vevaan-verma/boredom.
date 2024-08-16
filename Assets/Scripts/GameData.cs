using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {

    [Header("Quiz")]
    [SerializeField] private Homework homework;

    [Header("TV Repair")]
    [SerializeField] private TVRepair tvRepair;

    private void Start() {

        homework.Initialize();
        tvRepair.Initialize();

    }

    public Homework GetQuiz() { return homework; }

    public TVRepair GetTVRepair() { return tvRepair; }

}
