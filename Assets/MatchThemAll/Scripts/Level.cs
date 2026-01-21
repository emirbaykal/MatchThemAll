using MatchThemAll.Scripts;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header(" Settings ")]
    [SerializeField] private ItemPlacer itemPlacer;

    public ItemLevelData[] GetGoals()
        => itemPlacer.getGoals();
}
