# Implementation Theory

## Hypothesis

Баг вызван **недостаточной очисткой тестовых assets** в TearDown методах и **некорректной изоляцией тестов**. Тесты оставляют orphaned standalone assets, которые засоряют проект и вызывают провалы последующих тестов.

## Location

### Primary Location

- **File**: `Assets/Scripts/NovelCore/Tests/Editor/Windows/SceneEditorWindowSubAssetBreakTests.cs`
- **Namespace**: `NovelCore.Tests.Editor.Windows`
- **Class**: `SceneEditorWindowSubAssetBreakTests`
- **Method**: `TearDown()` (lines 30-38)
- **Issue**: Очистка удаляет только `_testScenePath`, но не очищает все созданные sub-assets и standalone assets

### Secondary Locations

1. **File**: `Assets/Scripts/NovelCore/Tests/Editor/Windows/SceneEditorWindowSubAssetReproductionTests.cs`
   - **Method**: `TearDown()` (lines 31-63)
   - **Issue**: Недостаточная очистка DialogueLineData и ChoiceData assets

2. **File**: `Assets/Scripts/NovelCore/Tests/Editor/Windows/SceneEditorWindowSubAssetDeletionReproductionTests.cs`
   - **Issue**: Вероятно аналогичная проблема с очисткой

3. **File**: `Assets/Scripts/NovelCore/Tests/Editor/Windows/SceneEditorWindowTests.cs`
   - **Issue**: DrawDialogue тесты также могут создавать assets без очистки

## Root Cause Hypothesis

### Category

- [x] **Test isolation issue** (tests не изолированы, влияют друг на друга)
- [x] **Resource cleanup** (incomplete teardown)
- [ ] Logical error
- [ ] Edge case not handled
- [ ] Race condition
- [ ] State management error
- [ ] API misuse

### Detailed Explanation

#### Проблема 1: FindAssets находит assets от предыдущих тестов

**Код проблемы** (SceneEditorWindowSubAssetBreakTests.cs:172):
```csharp
string[] standaloneChoices = AssetDatabase.FindAssets("Choice t:ChoiceData", new[] { directory });
Assert.That(standaloneChoices.Length, Is.EqualTo(0),
    "No standalone ChoiceData assets should exist");
```

**Issue**: Тест ожидает 0 standalone assets, но находит 2. Это значит:
1. Либо предыдущие тесты не удалили свои assets
2. Либо в директории "Assets/" уже есть ChoiceData от других тестов
3. Либо TearDown недостаточно эффективен

#### Проблема 2: TearDown не универсальный

**Текущий TearDown** (SceneEditorWindowSubAssetBreakTests.cs:30-38):
```csharp
[TearDown]
public void TearDown()
{
    if (!string.IsNullOrEmpty(_testScenePath) && File.Exists(_testScenePath))
    {
        AssetDatabase.DeleteAsset(_testScenePath);
    }
    AssetDatabase.SaveAssets();
}
```

**Problem**: 
- Удаляет только `_testScenePath`
- НЕ ищет orphaned standalone DialogueLineData/ChoiceData
- НЕ очищает sub-assets явно (хотя они должны удаляться с parent)
- НЕ обновляет AssetDatabase после удаления

### Call Stack Analysis

```
Test Execution
  ↓
SetUp() → создаёт TestScene.asset
  ↓
Test Body → создаёт DialogueLineData/ChoiceData через AddObjectToAsset
  ↓
TearDown() → удаляет только _testScenePath
  ↓
AssetDatabase caching → orphaned assets остаются в индексе
  ↓
Next Test → FindAssets находит orphaned assets
  ↓
Assertion fails
```

## Git History Insights

Не проверялось (тесты новые, баг сразу обнаружен).

## Edge Cases Analysis

### Edge Case 1: AssetDatabase caching
- **Current handling**: TearDown вызывает `AssetDatabase.SaveAssets()` но не `AssetDatabase.Refresh()`
- **Problem**: Asset индекс может содержать устаревшие ссылки

### Edge Case 2: Test execution order
- **Current handling**: NUnit может запускать тесты в произвольном порядке
- **Problem**: Тест A создаёт assets → Тест B находит их

### Edge Case 3: Parallel test execution
- **Current handling**: Нет защиты от параллельного выполнения
- **Problem**: Несколько тестов могут создавать assets в одной директории одновременно

## Confidence Score

**95%**

### Reasoning

1. **Error message явный**: "Expected: 0 But was: 2" - находит standalone assets
2. **Тест изоляция нарушена**: Tests не очищают после себя
3. **Reproducible**: Тесты падают детерминированно
4. **Simple fix**: Улучшить TearDown для более тщательной очистки

## Recommended Fix Approach

### Fix 1: Улучшить TearDown с полной очисткой

```csharp
[TearDown]
public void TearDown()
{
    // Delete main test scene
    if (!string.IsNullOrEmpty(_testScenePath) && File.Exists(_testScenePath))
    {
        AssetDatabase.DeleteAsset(_testScenePath);
    }

    // Clean up ALL DialogueLineData and ChoiceData in Assets/ directory
    CleanupOrphanedTestAssets();

    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();  // Force refresh to clear cache
}

private void CleanupOrphanedTestAssets()
{
    // Find and delete ALL standalone DialogueLineData in Assets/
    string[] dialogueAssets = AssetDatabase.FindAssets("t:DialogueLineData", new[] { "Assets" });
    foreach (string guid in dialogueAssets)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        // Delete if it's a standalone asset (not sub-asset)
        var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
        if (mainAsset is DialogueLineData && path.Contains("Test"))
        {
            AssetDatabase.DeleteAsset(path);
        }
    }

    // Same for ChoiceData
    string[] choiceAssets = AssetDatabase.FindAssets("t:ChoiceData", new[] { "Assets" });
    foreach (string guid in choiceAssets)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
        if (mainAsset is ChoiceData && path.Contains("Test"))
        {
            AssetDatabase.DeleteAsset(path);
        }
    }
}
```

### Fix 2: Использовать уникальные test директории

```csharp
[SetUp]
public void SetUp()
{
    // Create unique test directory for this test run
    string testDir = $"Assets/TestAssets_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
    Directory.CreateDirectory(testDir);
    
    _testScenePath = $"{testDir}/TestScene.asset";
    _testScene = ScriptableObject.CreateInstance<SceneData>();
    AssetDatabase.CreateAsset(_testScene, _testScenePath);
    AssetDatabase.SaveAssets();
}

[TearDown]
public void TearDown()
{
    // Delete entire test directory
    string testDir = Path.GetDirectoryName(_testScenePath);
    if (Directory.Exists(testDir))
    {
        AssetDatabase.DeleteAsset(testDir);
    }
    AssetDatabase.Refresh();
}
```

## Preferred Fix

**Fix 1** (улучшенная очистка) - минимальные изменения, решает проблему напрямую.
