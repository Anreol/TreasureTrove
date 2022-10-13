using HG;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace TreasureTrove.Equipments
{
    internal class Hellchain : Equipment
    {
        public override EquipmentDef equipmentDef { get; set; } = Assets.mainAssetBundle.LoadAsset<EquipmentDef>("HellChain");

        public override bool FireAction(EquipmentSlot slot)
        {
            return false;
        }
    }
}