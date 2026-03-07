using NUnit.Framework;
using UnityEngine;
using SaveSystemImpl = NovelCore.Runtime.Core.SaveSystem.SaveSystem;
using NovelCore.Runtime.Core.SaveSystem;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.TestTools;

namespace NovelCore.Tests.Runtime.Core.SaveSystem
{

/// <summary>
/// Unit tests for SaveSystem save/load operations.
/// </summary>
public class SaveSystemTests : BaseTestFixture
{
    private SaveSystemImpl _saveSystem;
    private string _testSaveDirectory;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _testSaveDirectory = Path.Combine(Application.persistentDataPath, "TestSaves");
        
        if (Directory.Exists(_testSaveDirectory))
        {
            Directory.Delete(_testSaveDirectory, true);
        }
        
        Directory.CreateDirectory(_testSaveDirectory);

        _saveSystem = new SaveSystemImpl();
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();

        if (Directory.Exists(_testSaveDirectory))
        {
            try
            {
                Directory.Delete(_testSaveDirectory, true);
            }
            catch (Exception)
            {
            }
        }
    }

    [Test]
    public async Task SaveSystem_SaveAsync_WithValidData_SavesSuccessfully()
    {
        var saveData = new SaveData
        {
            currentSceneId = "scene_001",
            currentDialogueIndex = 5,
            choiceHistory = new[] { "choice_1_opt_a", "choice_2_opt_b" },
            playtimeSeconds = 3600
        };

        bool result = await _saveSystem.SaveAsync("slot1", saveData);

        Assert.IsTrue(result);
        Assert.IsTrue(_saveSystem.SaveExists("slot1"));
    }

    [Test]
    public async Task SaveSystem_SaveAsync_WithNullSlotId_ReturnsFalse()
    {
        var saveData = new SaveData();
        
        LogAssert.Expect(LogType.Error, "SaveSystem: Invalid slot ID");
        bool result = await _saveSystem.SaveAsync(null, saveData);

        Assert.IsFalse(result);
    }

    [Test]
    public async Task SaveSystem_SaveAsync_WithNullData_ReturnsFalse()
    {
        LogAssert.Expect(LogType.Error, "SaveSystem: Cannot save null data");
        bool result = await _saveSystem.SaveAsync("slot1", null);

        Assert.IsFalse(result);
    }

    [Test]
    public async Task SaveSystem_LoadAsync_WithExistingSave_LoadsSuccessfully()
    {
        var originalData = new SaveData
        {
            currentSceneId = "scene_001",
            currentDialogueIndex = 5,
            choiceHistory = new[] { "choice_1_opt_a" },
            playtimeSeconds = 1800
        };

        await _saveSystem.SaveAsync("slot1", originalData);

        SaveData loadedData = await _saveSystem.LoadAsync("slot1");

        Assert.IsNotNull(loadedData);
        Assert.AreEqual("scene_001", loadedData.currentSceneId);
        Assert.AreEqual(5, loadedData.currentDialogueIndex);
        Assert.AreEqual(1, loadedData.choiceHistory.Length);
        Assert.AreEqual("choice_1_opt_a", loadedData.choiceHistory[0]);
        Assert.AreEqual(1800, loadedData.playtimeSeconds);
    }

    [Test]
    public async Task SaveSystem_LoadAsync_WithNonExistentSave_ReturnsNull()
    {
        LogAssert.Expect(LogType.Warning, "SaveSystem: No save found in slot 'nonexistent'");
        SaveData loadedData = await _saveSystem.LoadAsync("nonexistent");

        Assert.IsNull(loadedData);
    }

    [Test]
    public async Task SaveSystem_LoadAsync_WithNullSlotId_ReturnsNull()
    {
        LogAssert.Expect(LogType.Error, "SaveSystem: Invalid slot ID");
        SaveData loadedData = await _saveSystem.LoadAsync(null);

        Assert.IsNull(loadedData);
    }

    [Test]
    public async Task SaveSystem_DeleteSave_WithExistingSave_DeletesSuccessfully()
    {
        var saveData = new SaveData { currentSceneId = "scene_001" };
        await _saveSystem.SaveAsync("slot1", saveData);

        Assert.IsTrue(_saveSystem.SaveExists("slot1"));

        bool result = _saveSystem.DeleteSave("slot1");

        Assert.IsTrue(result);
        Assert.IsFalse(_saveSystem.SaveExists("slot1"));
    }

    [Test]
    public void SaveSystem_DeleteSave_WithNonExistentSave_ReturnsFalse()
    {
        LogAssert.Expect(LogType.Warning, "SaveSystem: No save found in slot 'nonexistent' to delete");
        bool result = _saveSystem.DeleteSave("nonexistent");

        Assert.IsFalse(result);
    }

    [Test]
    public void SaveSystem_DeleteSave_WithNullSlotId_ReturnsFalse()
    {
        LogAssert.Expect(LogType.Error, "SaveSystem: Invalid slot ID");
        bool result = _saveSystem.DeleteSave(null);

        Assert.IsFalse(result);
    }

