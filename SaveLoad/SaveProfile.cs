using System;

namespace Gamekit2D.Runtime.Utils.SaveLoad
{
    [Serializable]
    public sealed class SaveProfile<T> where T : SaveProfileData
    {
        public string name;
        public T data;

        private SaveProfile()
        {
        } //default constructor - used by JSON converter

        /// <summary>
        /// Creates a new SaveProfile with the given name.
        /// </summary>
        /// <param name="name">string name and filename of the save profile</param>
        /// <param name="data">T data to be saved into this profile</param>
        public SaveProfile(string name, T data)
        {
            this.name = name;
            this.data = data;
        }
    }

    public abstract record SaveProfileData
    {
        /// <summary>
        /// Saves a SaveProfile to the Saves folder in encrypted JSON format
        /// </summary>
        /// <param name="saveName">Name of the profile to be saved. Used as the file name. Refer to the same name when loading</param>
        /// <param name="overwrite">Should data which already exists be overwritten? Previous data will be lost forever!</param>
        /// <param name="encrypt">Optional, whether to encrypt the save file contents or not. Default is false</param>
        // ReSharper disable once UnusedMember.Global
        public async void SaveAsAsync(string saveName, bool overwrite = false, bool encrypt = true)
        {
            var profile = new SaveProfile<SaveProfileData>(saveName,this);
            await SaveManager.SaveAsAsync(profile, overwrite, encrypt);
        }
        
        // ReSharper disable once UnusedMember.Global
        public void SaveAs(string saveName, bool overwrite = false, bool encrypt = true)
        {
            var profile = new SaveProfile<SaveProfileData>(saveName,this);
            SaveManager.SaveAs(profile, overwrite, encrypt);
        }
    }
}