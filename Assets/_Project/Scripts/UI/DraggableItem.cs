using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemData itemData;
    public Image iconImage;
    
    [HideInInspector] public Transform originalParent;

    public void Setup(ItemData data)
    {
        itemData = data;
        iconImage.sprite = data.icon; 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; // Remember where we came from
        
        // Move the icon to the very top UI layer so it doesn't get stuck behind
        transform.SetParent(transform.root); 
        transform.SetAsLastSibling();
        
        // Turn off raycasting on the icon so the mouse can "see" the slot behind it when dropping
        iconImage.raycastTarget = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Follow the mouse position
        transform.position = eventData.position; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        iconImage.raycastTarget = true; // Turn raycasting back on
        
        // If we didn't drop it on a valid slot, snap it back to where it started
        if (transform.parent == transform.root)
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }
    }
}