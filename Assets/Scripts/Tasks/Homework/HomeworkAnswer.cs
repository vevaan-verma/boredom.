using System;
using UnityEngine;

[Serializable]
public class HomeworkAnswer {

    [Header("Information")]
    [SerializeField] private string answerText;
    [SerializeField] private bool isCorrect;

    public void Initialize() {

        if (answerText.Length == 0)
            Debug.LogError("HomeworkAnswer:Initialize - Answer text cannot be empty.");

    }

    public string GetAnswerText() { return answerText; }

    public bool IsCorrect() { return isCorrect; }


}
