using System;
using UnityEngine;

namespace MatchThemAllMain.Scripts.Gameplay.Powerup
{
    public class Hammer : Powerup
    {
        [Header(" Elements ")]
        [SerializeField] private Animator animator;
        
        [Header(" Actions ")] 
        public static Action started;

        private void TriggerPowerupStart()
        {
            started?.Invoke();
        }

        public void Play()
        {
            animator.Play("Hammer_Activate");
        }
    }
}