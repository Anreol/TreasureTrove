﻿using UnityEngine.Networking;

namespace TreasureTrove.Components
{
    public class WeaveTest : NetworkBehaviour
    {
        [SyncVar]
        public int int1 = 66;

        [SyncVar]
        public int int2 = 23487;

        [SyncVar]
        public string MyString = "esfdsagsdfgsdgdsfg";
    }
}