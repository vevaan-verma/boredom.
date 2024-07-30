using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSlot : MonoBehaviour, IDropHandler {

    [Header("References")]
    private UIController uiController;

    [Header("Slot")]
    private DraggableElement currElement;
    private int index;

    private void Start() {

        uiController = FindObjectOfType<UIController>();
        currElement = GetComponentInChildren<DraggableElement>();
        currElement.transform.localPosition = Vector2.zero;

        index = transform.GetSiblingIndex();

    }

    public DraggableElement GetCurrElement() { return currElement; }

    public void SwapElements(DraggableElement newElem) {

        newElem.SetSwitched(true); // only set for new element because that was the only one that was dragged

        DraggableElement tempElem = newElem;
        newElem.GetDragSlot().SetCurrElement(currElement);
        SetCurrElement(tempElem);

        uiController.OnTVRepairDrop();

    }

    public void SetCurrElement(DraggableElement elem) {

        currElement = elem;
        currElement.transform.SetParent(transform, false);
        currElement.SetDragSlot(this);
        currElement.transform.localPosition = Vector2.zero;

    }

    public void OnDrop(PointerEventData eventData) { // ON DRAG SLOT THAT WAS ELEMENT WAS DROPPED ON

        DraggableElement newElem = eventData.pointerDrag.GetComponent<DraggableElement>();

        if (transform.childCount != 0) // already has item (swap them)
            SwapElements(newElem);
        else // invalid drag slot
            newElem.SetDragParent(transform);

    }

    public int GetIndex() => index;

}
