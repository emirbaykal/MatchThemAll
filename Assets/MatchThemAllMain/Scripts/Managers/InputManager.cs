using UnityEngine;
using System;
using MatchThemAllMain.Scripts.Gameplay.Powerup;
using MatchThemAllMain.Scripts.Managers;

public class InputManager : MonoBehaviour
{
    public static Action<Item> itemClicked;
    public static Action<Powerup> powerupClicked;
    
    [Header(" Settings ")]
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private LayerMask powerupLayer;
    
    private Item currentItem;

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.IsGame())
            HandleControl();
    }

    private void HandleControl()
    {
        if (Input.GetMouseButtonDown(0))
            HandleMouseDown();
        if (Input.GetMouseButton(0))
            HandleDrag();
        else if (Input.GetMouseButtonUp(0))
            HandleMouseUp();
    }

    private void HandleMouseDown()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out RaycastHit hit, 100, powerupLayer);
        
        if(hit.collider == null)
            return;

        powerupClicked?.Invoke(hit.collider.GetComponent<Powerup>());
    }

    private void HandleDrag()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out RaycastHit hit, 100);

        
        if (hit.collider == null)
        {
            DeselectCurrentItem();
            return;
        }

        if(hit.collider.transform.parent == null)
            return;
        
        if (!hit.collider.transform.parent.TryGetComponent(out Item item))
        {
            DeselectCurrentItem();
            return;
        }
        
        if (currentItem)
            currentItem.Deselect();

        currentItem = item;
        currentItem.Select(outlineMaterial);
    }

    private void DeselectCurrentItem()
    {
        if (currentItem)
            currentItem.Deselect();
        currentItem = null;
    }
    
    private void HandleMouseUp()
    {
        if (currentItem == null)
            return;
        
        currentItem.Deselect();
        itemClicked?.Invoke(currentItem);
        currentItem = null;

    }
}
