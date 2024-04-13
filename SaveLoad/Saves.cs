using UnityEngine;

namespace CustomSaveLoad
{
    public static class Saves
    {
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