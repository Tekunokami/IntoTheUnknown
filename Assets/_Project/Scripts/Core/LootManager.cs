using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance;

    // Item pools by rarity
    private List<ItemData> poorItems = new List<ItemData>();
    private List<ItemData> commonItems = new List<ItemData>();
    private List<ItemData> rareItems = new List<ItemData>();
    private List<ItemData> epicItems = new List<ItemData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeLootPools();
        }
    }

    // At the start of the game, load all items and categorize them
    private void InitializeLootPools()
    {
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
        foreach (var item in allItems)
        {
            if (item.rarity == ItemRarity.Poor) poorItems.Add(item);
            else if (item.rarity == ItemRarity.Common) commonItems.Add(item);
            else if (item.rarity == ItemRarity.Rare) rareItems.Add(item);
            else if (item.rarity == ItemRarity.Epic) epicItems.Add(item);
        }
    }

    // When chests are opened, call this function
    public List<ItemData> GenerateChestLoot(int itemCount)
    {
        List<ItemData> droppedItems = new List<ItemData>();
        SaveData save = GameManager.Instance.currentSaveData;

        // Luck mechanics:
        bool forceGoodItem = save.chestBadLuckCounter >= 4; // If 4 bad drops in a row, guarantee a good item
        bool forceBadItem = save.chestGoodLuckCounter >= 2;  // If 2 good drops in a row, increase chance of bad item
        
        bool chestGotGoodItem = false;

        for (int i = 0; i < itemCount; i++)
        {
            ItemRarity rolledRarity;

            if (forceGoodItem && i == 0) 
            {
                // Bad luck protection
                rolledRarity = Random.value > 0.33f ? ItemRarity.Rare : ItemRarity.Epic;
            }
            else if (forceBadItem)
            {
                // Prevent too much good luck
                rolledRarity = Random.value > 0.15f ? ItemRarity.Common : ItemRarity.Poor;
            }
            else
            {
                // RNG roll for rarity
                float rand = Random.Range(0f, 100f);

                if (rand <= 15f) rolledRarity = ItemRarity.Poor;           // %15 chance
                else if (rand <= 85f) rolledRarity = ItemRarity.Common;    // %70 chance (15 + 70)
                else if (rand <= 95f) rolledRarity = ItemRarity.Rare;      // %10 chance (85 + 10)
                else rolledRarity = ItemRarity.Epic;                       // %5 chance  (95 + 5)
            }

            // Track if we got a good item for luck mechanics
            if (rolledRarity == ItemRarity.Rare || rolledRarity == ItemRarity.Epic)
            {
                chestGotGoodItem = true;
            }

            // Get a random item from the selected pool and add it to the list
            ItemData rolledItem = GetRandomItemFromPool(rolledRarity);
            if (rolledItem != null) droppedItems.Add(rolledItem);
        }

        // --- UPDATE LUCK TRACKING ---
        if (chestGotGoodItem)
        {
            save.chestBadLuckCounter = 0; // Bad luck pity counter reset
            save.chestGoodLuckCounter++;  // Good luck counter increased
        }
        else
        {
            save.chestBadLuckCounter++;   // Bad luck counter increased
            save.chestGoodLuckCounter = 0;// Good luck counter reset
        }

        return droppedItems;
    }

    private ItemData GetRandomItemFromPool(ItemRarity rarity)
    {
        List<ItemData> targetPool = commonItems; // Default to common if something goes wrongs

        if (rarity == ItemRarity.Poor && poorItems.Count > 0) targetPool = poorItems;
        else if (rarity == ItemRarity.Common && commonItems.Count > 0) targetPool = commonItems;
        else if (rarity == ItemRarity.Rare && rareItems.Count > 0) targetPool = rareItems;
        else if (rarity == ItemRarity.Epic && epicItems.Count > 0) targetPool = epicItems;

        if (targetPool.Count == 0) return null; // If the pool is completely empty, return null to avoid errors
        
        return targetPool[Random.Range(0, targetPool.Count)];
    }
}