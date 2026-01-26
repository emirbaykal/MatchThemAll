using TMPro;
using UnityEngine;

namespace MatchThemAllMain.Scripts.Gameplay.Powerup
{
    public enum EPowerupType
    {
        Vacuum = 0,
        Spring = 1,
        Fan = 2,
        FreezeGun = 3
    }
    public abstract class Powerup : MonoBehaviour
    {
        [Header(" Settings ")]
        [SerializeField] private EPowerupType powerupType;
        public EPowerupType PowerupType => powerupType;
        
        [Header(" Elements ")]
        [SerializeField] private TextMeshPro amountText;
        [SerializeField] private GameObject videoIcon;

        public void UpdateVisuals(int amount)
        {
            videoIcon.SetActive(amount <= 0);
            
            amountText.gameObject.SetActive(amount > 0);
            amountText.text = amount.ToString();
        }
    }
}