using System;
using System.Collections.Generic;
using MatchThemAll.Scripts;
using MatchThemAllMain.Scripts.Managers;
using MatchThemAllMain.Scripts.ScriptableObjects.Items;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpotsManager : MonoBehaviour
{
    public static  ItemSpotsManager instance;
    
    [Header(" Elements ")] 
    [SerializeField] private Transform itemSpotsParent;
    private ItemSpot[] spots;
    
    
    [Header(" Settings ")] 
    [SerializeField] private Vector3 itemLocalPositionOnSpot;
    //[SerializeField] private Vector3 itemLocalScaleOnSpot;
    private bool isBusy;
    
    [Header(" Data ")]
    private Dictionary<ItemData,ItemMergeData> itemMergeDatas = new Dictionary<ItemData,ItemMergeData>();
    
    [Header(" Animation Settings")]
    [SerializeField] private float animationDuration;
    [SerializeField] private LeanTweenType animationEasing;

    [Header(" Actions ")] 
    public static Action<List<Item>> mergeStarted;
    public static Action<Item> itemPickedUp;

     
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
        InputManager.itemClicked += OnItemClicked;
        PowerupManager.itemBackToGame += OnItemBackToGame;
        
        StoreSpots();
    }

    private void OnDestroy()
    {
        PowerupManager.itemBackToGame -= OnItemBackToGame;
        InputManager.itemClicked -= OnItemClicked;
    }

    private void OnItemBackToGame(Item releasedItem)
    {
        if(!itemMergeDatas.ContainsKey(releasedItem.ItemData))
            return;
        
        //Remove the item from the "itemMergeDatas"
        itemMergeDatas[releasedItem.ItemData].Remove(releasedItem);
        
        //Check if we have more items with the same
        //If not remove the "itemMergeDatas" entry
        if(itemMergeDatas[releasedItem.ItemData].items.Count <= 0)
            itemMergeDatas.Remove(releasedItem.ItemData);
        
    }


    private void OnItemClicked(Item item)
    {
        if (isBusy)
            return;

        if (!IsFreeSpotAvailable())
        {
            Debug.LogWarning("No Free Spot Available");
            return;
        }

        isBusy = true;
        
        itemPickedUp?.Invoke(item);
        
        HandleItemClicked(item);
        
    }

    private void HandleItemClicked(Item item)
    {
        if (itemMergeDatas.ContainsKey(item.ItemData))
            HandleItemMergeDataFound(item);
        else
            MoveItemToFirstFreeSpot(item);
    }

    private void HandleItemMergeDataFound(Item item)
    {
        ItemSpot idealSpot = GetIdealSpotFor(item);

        itemMergeDatas[item.ItemData].Add(item);
        
        TryMoveItemToIdealSpot(item, idealSpot);
    }

    private ItemSpot GetIdealSpotFor(Item item)
    {
        List<Item> items = itemMergeDatas[item.ItemData].items;
        List<ItemSpot> itemSpots = new List<ItemSpot>();

        for (int i = 0; i < items.Count; i++)
            itemSpots.Add(items[i].Spot);
        
        //We have a list of occupied spots by the items similar to item
        
        // If we only have one spot, we should simply grap the spot next to it

        if (itemSpots.Count >= 2)
        {
            itemSpots.Sort((a, b) =>b.transform.GetSiblingIndex().CompareTo(a.transform.GetSiblingIndex()));
        }

        int idealSpotIndex = itemSpots[0].transform.GetSiblingIndex() + 1;
        
        return spots[idealSpotIndex];
    }

    private void TryMoveItemToIdealSpot(Item item, ItemSpot idealSpot)
    {
        if (!idealSpot.IsEmpty())
        {
            HandleIdealSpotFull(item, idealSpot);
            return;
        }

        MoveItemToSpot(item, idealSpot, () => HandleItemReachedSpot(item));
    }

    private void MoveItemToSpot(Item item, ItemSpot targetSpot, Action completeCallback)
    {
        targetSpot.Populate(item);
        
        //Move animations
        LeanTween.moveLocal(item.gameObject, itemLocalPositionOnSpot, animationDuration)
            .setEase(animationEasing);
        LeanTween.scale(item.gameObject, item.ItemData.itemLocalScaleOnSpot, animationDuration)
            .setEase(animationEasing);
        LeanTween.rotateLocal(item.gameObject, Vector3.zero, animationDuration)
            .setEase(animationEasing)
            .setOnComplete(completeCallback);
        
        // Disable its shadow
        item.DisableShadow();

        // Disable its collider / physics
        item.DisablePhysics();
        
        //HandleItemReachedSpot(item, checkForMerge);
    }

    private void HandleIdealSpotFull(Item item, ItemSpot idealSpot)
    {
        MoveAllItemsToTheRightFrom(idealSpot, item);
    }

    private void MoveAllItemsToTheRightFrom(ItemSpot idealSpot, Item itemToPlace)
    {
         int spotIndex = idealSpot.transform.GetSiblingIndex();

         for (int i = spots.Length - 2; i >= spotIndex; i--)
         {
             ItemSpot spot = spots[i];
             
             if(spot.IsEmpty())
                 continue;
             
             Item item = spots[i].Item;
             
             spot.Clear();
             
             ItemSpot targetSpot = spots[i + 1];

             if (!targetSpot.IsEmpty())
             {
                 Debug.LogError("This should not happen");
                 isBusy = false;
                 return;
             }
             
             MoveItemToSpot(item, targetSpot, (() => HandleItemReachedSpot(item, false)));
         }
         
         MoveItemToSpot(itemToPlace,idealSpot, () => HandleItemReachedSpot(itemToPlace));
    }

    private void MoveItemToFirstFreeSpot(Item item)
    {
        ItemSpot targetSpot = GetFreeSpot();
        
        if(targetSpot == null)
            return;

        CreateItemMergeData(item);
        
        MoveItemToSpot(item,targetSpot, () => HandleFirstItemReachedSpot(item));
    }

    private void HandleItemReachedSpot(Item item, bool checkForMerge = true)
    {
        item.Spot.BumbDown();
        
        if(!checkForMerge)
            return;
        if (itemMergeDatas[item.ItemData].CanMergeItems())
            MergeItems(item.ItemData, itemMergeDatas[item.ItemData]);
        else
            CheckForGameOver();
    }

    private void MergeItems(ItemData itemData, ItemMergeData itemMergeData)
    {
        List<Item> items = itemMergeData.items;
        //Remove the item merge data from dictionary
        itemMergeDatas.Remove(itemData);

        for (int i = 0; i < items.Count; i++)
            items[i].Spot.Clear();

        if (itemMergeDatas.Count <= 0)
            isBusy = false;
        else
           MoveAllItemsToTheLeft(HandleAllItemsMovedToTheLeft);

        mergeStarted?.Invoke(items);
    }

    private void MoveAllItemsToTheLeft(Action completeCallback)
    {
        bool callbackTriggered = false;
        
        for (int i = 3; i < spots.Length; i++)
        {
            ItemSpot spot = spots[i];
            
            if (spot.IsEmpty())
                continue;

            Item item = spot.Item;
            //buraya bak bi 
            ItemSpot targetSpot = spots[i - 3];

            if (!targetSpot.IsEmpty())
            {
                isBusy = false;
                return;
            }
            
            spot.Clear();

            completeCallback += () => HandleItemReachedSpot(item, false);
            MoveItemToSpot(item, targetSpot, () => HandleItemReachedSpot(item, false));
        }

        if (!callbackTriggered)
        {
            completeCallback?.Invoke();
        }
    }

    private void HandleAllItemsMovedToTheLeft()
    {
        isBusy = false;
    }

    private void HandleFirstItemReachedSpot(Item item)
    {
        item.Spot.BumbDown();
        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        if (GetFreeSpot() == null)
            GameManager.instance.SetGameState(EGameState.GAMEOVER);
        else
            isBusy = false;
    }

    private void CreateItemMergeData(Item item)
    {
        itemMergeDatas.Add(item.ItemData, new ItemMergeData(item));
    }

    private ItemSpot GetFreeSpot()
    {
        for (int i = 0; i < spots.Length; i++)
        {
            if (spots[i].IsEmpty())
                return spots[i];
        }
        return null;
    }

    private void StoreSpots()
    {
        spots = new ItemSpot[itemSpotsParent.childCount];

        for (int i = 0; i < itemSpotsParent.childCount; i++)
            spots[i] = itemSpotsParent.GetChild(i).GetComponent<ItemSpot>();
    }
    private bool IsFreeSpotAvailable()
    {
        for (int i = 0; i < spots.Length; i++)
        {
            if (spots[i].IsEmpty())
                return true;
        }
        return false;
    }

    public ItemSpot GetRandomOccupiedSpot()
    {
        List<ItemSpot> occupiedSpots = new List<ItemSpot>();

        foreach (var spot in spots)
        {
            if(spot.IsEmpty())
                continue;
            
            occupiedSpots.Add(spot);
        }

        if (occupiedSpots.Count <= 0)
            return null;
        
        return occupiedSpots[Random.Range(0, occupiedSpots.Count)];
    } 
}
