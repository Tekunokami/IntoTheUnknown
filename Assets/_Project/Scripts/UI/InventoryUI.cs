using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;       
    public Animator bookAnimator;           
    public CanvasGroup leftPageGroup;   
    public CanvasGroup rightPageGroup;  

    [Header("Equip Slot References")]
    public Transform headSlot;
    public Transform bodySlot;
    public Transform weaponSlot;
    public Transform accessorySlot;

    [Header("Grid References")]
    public Transform slotsContainer;  
    public GameObject slotPrefab;     

    private GameControls controls;
    private bool isInventoryOpen = false;
    private Coroutine animationCoroutine; 

    private void Awake()
    {
        controls = new GameControls();
        controls.Player.Inventory.performed += ctx => ToggleInventory();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable()
{
    if (controls != null) controls.Disable();
}

    private void Start()
    {
        inventoryPanel.SetActive(false); 
        leftPageGroup.alpha = 0; 
        rightPageGroup.alpha = 0; 
    }

    public void ToggleInventory()
    {   
        isInventoryOpen = !isInventoryOpen;

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);

        if (isInventoryOpen)
        {

            controls.Player.Attack.Disable(); // Disable combat when inventory is open
            Time.timeScale = 0f; // Pause game

            inventoryPanel.SetActive(true);
            RefreshUI();
            UpdateStats(); 
            animationCoroutine = StartCoroutine(OpenBookRoutine());
        }
        else
        {
            animationCoroutine = StartCoroutine(CloseBookRoutine());
        }
    }

    private IEnumerator OpenBookRoutine()
    {
        leftPageGroup.alpha = 0; 
        rightPageGroup.alpha = 0; 
        
        // Wait exactly 1 frame for the Animator to wake up
        yield return null; 

        if (inventoryPanel.activeInHierarchy && bookAnimator != null)  
        {
            bookAnimator.Play("Book_Opening");
        }
 
        yield return new WaitForSecondsRealtime(0.5f); // Wait for animation

        leftPageGroup.alpha = 1; 
        rightPageGroup.alpha = 1; 
    }

    private IEnumerator CloseBookRoutine()
    {
        leftPageGroup.alpha = 0; 
        rightPageGroup.alpha = 0; 
        
        // Wait exactly 1 frame!
        yield return null; 

        if (inventoryPanel.activeInHierarchy && bookAnimator != null) 
        {
            bookAnimator.Play("Book_Closing");
        }

        yield return new WaitForSecondsRealtime(0.5f); // Wait for animation

        inventoryPanel.SetActive(false); 
        Time.timeScale = 1f; // Unpause game

        yield return new WaitForEndOfFrame();
        controls.Player.Attack.Enable(); // Enable combat after inventory closed
    }

    private void RefreshUI()
    {
        if (GameManager.Instance == null) return;

        // Clear existing UI 
        foreach (Transform child in slotsContainer) Destroy(child.gameObject);
        foreach (Transform child in headSlot) Destroy(child.gameObject);
        foreach (Transform child in bodySlot) Destroy(child.gameObject);
        foreach (Transform child in weaponSlot) Destroy(child.gameObject);
        foreach (Transform child in accessorySlot) Destroy(child.gameObject);

        SaveData save = GameManager.Instance.currentSaveData;

        // For safety, prevent crash if lists are null
        if (save.inventoryItemIDs == null) save.inventoryItemIDs = new System.Collections.Generic.List<string>();
        if (save.equippedItemIDs == null) save.equippedItemIDs = new System.Collections.Generic.List<string>();
             
        int maxBagSize = 12; 
        for (int i = 0; i < maxBagSize; i++)
        {
            if (i < save.inventoryItemIDs.Count)
            {
                string itemID = save.inventoryItemIDs[i]; 
                ItemData itemData = GameManager.Instance.GetItemByID(itemID);
                SpawnItemIcon(itemData, slotsContainer);
            }
            else
            {
                SpawnItemIcon(null, slotsContainer);
            }
        }

        // Rebuild Equipped Items
        foreach (string equipID in save.equippedItemIDs)
        {
            if (string.IsNullOrEmpty(equipID)) continue;
            EquipmentData equipData = GameManager.Instance.GetItemByID(equipID) as EquipmentData;
            
            if (equipData != null)
            {
                Transform targetSlot = null;
                switch (equipData.equipSlot)
                {
                    case EquipSlot.Head: targetSlot = headSlot; break;
                    case EquipSlot.Body: targetSlot = bodySlot; break;
                    case EquipSlot.Weapon: targetSlot = weaponSlot; break;
                    case EquipSlot.Accessory: targetSlot = accessorySlot; break;
                }
                
                if (targetSlot != null) SpawnItemIcon(equipData, targetSlot);
            }
        }
    }

    // Helper function to spawn an item icon in a given slot
    private void SpawnItemIcon(ItemData itemData, Transform parentSlot)
    {
        GameObject newSlot = Instantiate(slotPrefab, parentSlot);
        
        // Based on slot scripts and sync their types
        InventorySlot parentSlotScript = parentSlot.GetComponent<InventorySlot>();
        InventorySlot newSlotScript = newSlot.GetComponent<InventorySlot>();

        if (parentSlotScript != null && newSlotScript != null)
        {
            newSlotScript.slotType = parentSlotScript.slotType;
        }

        // Centers the icon in the slot and sets it up
        RectTransform rt = newSlot.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        rt.localScale = Vector3.one;

        DraggableItem dragItem = newSlot.transform.Find("Icon").GetComponent<DraggableItem>();
        if (dragItem != null)
        {
            if (itemData != null)
            {
                dragItem.Setup(itemData);
                dragItem.GetComponent<Image>().color = Color.white;
            }
            else
            {
                Image iconImg = dragItem.GetComponent<Image>();
                iconImg.sprite = null;
                iconImg.color = Color.clear;
                iconImg.raycastTarget = false;
            }
        }
    }    private void UpdateStats()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateStatsDisplay();
        }
    }
}