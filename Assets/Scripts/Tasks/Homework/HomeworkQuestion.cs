using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HomeworkQuestion {

    [Header("Answers")]
    [SerializeField] private string questionText;
    [SerializeField] private HomeworkAnswer[] answers;

    public void Initialize() {

        // make sure all conditions are met
        int correctAnswers = 0;

        foreach (HomeworkAnswer answer in answers) {

            answer.Initialize();

            if (answer.IsCorrect())
                correctAnswers++;

        }

        if (questionText.Length == 0)
            Debug.LogError("HomeworkQuestion:Initialize - Question text cannot be empty.");

        if (correctAnswers == 0)
            Debug.LogError("HomeworkQuestion:Initialize - There must be a correct answer.\nQuestion: " + questionText);

    }

    public string GetQuestionText() { return questionText; }

    public string[] GetOptions() {

        string[] options = new string[answers.Length];

        for (int i = 0; i < answers.Length; i++)
            options[i] = answers[i].GetAnswerText();

        return options;

    }

    public bool IsCorrect(int answerIndex) { return answers[answerIndex].IsCorrect(); }

    public int GetAnswerCount() { return answers.Length; }

}