    [Test]
    public async Task SaveSystem_GetAllSaveSlots_WithMultipleSaves_ReturnsAllSlots()
    {
        var saveData1 = new SaveData { currentSceneId = "scene_001", playtimeSeconds = 100 };
        var saveData2 = new SaveData { currentSceneId = "scene_002", playtimeSeconds = 200 };
        var saveData3 = new SaveData { currentSceneId = "scene_003", playtimeSeconds = 300 };

        await _saveSystem.SaveAsync("slot1", saveData1);
        await Task.Delay(100);
        await _saveSystem.SaveAsync("slot2", saveData2);
        await Task.Delay(100);
        await _saveSystem.SaveAsync("slot3", saveData3);

        SaveSlotInfo[] slots = _saveSystem.GetAllSaveSlots();

        Assert.AreEqual(3, slots.Length);
        Assert.AreEqual("slot3", slots[0].slotId);
        Assert.AreEqual("slot2", slots[1].slotId);
        Assert.AreEqual("slot1", slots[2].slotId);
    }

    [Test]
    public async Task SaveSystem_GetAllSaveSlots_WithNoSaves_ReturnsEmptyArray()
    {
        SaveSlotInfo[] slots = _saveSystem.GetAllSaveSlots();

        Assert.IsNotNull(slots);
        Assert.AreEqual(0, slots.Length);
    }

    [Test]
    public async Task SaveSystem_SaveExists_WithExistingSave_ReturnsTrue()
    {
        var saveData = new SaveData { currentSceneId = "scene_001" };
        await _saveSystem.SaveAsync("slot1", saveData);

        bool exists = _saveSystem.SaveExists("slot1");

        Assert.IsTrue(exists);
    }

    [Test]
    public void SaveSystem_SaveExists_WithNonExistentSave_ReturnsFalse()
    {
        bool exists = _saveSystem.SaveExists("nonexistent");

        Assert.IsFalse(exists);
    }

    [Test]
    public async Task SaveSystem_GetSaveSlotInfo_WithExistingSave_ReturnsCorrectInfo()
    {
        var saveData = new SaveData
        {
            currentSceneId = "scene_001",
            playtimeSeconds = 3600,
            saveSlotName = "Test Save"
        };

        await _saveSystem.SaveAsync("slot1", saveData);

        SaveSlotInfo slotInfo = _saveSystem.GetSaveSlotInfo("slot1");

        Assert.IsNotNull(slotInfo);
        Assert.AreEqual("slot1", slotInfo.slotId);
        Assert.AreEqual("Test Save", slotInfo.slotName);
        Assert.AreEqual("scene_001", slotInfo.currentSceneId);
        Assert.AreEqual(3600, slotInfo.playtimeSeconds);
        Assert.IsFalse(slotInfo.isEmpty);
    }

    [Test]
    public void SaveSystem_GetSaveSlotInfo_WithNonExistentSave_ReturnsEmptySlotInfo()
    {
        SaveSlotInfo slotInfo = _saveSystem.GetSaveSlotInfo("emptyslot");

        Assert.IsNotNull(slotInfo);
        Assert.AreEqual("emptyslot", slotInfo.slotId);
        Assert.IsTrue(slotInfo.isEmpty);
    }

    [Test]
    public async Task SaveSystem_AutoSaveAsync_SavesSuccessfully()
    {
        var saveData = new SaveData
        {
            currentSceneId = "scene_001",
            playtimeSeconds = 1200
        };

        bool result = await _saveSystem.AutoSaveAsync(saveData);

        Assert.IsTrue(result);
        Assert.IsTrue(_saveSystem.SaveExists("autosave"));
    }

    [Test]
    public async Task SaveSystem_LoadAutoSaveAsync_LoadsSuccessfully()
    {
        var saveData = new SaveData
        {
            currentSceneId = "scene_001",
            playtimeSeconds = 1200
        };

        await _saveSystem.AutoSaveAsync(saveData);

        SaveData loadedData = await _saveSystem.LoadAutoSaveAsync();

        Assert.IsNotNull(loadedData);
        Assert.AreEqual("scene_001", loadedData.currentSceneId);
        Assert.AreEqual(1200, loadedData.playtimeSeconds);
    }

    [Test]
    public void SaveSystem_CreateSaveSnapshot_CreatesCorrectSnapshot()
    {
        var choiceHistory = new List<string> { "choice_1", "choice_2" };

        SaveData snapshot = SaveSystemImpl.CreateSaveSnapshot(
            "scene_005",
            3,
            choiceHistory,
            7200
        );

        Assert.IsNotNull(snapshot);
        Assert.AreEqual("scene_005", snapshot.currentSceneId);
        Assert.AreEqual(3, snapshot.currentDialogueIndex);
        Assert.AreEqual(2, snapshot.choiceHistory.Length);
        Assert.AreEqual("choice_1", snapshot.choiceHistory[0]);
        Assert.AreEqual("choice_2", snapshot.choiceHistory[1]);
        Assert.AreEqual(7200, snapshot.playtimeSeconds);
    }

    [Test]
    public async Task SaveSystem_SaveAndLoad_PreservesTimestamp()
    {
        var originalData = new SaveData
        {
            currentSceneId = "scene_001"
        };

        await _saveSystem.SaveAsync("slot1", originalData);

        SaveData loadedData = await _saveSystem.LoadAsync("slot1");

        Assert.IsNotNull(loadedData);
        Assert.AreNotEqual(DateTime.MinValue, loadedData.saveTimestamp);
    }
}

}
