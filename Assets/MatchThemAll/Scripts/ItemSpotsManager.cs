using UnityEngine;

public class ItemSpotsManager : MonoBehaviour
{
    [Header(" Elements ")] 
    [SerializeField] private Transform itemSpot;
    
    [Header(" Settings ")] 
    [SerializeField] private Vector3 itemLocalPositionOnSpot;
    [SerializeField] private Vector3 itemLocalScaleOnSpot;

     
    private void Awake()
    {
        InputManager.itemClicked += OnItemClicked;
    }

    private void OnDestroy()
    {
        InputManager.itemClicked -= OnItemClicked;
    }
    
    
    private void OnItemClicked(Item item)
    {
        item.transform.SetParent(itemSpot);
          
        item.transform.localPosition = itemLocalPositionOnSpot;
        item.transform.localScale = itemLocalScaleOnSpot;
        item.transform.localRotation = Quaternion.identity;

        item.DisableShadow();

        item.DisablePhysics();
    }
}
