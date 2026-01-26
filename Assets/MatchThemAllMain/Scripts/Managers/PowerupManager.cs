using System;
using System.Collections.Generic;
using System.Linq;
using MatchThemAll.Scripts;
using MatchThemAllMain.Scripts.Gameplay.Powerup;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

public class PowerupManager : MonoBehaviour
{
    [Header(" Vacuum Elements ")] 
    [SerializeField] private Vacuum vacuum;
    [SerializeField] private Transform vacuumSuckPosition;

    [Header(" Settings ")] 
    private bool isBusy;
    private int vacuumItemsToCollect;
    private int vacuumCounter;
    
    [Header(" Actions ")] 
    public static Action<Item> itemPickedUp;

    [Header(" Data ")] 
    [SerializeField] private int initialPUCount;

    private int vacuumPUCount;

    private void Update()
    {
        Debug.Log(isBusy);
    }

    private void Awake()
    {
        LoadData();
        
        Vacuum.started += OnVacuumStarted;
        InputManager.powerupClicked += OnPowerupClicked;
    }

    private void OnDestroy()
    {
        Vacuum.started -= OnVacuumStarted;
        InputManager.powerupClicked -= OnPowerupClicked;
    }

    private void OnPowerupClicked(Powerup powerup)
    {
        if(isBusy)
            return;

        switch (powerup.PowerupType)
        {
            case EPowerupType.Vacuum:
                HandleVacuumClicked();
                UpdateVacuumVisuals();
                break;
        }
    }

    private void HandleVacuumClicked()
    {
        if (vacuumPUCount <= 0)
        {
            vacuumPUCount = 3;
            SaveData();
        }
        else
        {
            isBusy = true;
            
            vacuumPUCount--;
            SaveData();
            
            vacuum.Play();
        }
    }

    private void OnVacuumStarted()
    {
        VacuumPowerup();
    }
    
    [Button]
    private void VacuumPowerup()
    {
        Item[] items = LevelManager.instance.Items;
        ItemLevelData[] goals = GoalManager.instance.Goals;

        ItemLevelData? greatestGoal = GetGreatestGoal(goals);
        
        if(greatestGoal == null)
            return;
        
        ItemLevelData goal = (ItemLevelData)greatestGoal;

        vacuumCounter = 0;

        List<Item> itemsToCollect = new List<Item>();

        foreach (Item item in items)
        {
            if(item == null)
                continue;
            
            if (item.ItemData == goal.itemPrefab.ItemData)
            {
                itemsToCollect.Add(item);
                
                if(itemsToCollect.Count >= 3)
                    break;
            }
        }

        vacuumItemsToCollect = itemsToCollect.Count;
        
        for (int i = 0; i < itemsToCollect.Count; i++)
        {
            itemsToCollect[i].DisablePhysics();
            
            Item itemToCollect = itemsToCollect[i];
            
            ///TEKRAR BAK BU MOVE ISLEMINE POLISH VE OPTIMIZASYON
            
            List<Vector3> movePoints = new List<Vector3>();
            
            movePoints.Add(itemsToCollect[i].transform.position);
            movePoints.Add(itemsToCollect[i].transform.position);
            
            movePoints.Add(itemsToCollect[i].transform.position + Vector3.up * 2);
            //movePoints.Add(vacuumSuckPosition.position + Vector3.up * 2);

            movePoints.Add(vacuumSuckPosition.position);
            movePoints.Add(vacuumSuckPosition.position);
            
            LeanTween.moveSpline(itemsToCollect[i].gameObject, movePoints.ToArray(), 0.5f)                
                .setOnComplete(() => ItemReachedVacuum(itemToCollect));

            LeanTween.scale(itemsToCollect[i].gameObject, Vector3.zero, 1f);
        }
        
        for (int i = itemsToCollect.Count - 1; i >= 0; i--)
        {
            itemPickedUp?.Invoke(itemsToCollect[i]);
            //Destroy(itemsToCollect[i].gameObject);
        }
    }

    private void ItemReachedVacuum(Item item)
    {
        vacuumCounter++;
        
        if (vacuumCounter >= vacuumItemsToCollect)
            isBusy = false;
        
        Destroy(item.gameObject);
    }

    private ItemLevelData? GetGreatestGoal(ItemLevelData[] goals)
    {
        return goals.OrderByDescending(g => g.amount).FirstOrDefault();
    }

    private void UpdateVacuumVisuals()
    {
        vacuum.UpdateVisuals(vacuumPUCount);
    }
    
    private void LoadData()
    {
        vacuumPUCount = PlayerPrefs.GetInt("VacuumCount", initialPUCount);

        UpdateVacuumVisuals();
    }

    

    private void SaveData()
    {
        PlayerPrefs.SetInt("VacuumCount", vacuumPUCount);
    }
}
