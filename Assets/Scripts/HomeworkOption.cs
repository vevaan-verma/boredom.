using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeworkOption : MonoBehaviour {

    [Header("References")]
    private Button button;

    [Header("Order")]
    private int optionIndex;

    public void Initialize(HomeworkQuestionUI questionUI, int optionIndex) {

        this.optionIndex = optionIndex;
        button = GetComponent<Button>();

        button.onClick.AddListener(() => questionUI.OnOptionSelect(this));

    }

    public Button GetButton() { return button; }

    public int GetIndex() { return optionIndex; }

}