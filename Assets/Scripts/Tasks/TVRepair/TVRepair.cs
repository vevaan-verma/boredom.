using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class TVRepair {

    [Header("Questions")]
    [SerializeField] private int questionsPerQuiz;
    [SerializeField] private TVRepairQuestion[] questions;
    private TVRepairQuestion[] currQuestions;

    public void Initialize() {

        foreach (TVRepairQuestion question in questions)
            question.Initialize();

        if (questionsPerQuiz > questions.Length)
            Debug.LogError("TVRepair:Initialize - Question per quiz exceeds total questions.");

    }

    public TVRepairQuestion[] GetRandomQuestions() {

        List<TVRepairQuestion> availableQuestions = new List<TVRepairQuestion>(questions);
        currQuestions = new TVRepairQuestion[questionsPerQuiz];

        for (int i = 0; i < questionsPerQuiz; i++) {

            int randIndex = UnityEngine.Random.Range(0, availableQuestions.Count);
            currQuestions[i] = availableQuestions[randIndex];
            availableQuestions.RemoveAt(randIndex);

        }

        return currQuestions;

    }

    public bool ValidateAnswers(List<TVRepairQuestionUI> questions, List<string> answers) {

        // validate answers
        for (int i = 0; i < questions.Count; i++)
            if (!questions[i].GetQuestion().IsCorrect(answers[i]))
                return false;

        return true;

    }
}
