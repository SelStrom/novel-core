using System;
using System.Threading.Tasks;
using UnityEngine;

namespace NovelCore.Runtime.Core.SaveSystem
{
    /// <summary>
    /// Interface for save/load functionality.
    /// </summary>
    public interface ISaveSystem
    {
        /// <summary>
        /// Saves game state to the specified slot.
        /// </summary>
        /// <param name="slotId">Save slot identifier</param>
        /// <param name="saveData">Data to save</param>
        /// <returns>True if save was successful</returns>
        Task<bool> SaveAsync(string slotId, SaveData saveData);
        
        /// <summary>
        /// Loads game state from the specified slot.
        /// </summary>
        /// <param name="slotId">Save slot identifier</param>
        /// <returns>Loaded save data, or null if slot doesn't exist</returns>
        Task<SaveData> LoadAsync(string slotId);
        
        /// <summary>
        /// Deletes the save in the specified slot.
        /// </summary>
        /// <param name="slotId">Save slot identifier</param>
        /// <returns>True if delete was successful</returns>
        bool DeleteSave(string slotId);
        
        /// <summary>
        /// Gets all available save slots.
        /// </summary>
        /// <returns>Array of save slot metadata</returns>
        SaveSlotInfo[] GetAllSaveSlots();
        
        /// <summary>
        /// Checks if a save exists in the specified slot.
        /// </summary>
        /// <param name="slotId">Save slot identifier</param>
        bool SaveExists(string slotId);
        
        /// <summary>
        /// Gets metadata for a specific save slot.
        /// </summary>
        /// <param name="slotId">Save slot identifier</param>
        SaveSlotInfo GetSaveSlotInfo(string slotId);
    }
    
    /// <summary>
    /// Data structure for game save state.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public string saveVersion = "1.0";
        public string currentSceneId;
        public int currentDialogueIndex;
        public string[] choiceHistory;
        public DateTime saveTimestamp;
        public string saveSlotId;
        public string saveSlotName;
        
        // Player progress
        public int playtimeSeconds;
        public SerializableDictionary<string, bool> flags;
        public SerializableDictionary<string, int> variables;
        
        public SaveData()
        {
            saveTimestamp = DateTime.Now;
            flags = new SerializableDictionary<string, bool>();
            variables = new SerializableDictionary<string, int>();
        }
    }
    
    /// <summary>
    /// Metadata about a save slot.
    /// </summary>
    [Serializable]
    public class SaveSlotInfo
    {
        public string slotId;
        public string slotName;
        public DateTime saveTimestamp;
        public string currentSceneId;
        public int playtimeSeconds;
        public bool isEmpty;
        
        public string GetFormattedTimestamp()
        {
            return saveTimestamp.ToString("yyyy-MM-dd HH:mm");
        }
        
        public string GetFormattedPlaytime()
        {
            int hours = playtimeSeconds / 3600;
            int minutes = (playtimeSeconds % 3600) / 60;
            return $"{hours}h {minutes}m";
        }
    }
    
    /// <summary>
    /// Simple serializable dictionary for save data.
    /// </summary>
    [Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        [SerializeField] private TKey[] keys = Array.Empty<TKey>();
        [SerializeField] private TValue[] values = Array.Empty<TValue>();
        
        private System.Collections.Generic.Dictionary<TKey, TValue> _dictionary;
        
        public System.Collections.Generic.Dictionary<TKey, TValue> ToDictionary()
        {
            if (_dictionary == null)
            {
                _dictionary = new System.Collections.Generic.Dictionary<TKey, TValue>();
                
                for (int i = 0; i < keys.Length; i++)
                {
                    if (i < values.Length)
                    {
                        _dictionary[keys[i]] = values[i];
                    }
                }
            }
            
            return _dictionary;
        }
        
        public void FromDictionary(System.Collections.Generic.Dictionary<TKey, TValue> dict)
        {
            _dictionary = dict;
            keys = new TKey[dict.Count];
            values = new TValue[dict.Count];
            
            int index = 0;
            foreach (var kvp in dict)
            {
                keys[index] = kvp.Key;
                values[index] = kvp.Value;
                index++;
            }
        }
    }
}
