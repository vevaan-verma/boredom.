using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TVRepairAnswerUI : MonoBehaviour {

    [Header("References")]
    private TMP_Text answerText;
    private int index;

    public void Initialize() {

        answerText = GetComponent<TMP_Text>();
        index = transform.parent.GetSiblingIndex();

    }

    public void SetAnswerText(string answer) => answerText.text = answer;

    public string GetAnswerText() => answerText.text;

    public int GetIndex() => index;

}
