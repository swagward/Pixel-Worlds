﻿using TerrariaClone.Runtime.Player;
using UnityEngine;

namespace TerrariaClone.Runtime.Data
{
    [CreateAssetMenu(fileName = "HealthPotion", menuName = "Pixel Worlds/Items/Consumables/HealthPotion", order = 0)]
    public class HealthPotionClass : ConsumableClass
    {
        public int healthToAdd;

        public override void Use(PlayerController caller)
        {
            if (Input.GetMouseButtonDown(0) && caller.health.currentHealth < caller.health.currentHealth - healthToAdd)
            {
                base.Use(caller);
                caller.health.AddHealth(healthToAdd);
            }
        }
    }
}
