using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Homework {

    [Header("Questions")]
    [SerializeField] private int questionsPerQuiz;
    [SerializeField] private HomeworkQuestion[] questions;
    private HomeworkQuestion[] currQuestions;

    [Header("Checking")]
    [SerializeField] private Color correctColor;
    [SerializeField] private string correctMarker;
    [SerializeField] private Color incorrectColor;
    [SerializeField] private string incorrectMarker;
    [SerializeField] private float markerFadeDuration;

    public void Initialize() {

        foreach (HomeworkQuestion question in questions)
            question.Initialize();

        if (questionsPerQuiz > questions.Length)
            Debug.LogError("Homework:Initialize - Question per quiz exceeds total questions.");

    }

    public HomeworkQuestion[] GetRandomQuestions() {

        List<HomeworkQuestion> availableQuestions = new List<HomeworkQuestion>(questions);
        currQuestions = new HomeworkQuestion[questionsPerQuiz];

        for (int i = 0; i < questionsPerQuiz; i++) {

            int randIndex = UnityEngine.Random.Range(0, availableQuestions.Count);
            currQuestions[i] = availableQuestions[randIndex];
            availableQuestions.RemoveAt(randIndex);

        }

        return currQuestions;

    }

    public bool ValidateAnswers(List<HomeworkQuestionUI> questionUIs) {

        for (int i = 0; i < questionUIs.Count; i++)
            if (questionUIs[i].GetSelectedIndex() == -1) // a question is unanswered
                return false;

        bool allCorrect = true;

        // validate answers
        for (int i = 0; i < currQuestions.Length; i++) {

            if (currQuestions[i].IsCorrect(questionUIs[i].GetSelectedIndex())) {

                questionUIs[i].SetScoreMarker(correctMarker, correctColor, markerFadeDuration);

            } else {

                questionUIs[i].SetScoreMarker(incorrectMarker, incorrectColor, markerFadeDuration);
                allCorrect = false;

            }
        }

        return allCorrect;

    }
}