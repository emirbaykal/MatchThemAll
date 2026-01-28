using MatchThemAll.Scripts;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header(" Settings ")]
    [SerializeField] private ItemPlacer itemPlacer;
    public Transform ItemParent => itemPlacer.transform;

    [Header(" Settings ")] 
    [SerializeField] private int duration;
    public int Duration => duration;
    public ItemLevelData[] GetGoals()
        => itemPlacer.GetGoals();

    public Item[] GetItems()
    {
        return itemPlacer.GetItems();
    }
}
