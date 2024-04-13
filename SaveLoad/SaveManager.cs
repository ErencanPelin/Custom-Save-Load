using System.IO;
using System.Threading.Tasks;
using Gamekit2D.Runtime.Utils.Progress;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Gamekit2D.Runtime.Utils.SaveLoad
{
    [UsedImplicitly]
    public static class SaveManager
    {
        private static readonly string saveFolder = Application.persistentDataPath + "/gameData"; 
        private const string key = "b14ca5898a4e4133bbce2ea2315a1916"; //private key used for AES encryption

        /// <summary>
        /// Deletes a given save from the saves folder. This method is asynchronous, and the game will continue playing as the file is being deleted in the background
        /// </summary>
        /// <param name="profileName">the name of the save file being deleted</param>
        // ReSharper disable once UnusedMember.Global
        public static async Task DeleteSaveAsync(string profileName)
        {
            LoadingManager.Show(0, 2);
            await Task.Run(() =>
            {
                //check if the file exists
                if (!File.Exists($"{saveFolder}/{profileName}"))
                {
                    Task.FromException(new Exception(
                        $"Save file: {profileName} not found. Make sure you've saved the file before accessing it, and the profileName is correct."));
                }
                LoadingManager.Report(1);
                //remove the file
                File.Delete($"{saveFolder}/{profileName}");
                LoadingManager.Report(2);
            });
            LoadingManager.Hide();
        }

        /// <summary>
        /// Deletes a given save from the saves folder. This method is NOT asynchronous, and may halt the main game loop while the file is being deleted.
        /// </summary>
        /// <param name="profileName">the name of the save file being deleted</param>
        // ReSharper disable once UnusedMember.Global
        public static void DeleteSave(string profileName)
        {
            //check if the file exists
            if (!File.Exists($"{saveFolder}/{profileName}"))
            {
                throw new Exception(
                    $"Save file: {profileName} not found. Make sure you've saved the file before accessing it, and the profileName is correct.");
            }

            //remove the file
            File.Delete($"{saveFolder}/{profileName}");
        }

        /// <summary>
        /// Loads and returns a SaveProfile from the Saves Folder. Uses the given profile name to retrieve the file.
        /// The file is automatically decrypted and turned into a SaveProfile object of the given type. This method is asynchronous and should be awaited for its Result.
        /// </summary>
        /// <param name="profileName">The name of the profile being loaded</param>
        /// <param name="encryptionEnabled">Optional, whether or not to use decryption on the save file. Set to true if the file being loaded is encrypted</param>
        /// <returns>new SaveProfile Object</returns>
        // ReSharper disable once UnusedMember.Global
        public static async Task<SaveProfile<T>> LoadAsAsync<T>(string profileName, bool encryptionEnabled = true)
            where T : SaveProfileData
        {
            LoadingManager.Show(0, 3);
            var output = await Task.Run(() =>
            {
                //check if file exists
                if (!File.Exists($"{saveFolder}/{profileName}"))
                    Task.FromException(
                        new Exception(
                            $"Save file: {profileName} not found. Make sure you've saved the file before accessing it, and the profileName is correct."));
                LoadingManager.Report(1);
                // Read the entire file and save its contents.
                var fileContents = File.ReadAllText($"{saveFolder}/{profileName}"); //encrypted
                LoadingManager.Report(2);
                if (encryptionEnabled)
                    fileContents = AesOperation.DecryptString(key, fileContents); //decrypted
                LoadingManager.Report(3);
                // Deserialize the JSON data 
                Debug.Log("Successfully loaded data");
                return JsonConvert.DeserializeObject<SaveProfile<T>>(fileContents);
            });
            LoadingManager.Hide();
            return output;
        }

        /// <summary>
        /// Loads and returns a SaveProfile from the Saves Folder. Uses the given profile name to retrieve the file.
        /// The file is automatically decrypted and turned into a SaveProfile object of the given type. This method is asynchronous and should be awaited for its Result.
        /// </summary>
        /// <param name="profileName">The name of the profile being loaded</param>
        /// <param name="encryptionEnabled">Optional, whether or not to use decryption on the save file. Set to true if the file being loaded is encrypted</param>
        /// <returns>new SaveProfile Object</returns>
        // ReSharper disable once UnusedMember.Global
        public static SaveProfile<T> LoadAs<T>(string profileName, bool encryptionEnabled = true)
            where T : SaveProfileData
        {
            //check if file exists
            if (!File.Exists($"{saveFolder}/{profileName}"))
                throw new Exception(
                    $"Save file: {profileName} not found. Make sure you've saved the file before accessing it, and the profileName is correct.");
            // Read the entire file and save its contents.
            var fileContents = File.ReadAllText($"{saveFolder}/{profileName}"); //encrypted
            if (encryptionEnabled)
                fileContents = AesOperation.DecryptString(key, fileContents); //decrypted
            // Deserialize the JSON data 
#if UNITY_EDITOR
            Debug.Log("Successfully loaded data");
#endif
            return JsonConvert.DeserializeObject<SaveProfile<T>>(fileContents);
        }

        /// <summary>
        /// Saves a SaveProfile to the Saves folder in encrypted JSON format
        /// </summary>
        /// <param name="save">SaveProfile being saved</param>
        /// <param name="overwrite">Should data which already exists be overwritten? Previous data will be lost forever!</param>
        /// <param name="encryptionEnabled">Optional, whether to encrypt the save file contents or not. Default is false</param>
        internal static async Task SaveAsAsync<T>(SaveProfile<T> save, bool overwrite = false, bool encryptionEnabled = true)
            where T : SaveProfileData
        {
            LoadingManager.Show(0, 6);
            await Task.Run(() =>
            {
                try
                {
                    LoadingManager.Report(1);
                    if (!overwrite && File.Exists($"{saveFolder}/{save.name}"))
                        throw new Exception(
                            $"Save file: {save.name} already exists, please use a different save profile name.");
                    LoadingManager.Report(2);
                    //Serialize the object into JSON and save string.
                    var jsonString = JsonConvert.SerializeObject(save, Formatting.Indented,
                        new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    LoadingManager.Report(3);
                    //encrypt the plain text string using Aes and our symmetric key
                    if (encryptionEnabled)
                        jsonString = AesOperation.EncryptString(key, jsonString);
                    LoadingManager.Report(4);
                    // Write JSON to file.
                    if (!Directory.Exists(saveFolder)) //create the saves folder if we don't already have it!
                        Directory.CreateDirectory(saveFolder);
                    LoadingManager.Report(5);
                    //write the encrypted text into the file
                    File.WriteAllText($"{saveFolder}/{save.name}", jsonString);
                    LoadingManager.Report(6);
#if UNITY_EDITOR
                    Debug.Log($"Successfully saved data into: {saveFolder}/{save.name}");
#endif
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    Debug.LogError($"Failed to save data into {save.name}. Full stack trace:\n{e}");
#endif
                }
            });
            LoadingManager.Hide();
        }

        /// <summary>
        /// Saves a SaveProfile to the Saves folder in encrypted JSON format
        /// </summary>
        /// <param name="save">SaveProfile being saved</param>
        /// <param name="overwrite">Should data which already exists be overwritten? Previous data will be lost forever!</param>
        /// <param name="encryptionEnabled">Optional, whether to encrypt the save file contents or not. Default is false</param>
        internal static void SaveAs<T>(SaveProfile<T> save, bool overwrite = false,
            bool encryptionEnabled = true)
            where T : SaveProfileData
        {
            try
            {
                if (!overwrite && File.Exists($"{saveFolder}/{save.name}"))
                    throw new Exception(
                        $"Save file: {save.name} already exists, please use a different save profile name.");
                //Serialize the object into JSON and save string.
                var jsonString = JsonConvert.SerializeObject(save, Formatting.Indented,
                    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                //encrypt the plain text string using Aes and our symmetric key
                if (encryptionEnabled)
                    jsonString = AesOperation.EncryptString(key, jsonString);
                // Write JSON to file.
                if (!Directory.Exists(saveFolder)) //create the saves folder if we don't already have it!
                    Directory.CreateDirectory(saveFolder);
                //write the encrypted text into the file
                File.WriteAllText($"{saveFolder}/{save.name}", jsonString);
#if UNITY_EDITOR
                Debug.Log($"Successfully saved data into: {saveFolder}/{save.name}");
#endif
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError($"Failed to save data into {save.name}. Full stack trace:\n{e}");
#endif
            }
        }
    }
}