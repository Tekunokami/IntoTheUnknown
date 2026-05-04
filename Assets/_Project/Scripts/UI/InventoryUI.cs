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
    public CanvasGroup leftPageGroup;   // Replaced the single Canvas Group
    public CanvasGroup rightPageGroup;  // Replaced the single Canvas Group
    
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
    private void OnDisable() => controls.Disable();

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
        
        // CRITICAL FIX: Wait exactly 1 frame!
        yield return null; 

        if (inventoryPanel.activeInHierarchy && bookAnimator != null) 
        {
            bookAnimator.Play("Book_Closing");
        }

        yield return new WaitForSecondsRealtime(0.5f); // Wait for animation

        inventoryPanel.SetActive(false); 
        Time.timeScale = 1f; // Unpause game
    }

    private void RefreshUI()
    {
        foreach (Transform child in slotsContainer) Destroy(child.gameObject);
        if (GameManager.Instance == null) return;

        List<string> savedItems = GameManager.Instance.currentSaveData.inventoryItemIDs;
        foreach (string itemID in savedItems)
        {
            ItemData itemData = Resources.Load<ItemData>($"Items/{itemID}");
            if (itemData != null)
            {
                GameObject newSlot = Instantiate(slotPrefab, slotsContainer);
                Image icon = newSlot.transform.Find("Icon").GetComponent<Image>();
                icon.sprite = itemData.icon; 
            }
        }
    }

    private void UpdateStats()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateStatsDisplay();
        }
    }
}