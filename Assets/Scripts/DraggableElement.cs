using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    [Header("References")]
    [SerializeField] private Transform tvRepair;
    private DragSlot dragSlot;
    private Image image;

    [Header("Drag")]
    private Transform dragParent;
    private bool switched;

    private void Start() {

        dragSlot = transform.parent.GetComponent<DragSlot>();
        image = GetComponent<Image>();

    }

    public void OnBeginDrag(PointerEventData eventData) {

        dragParent = transform.parent;
        transform.SetParent(tvRepair);
        transform.SetAsLastSibling();
        image.raycastTarget = false;

    }

    public void OnDrag(PointerEventData eventData) {

        transform.position = new Vector2(transform.position.x, Input.mousePosition.y);

    }

    public void OnEndDrag(PointerEventData eventData) {

        if (switched) { // don't reset position if element was switched

            image.raycastTarget = true;
            switched = false;
            return;

        }

        transform.SetParent(dragParent);
        image.raycastTarget = true;

    }

    public void SetDragParent(Transform dragParent) => this.dragParent = dragParent;

    public DragSlot GetDragSlot() => dragSlot;

    public void SetDragSlot(DragSlot dragSlot) => this.dragSlot = dragSlot;

    public void SetSwitched(bool switched) => this.switched = switched;

    public int GetIndex() => dragSlot.GetIndex();

}
