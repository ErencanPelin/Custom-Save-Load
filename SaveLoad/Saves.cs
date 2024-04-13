using Gamekit2D.Runtime.Items;
using UnityEngine;

namespace Gamekit2D.Runtime.Utils.SaveLoad
{
    public static class Saves
    {
        /// <summary>
        /// Standard Inventory save data. Stores a <see cref="Inventory"/> object. You can inherit this record
        /// to add more fields to save additional custom data
        /// </summary>
        [System.Serializable]
        public record InventorySaveData : SaveProfileData
        {
            public Inventory inventoryData;
        }
        
        /// <summary>
        /// Standard player save data. You may inherit this record to add additional fields
        /// to save custom data
        /// </summary>
        [System.Serializable]
        public record PlayerSaveData : SaveProfileData
        {
            public Vector3 position;
            public int currentLevel;
            public float health;
            public float xp;
        }
    }
}