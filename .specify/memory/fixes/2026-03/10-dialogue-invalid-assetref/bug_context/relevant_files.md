# Relevant Files

## Primary Files (directly affected)

### 1. `Assets/Scripts/NovelCore/Runtime/Core/DialogueSystem/DialogueSystem.cs`
- **Location**: namespace `NovelCore.Runtime.Core.DialogueSystem`, class `DialogueSystem`, method `CompleteDialogue()`
- **Lines**: 338-380
- **Reason**: 
  - Содержит логику определения следующей сцены
  - Не проверяет валидность AssetReference перед загрузкой
  - Вызывает LoadAssetAsync с невалидным ключом
  - Должен обрабатывать случай "нет следующей сцены" (end of story)

### 2. `Assets/Scripts/NovelCore/Runtime/Core/AssetManagement/AddressablesAssetManager.cs`
- **Location**: namespace `NovelCore.Runtime.Core.AssetManagement`, class `AddressablesAssetManager`, method `LoadAssetAsync<T>()`
- **Lines**: 35-70
- **Reason**:
  - Принимает object key без валидации
  - Не проверяет RuntimeKeyIsValid() для AssetReference
  - Передает невалидный ключ в Addressables.LoadAssetAsync, что вызывает InvalidKeyException

## Secondary Files (dependencies)

### 3. `Assets/Scripts/NovelCore/Runtime/Data/Scenes/SceneData.cs`
- **Reason**: 
  - Определяет структуру NextScene (AssetReference)
  - Содержит валидацию SceneData, но не проверяет валидность NextScene AssetReference

### 4. `Assets/Scripts/NovelCore/Runtime/UI/DialogueBox/DialogueBoxController.cs`
- **Location**: namespace `NovelCore.Runtime.UI.DialogueBox`, method `OnDialogueComplete()`
- **Lines**: 159-173
- **Reason**:
  - Слушает событие OnDialogueComplete
  - Скрывает диалоговую панель, но не показывает уведомление о завершении

### 5. `Assets/Scripts/NovelCore/Runtime/UI/UIManager.cs`
- **Reason**:
  - Управляет UI компонентами
  - Потенциальное место для добавления UI-уведомления "End of Story"

### 6. `Assets/Scripts/NovelCore/Runtime/Core/DialogueSystem/IDialogueSystem.cs`
- **Reason**:
  - Определяет event OnDialogueComplete
  - Может потребоваться новый event OnStoryComplete для явного обозначения конца сюжета

## Files to Create (new)

### 7. `Assets/Scripts/NovelCore/Runtime/UI/EndOfStoryPanel/EndOfStoryPanel.cs` (NEW)
- **Reason**: 
  - UI компонент для отображения немодального окна "Сюжет завершён"
  - Должен слушать новый event или существующий OnDialogueComplete с контекстом

### 8. `Assets/Scripts/NovelCore/Tests/Runtime/Core/DialogueSystem/EndOfStoryTests.cs` (NEW)
- **Reason**:
  - Failing test для воспроизведения бага
  - Regression tests после исправления
