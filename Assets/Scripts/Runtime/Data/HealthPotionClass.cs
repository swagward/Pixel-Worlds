﻿using PixelWorlds.Runtime.Player;
using UnityEngine;

namespace PixelWorlds.Runtime.Data
{
    [CreateAssetMenu(fileName = "HealthPotion", menuName = "Pixel Worlds/Items/Consumables/HealthPotion", order = 0)]
    public class HealthPotionClass : ConsumableClass
    {
        public int healthToAdd;

        public override void Use(PlayerController caller)
        {
            base.Use(caller);
            caller.health += healthToAdd;
            Debug.Log("test");
        }
    }
}
