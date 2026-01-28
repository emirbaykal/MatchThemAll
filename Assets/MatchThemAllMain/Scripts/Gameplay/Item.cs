using System;
using MatchThemAll.Scripts;
using MatchThemAllMain.Scripts.ScriptableObjects.Items;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    [Header(" Data ")]
    [SerializeField] private ItemData itemData;
    public ItemData ItemData => itemData;

    private ItemSpot spot;
    public ItemSpot Spot => spot;
    
    [Header(" Elements ")] 
    [SerializeField] private Renderer renderer;
    [SerializeField] private Collider collider;
    private Material baseMaterial;

    private void Awake()
    {
        baseMaterial = renderer.material;
    }
    
    public void AssignSpot(ItemSpot spot)
        => this.spot = spot;
    
    public void UnassignSpot()
        => spot = null;

    public void DisableShadow()
    {
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
    
    public void EnhableShadow()
    {
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
    
    public void DisablePhysics()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        collider.enabled = false;
    }
    public void EnablePhysics()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        collider.enabled = true;
    }

    public void Select(Material outlineMaterial)
    {
        renderer.materials = new Material[] { baseMaterial, outlineMaterial };
    }

    public void Deselect()
    {
        renderer.materials = new Material[] { baseMaterial};
    }

    public void ApplyRandomForce(float magnitude)
    {
        GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * magnitude, ForceMode.VelocityChange);
    }

}
