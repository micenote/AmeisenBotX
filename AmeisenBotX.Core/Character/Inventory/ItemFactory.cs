﻿using AmeisenBotX.Core.Character.Inventory.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Character.Inventory
{
    public static class ItemFactory
    {
        public static WowBasicItem BuildSpecificItem(WowBasicItem basicItem)
            => (basicItem.Type.ToUpper()) switch
            {
                "ARMOR" => new WowArmor(basicItem),
                "CONSUMEABLE" => new WowConsumable(basicItem),
                "CONTAINER" => new WowContainer(basicItem),
                "GEM" => new WowGem(basicItem),
                "KEY" => new WowKey(basicItem),
                "MISCELLANEOUS" => new WowMiscellaneousItem(basicItem),
                "MONEY" => new WowMoneyItem(basicItem),
                "PROJECTILE" => new WowProjectile(basicItem),
                "QUEST" => new WowQuestItem(basicItem),
                "QUIVER" => new WowQuiver(basicItem),
                "REAGENT" => new WowReagent(basicItem),
                "RECIPE" => new WowRecipe(basicItem),
                "TRADE GOODS" => new WowTradegood(basicItem),
                "WEAPON" => new WowWeapon(basicItem),
                _ => basicItem,
            };

        public static WowBasicItem ParseItem(string json)
        {
            return JsonConvert.DeserializeObject<WowBasicItem>(json);
        }

        public static List<WowBasicItem> ParseItemList(string json)
        {
            return JsonConvert.DeserializeObject<List<WowBasicItem>>(json, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });
        }

        private static void HandleDeserializationError(object sender, ErrorEventArgs e)
        {
            e.ErrorContext.Handled = true;
        }
    }
}