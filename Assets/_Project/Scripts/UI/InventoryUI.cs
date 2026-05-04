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
        
        // Wait exactly 1 frame!
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
        ItemData[] allGameItems = Resources.LoadAll<ItemData>("Items");

        foreach (string itemID in savedItems)
        {
            if (string.IsNullOrEmpty(itemID)) continue; // Skip empty saves
            
            ItemData itemData = System.Array.Find(allGameItems, item => item.itemID == itemID);

            if (itemData != null)
            {
                if (slotPrefab == null)
                {
                    Debug.LogError("CRASH AVOIDED: Your Slot Prefab is missing in the InventoryUI Inspector!");
                    return;
                }

                GameObject newSlot = Instantiate(slotPrefab, slotsContainer);
                Transform iconTransform = newSlot.transform.Find("Icon");
                
                if (iconTransform == null)
                {
                    Debug.LogError("CRASH AVOIDED: Your Slot Prefab does not have a child object named EXACTLY 'Icon'!");
                    continue;
                }

                Image icon = iconTransform.GetComponent<Image>();
                
                icon.sprite = itemData.icon; 
            }
            else
            {
                Debug.LogWarning($"Inventory Warning: Could not find any ItemData in Resources/Items with the ID: '{itemID}'");
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