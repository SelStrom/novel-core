using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NovelCore.Runtime.Core.SaveSystem;

namespace NovelCore.Tests.Runtime.Core.SaveSystem
{

/// <summary>
/// Tests for SaveData validation.
/// Validates data-model.md: SaveData entity validation rules.
/// </summary>
[TestFixture]
public class SaveDataValidationTests
{
    [Test]
    public void SaveData_GeneratesSaveId_OnCreation()
    {
        // Act
        var saveData = new SaveData();

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(saveData.saveId), "SaveId should be auto-generated");
        Assert.IsTrue(Guid.TryParse(saveData.saveId, out _), "SaveId should be a valid GUID");
    }

    [Test]
    public void SaveData_SetsSaveTimestamp_OnCreation()
    {
        // Arrange
        DateTime beforeCreation = DateTime.Now;

        // Act
        var saveData = new SaveData();

        // Assert
        DateTime afterCreation = DateTime.Now;
        Assert.IsTrue(saveData.saveTimestamp >= beforeCreation && saveData.saveTimestamp <= afterCreation,
            "SaveTimestamp should be set to current time");
    }

    [Test]
    public void SaveData_InitializesCollections()
    {
        // Act
        var saveData = new SaveData();

        // Assert
        Assert.IsNotNull(saveData.flags, "Flags dictionary should be initialized");
        Assert.IsNotNull(saveData.variables, "Variables dictionary should be initialized");
        Assert.IsNotNull(saveData.navigationState, "NavigationState should be initialized");
        Assert.IsNotNull(saveData.choiceHistory, "ChoiceHistory should be initialized");
        Assert.AreEqual(0, saveData.choiceHistory.Length, "ChoiceHistory should be empty array");
    }

    [Test]
    public void Validate_FailsWithoutSaveVersion()
    {
        // Arrange
        var saveData = new SaveData
        {
            saveVersion = null,
            projectId = "test_project",
            currentSceneId = "test_scene"
        };

        // Expect error log
        LogAssert.Expect(LogType.Error, "SaveData: saveVersion is required");

        // Act
        bool isValid = saveData.Validate();

        // Assert
        Assert.IsFalse(isValid, "Validation should fail without saveVersion");
    }

    [Test]
    public void Validate_FailsWithoutProjectId()
    {
        // Arrange
        var saveData = new SaveData
        {
            saveVersion = "1.2",
            projectId = null,
            currentSceneId = "test_scene"
        };

        // Expect error log
        LogAssert.Expect(LogType.Error, "SaveData: projectId is required");

        // Act
        bool isValid = saveData.Validate();

        // Assert
        Assert.IsFalse(isValid, "Validation should fail without projectId");
    }

    [Test]
    public void Validate_FailsWithoutCurrentSceneId()
    {
        // Arrange
        var saveData = new SaveData
        {
            saveVersion = "1.2",
            projectId = "test_project",
            currentSceneId = null
        };

        // Expect error log
        LogAssert.Expect(LogType.Error, "SaveData: currentSceneId is required");

        // Act
        bool isValid = saveData.Validate();

        // Assert
        Assert.IsFalse(isValid, "Validation should fail without currentSceneId");
    }

    [Test]
    public void Validate_PassesWithAllRequiredFields()
    {
        // Arrange
        var saveData = new SaveData
        {
            saveVersion = "1.2",
            projectId = "test_project",
            currentSceneId = "test_scene"
        };

        // Act
        bool isValid = saveData.Validate();

        // Assert
        Assert.IsTrue(isValid, "Validation should pass with all required fields");
    }

    [Test]
    public void SaveData_SupportsVersioning()
    {
        // Arrange
        var saveData = new SaveData();

        // Assert
        Assert.AreEqual("1.2", saveData.saveVersion, "Current save version should be 1.2");
    }

    [Test]
    public void SaveData_CanStoreGameState()
    {
        // Arrange
        var saveData = new SaveData();
        
        // Act
        saveData.flags.FromDictionary(new System.Collections.Generic.Dictionary<string, bool>
        {
            { "hasKey", true },
            { "metCharacter", false }
        });
        
        saveData.variables.FromDictionary(new System.Collections.Generic.Dictionary<string, int>
        {
            { "health", 100 },
            { "affinity", 75 }
        });

        // Assert
        var flagsDict = saveData.flags.ToDictionary();
        var varsDict = saveData.variables.ToDictionary();
        
        Assert.AreEqual(2, flagsDict.Count);
        Assert.IsTrue(flagsDict["hasKey"]);
        Assert.IsFalse(flagsDict["metCharacter"]);
        
        Assert.AreEqual(2, varsDict.Count);
        Assert.AreEqual(100, varsDict["health"]);
        Assert.AreEqual(75, varsDict["affinity"]);
    }

    [Test]
    public void SaveData_CanStoreChoiceHistory()
    {
        // Arrange
        var saveData = new SaveData
        {
            choiceHistory = new string[] 
            { 
                "choice_1_opt_a", 
                "choice_2_opt_b", 
                "choice_3_opt_c" 
            }
        };

        // Assert
        Assert.AreEqual(3, saveData.choiceHistory.Length);
        Assert.AreEqual("choice_1_opt_a", saveData.choiceHistory[0]);
    }

    [Test]
    public void SaveData_TracksPlaytime()
    {
        // Arrange & Act
        var saveData = new SaveData
        {
            playtimeSeconds = 3665 // 1 hour, 1 minute, 5 seconds
        };

        // Assert
        int hours = saveData.playtimeSeconds / 3600;
        int minutes = (saveData.playtimeSeconds % 3600) / 60;
        
        Assert.AreEqual(1, hours);
        Assert.AreEqual(1, minutes);
    }

    [Test]
    public void SaveData_SupportsScreenshotThumbnail()
    {
        // Arrange
        string fakeBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
        
        // Act
        var saveData = new SaveData
        {
            screenshotThumbnail = fakeBase64
        };

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(saveData.screenshotThumbnail));
    }
}

}
