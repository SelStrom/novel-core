using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace NovelCore.Runtime.Core.SaveSystem
{
    /// <summary>
    /// Implementation of save system using JSON serialization.
    /// Saves to persistent data path with platform-specific cloud sync support.
    /// </summary>
    public class SaveSystem : ISaveSystem
    {
        private const string SaveFileExtension = ".save";
        private const string AutoSaveSlotId = "autosave";
        private readonly string _saveDirectory;
        
        public SaveSystem() : this(null)
        {
        }
        
        public SaveSystem(string customSaveDirectory)
        {
            _saveDirectory = string.IsNullOrEmpty(customSaveDirectory) 
                ? Path.Combine(Application.persistentDataPath, "Saves")
                : customSaveDirectory;
            
            // Ensure save directory exists
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
                Debug.Log($"SaveSystem: Created save directory at {_saveDirectory}");
            }
        }
        
        public async Task<bool> SaveAsync(string slotId, SaveData saveData)
        {
            if (string.IsNullOrEmpty(slotId))
            {
                Debug.LogError("SaveSystem: Invalid slot ID");
                return false;
            }
            
            if (saveData == null)
            {
                Debug.LogError("SaveSystem: Cannot save null data");
                return false;
            }
            
            try
            {
                // Update save metadata
                saveData.saveSlotId = slotId;
                saveData.saveTimestamp = DateTime.Now;
                
                // Serialize to JSON
                string json = JsonUtility.ToJson(saveData, true);
                
                // Write to file
                string filePath = GetSaveFilePath(slotId);
                await File.WriteAllTextAsync(filePath, json);
                
                Debug.Log($"SaveSystem: Saved to slot '{slotId}' at {filePath}");
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveSystem: Failed to save to slot '{slotId}': {ex.Message}");
                return false;
            }
        }
        
        public async Task<SaveData> LoadAsync(string slotId)
        {
            if (string.IsNullOrEmpty(slotId))
            {
                Debug.LogError("SaveSystem: Invalid slot ID");
                return null;
            }
            
            string filePath = GetSaveFilePath(slotId);
            
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"SaveSystem: No save found in slot '{slotId}'");
                return null;
            }
            
            try
            {
                // Read file
                string json = await File.ReadAllTextAsync(filePath);
                
                // Deserialize from JSON
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
                if (saveData == null)
                {
                    Debug.LogError($"SaveSystem: Failed to deserialize save data from slot '{slotId}'");
                    return null;
                }
                
                Debug.Log($"SaveSystem: Loaded from slot '{slotId}'");
                
                return saveData;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveSystem: Failed to load from slot '{slotId}': {ex.Message}");
                return null;
            }
        }
        
        public bool DeleteSave(string slotId)
        {
            if (string.IsNullOrEmpty(slotId))
            {
                Debug.LogError("SaveSystem: Invalid slot ID");
                return false;
            }
            
            string filePath = GetSaveFilePath(slotId);
            
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"SaveSystem: No save found in slot '{slotId}' to delete");
                return false;
            }
            
            try
            {
                File.Delete(filePath);
                Debug.Log($"SaveSystem: Deleted save from slot '{slotId}'");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveSystem: Failed to delete save from slot '{slotId}': {ex.Message}");
                return false;
            }
        }
        
        public SaveSlotInfo[] GetAllSaveSlots()
        {
            try
            {
                // Get all save files
                string[] saveFiles = Directory.GetFiles(_saveDirectory, $"*{SaveFileExtension}");
                
                List<SaveSlotInfo> slots = new List<SaveSlotInfo>();
                
                foreach (string filePath in saveFiles)
                {
                    string slotId = Path.GetFileNameWithoutExtension(filePath);
                    SaveSlotInfo info = GetSaveSlotInfo(slotId);
                    
                    if (info != null)
                    {
                        slots.Add(info);
                    }
                }
                
                // Sort by timestamp (newest first)
                return slots.OrderByDescending(s => s.saveTimestamp).ToArray();
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveSystem: Failed to get save slots: {ex.Message}");
                return Array.Empty<SaveSlotInfo>();
            }
        }
        
        public bool SaveExists(string slotId)
        {
            if (string.IsNullOrEmpty(slotId))
                return false;
            
            return File.Exists(GetSaveFilePath(slotId));
        }
        
        public SaveSlotInfo GetSaveSlotInfo(string slotId)
        {
            if (string.IsNullOrEmpty(slotId))
                return null;
            
            string filePath = GetSaveFilePath(slotId);
            
            if (!File.Exists(filePath))
            {
                // Return empty slot info
                return new SaveSlotInfo
                {
                    slotId = slotId,
                    slotName = $"Empty Slot {slotId}",
                    isEmpty = true,
                    saveTimestamp = DateTime.MinValue
                };
            }
            
            try
            {
                // Read and parse save data for metadata
                string json = File.ReadAllText(filePath);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
                if (saveData == null)
                    return null;
                
                return new SaveSlotInfo
                {
                    slotId = slotId,
                    slotName = saveData.saveSlotName ?? $"Save {slotId}",
                    saveTimestamp = saveData.saveTimestamp,
                    currentSceneId = saveData.currentSceneId,
                    playtimeSeconds = saveData.playtimeSeconds,
                    isEmpty = false
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveSystem: Failed to get info for slot '{slotId}': {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Performs an auto-save to the dedicated auto-save slot.
        /// </summary>
        public async Task<bool> AutoSaveAsync(SaveData saveData)
        {
            saveData.saveSlotName = "Auto Save";
            return await SaveAsync(AutoSaveSlotId, saveData);
        }
        
        /// <summary>
        /// Loads the auto-save if it exists.
        /// </summary>
        public async Task<SaveData> LoadAutoSaveAsync()
        {
            return await LoadAsync(AutoSaveSlotId);
        }
        
        /// <summary>
        /// Gets the full file path for a save slot.
        /// </summary>
        private string GetSaveFilePath(string slotId)
        {
            return Path.Combine(_saveDirectory, $"{slotId}{SaveFileExtension}");
        }
        
        /// <summary>
        /// Creates a save data snapshot from current game state.
        /// </summary>
        public static SaveData CreateSaveSnapshot(string currentSceneId, int dialogueIndex, List<string> choiceHistory, int playtimeSeconds)
        {
            SaveData saveData = new SaveData
            {
                currentSceneId = currentSceneId,
                currentDialogueIndex = dialogueIndex,
                choiceHistory = choiceHistory?.ToArray() ?? Array.Empty<string>(),
                playtimeSeconds = playtimeSeconds
            };
            
            return saveData;
        }
    }
}
