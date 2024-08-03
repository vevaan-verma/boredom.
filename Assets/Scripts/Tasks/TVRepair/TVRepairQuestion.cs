using System;
using UnityEngine;

[Serializable]
public class TVRepairQuestion {

    [Header("Information")]
    [SerializeField] private string questionText;
    [SerializeField] private TVRepairAnswer answer;

    public void Initialize() {

        answer.Initialize();

        if (questionText.Length == 0)
            Debug.LogError("TVRepairQuestion:Initialize - Question text cannot be empty.");

    }

    public string GetQuestionText() { return questionText; }

    public string GetAnswerText() { return answer.GetAnswerText(); }

    public bool IsCorrect(string answer) { return this.answer.GetAnswerText().Equals(answer); }

}
