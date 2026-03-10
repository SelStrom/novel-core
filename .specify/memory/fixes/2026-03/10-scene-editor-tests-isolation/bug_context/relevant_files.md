# Relevant Files

## Primary Files (directly affected)

### 1. SceneEditorWindow.cs
- **Path**: `Assets/Scripts/NovelCore/Editor/Windows/SceneEditorWindow.cs`
- **Location**: Lines 609-643
- **Methods**: 
  - `CreateNewDialogueLine()` (line 609)
  - `CreateNewChoice()` (line 627)
- **Current Implementation**: ✅ Correctly uses `AssetDatabase.AddObjectToAsset()`
- **Reason**: Эти методы создают sub-assets для DialogueLineData и ChoiceData

### 2. SceneEditorWindowSubAssetReproductionTests.cs
- **Path**: `Assets/Scripts/NovelCore/Tests/Editor/Windows/SceneEditorWindowSubAssetReproductionTests.cs`
- **Location**: Lines 66-165
- **Issue**: ❌ Тесты написаны как REPRODUCTION tests (должны падать до фикса)
- **Problem**: Код уже исправлен, но тесты всё ещё падают
- **Reason**: Тесты воспроизводят старый баг, но код уже использует правильный API

### 3. SceneEditorWindowSubAssetBreakTests.cs
- **Path**: `Assets/Scripts/NovelCore/Tests/Editor/Windows/SceneEditorWindowSubAssetBreakTests.cs`
- **Issue**: ❌ 2 теста падают (CreateChoice_ShouldAlsoBeSubAsset, CreateMultipleDialogueLines_AllShouldBeSubAssets)
- **Reason**: Эти тесты проверяют АКТУАЛЬНЫЙ код SceneEditorWindow

### 4. SceneEditorWindowSubAssetDeletionReproductionTests.cs
- **Path**: `Assets/Scripts/NovelCore/Tests/Editor/Windows/SceneEditorWindowSubAssetDeletionReproductionTests.cs`  
- **Issue**: ❌ 3 теста падают (удаление sub-assets)
- **Reason**: Тесты для проверки корректного удаления sub-assets

### 5. SceneEditorWindowTests.cs
- **Path**: `Assets/Scripts/NovelCore/Tests/Editor/Windows/SceneEditorWindowTests.cs`
- **Issue**: ❌ 2 теста падают (DrawDialogue тесты)
- **Reason**: UI rendering тесты

## Secondary Files (dependencies)

### 1. SampleProjectGenerator.cs
- **Path**: `Assets/Scripts/NovelCore/Editor/Tools/Generators/SampleProjectGenerator.cs`
- **Reason**: Используется в одном из failing tests (SampleProjectGenerator_CreatesSubAssets_AsExpected)

### 2. DialogueLineData.cs
- **Path**: `Assets/Scripts/NovelCore/Runtime/Data/Dialogue/DialogueLineData.cs`
- **Reason**: Тип данных, создаваемый как sub-asset

### 3. ChoiceData.cs
- **Path**: `Assets/Scripts/NovelCore/Runtime/Data/Choices/ChoiceData.cs`
- **Reason**: Тип данных, создаваемый как sub-asset

## Analysis Summary

**Root Cause Category**: TEST ISSUE - тесты некорректно написаны или устарели

**Evidence**:
1. ✅ `SceneEditorWindow.CreateNewDialogueLine()` уже использует `AssetDatabase.AddObjectToAsset()`
2. ✅ `SceneEditorWindow.CreateNewChoice()` уже использует `AssetDatabase.AddObjectToAsset()`
3. ❌ Reproduction tests воспроизводят старый баг через `AssetDatabase.CreateAsset()` вместо проверки актуального кода
4. ❌ Break tests проверяют актуальный код, но падают по неизвестной причине

**Hypothesis**: Либо тесты устарели после исправления кода, либо тесты вызывают методы SceneEditorWindow неправильно.
