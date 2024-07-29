using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TVRepairQuestionUI : MonoBehaviour {

    [Header("References")]
    [SerializeField] private TMP_Text questionText;
    private TVRepairQuestion question;

    public void SetQuestionText(TVRepairQuestion question) {

        this.question = question;
        questionText.text = question.GetQuestionText();

    }

    public string GetQuestionText() => questionText.text;

    public int GetIndex() => transform.parent.GetSiblingIndex();

    public TVRepairQuestion GetQuestion() => question;

}
