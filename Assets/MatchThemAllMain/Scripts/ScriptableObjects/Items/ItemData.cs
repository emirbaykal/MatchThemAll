using UnityEngine;

namespace MatchThemAllMain.Scripts.ScriptableObjects.Items
{
    [CreateAssetMenu(menuName =  "Scriptable Objects/Items/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Data")]
        public string itemID;
        
        public Vector3 itemLocalScaleOnSpot;
        
        public GameObject itemPrefab;
        
        [Header("Item Asset")]
        public Sprite icon;
        

        
    }
}