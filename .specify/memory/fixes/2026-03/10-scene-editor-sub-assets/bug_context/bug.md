# Bug Description

DialogueLineData и ChoiceData создаются как отдельные standalone assets в SceneEditorWindow, вместо sub-assets (как в SampleProjectGenerator). Это приводит к несогласованности подходов и проблемам для контент-криейторов.

## Expected Behavior

При создании DialogueLineData или ChoiceData через SceneEditorWindow:
- Объекты должны создаваться как **sub-assets** внутри файла SceneData
- Использование `AssetDatabase.AddObjectToAsset(newLine, sceneData)`
- В Project Browser должна быть видна только SceneData, без множества мелких asset-файлов
- При копировании/удалении SceneData все dialogue lines и choices должны автоматически копироваться/удаляться

## Actual Behavior

При создании DialogueLineData или ChoiceData через SceneEditorWindow:
- Создаются отдельные standalone asset-файлы (`DialogueLine 1.asset`, `Choice 1.asset`, etc.)
- Используется `AssetDatabase.CreateAsset(newLine, linePath)`
- Project Browser засоряется множеством мелких файлов
- При копировании SceneData dialogue lines остаются в исходной директории (broken references)
- Риск случайного удаления dialogue line без удаления ссылки из SceneData

## Steps to Reproduce

1. Открыть Scene Editor Window (`Window > NovelCore > Scene Editor`)
2. Создать новую сцену или выбрать существующую SceneData
3. Нажать кнопку "+ Add Dialogue Line"
4. Наблюдать в Project Browser создание файла `DialogueLine 1.asset` в той же директории
5. Повторить для Choice: нажать "+ Add Choice"
6. Наблюдать создание файла `Choice 1.asset`

**Ожидается**: Dialogue lines и choices должны быть sub-assets внутри SceneData (как в Sample проекте)
**Фактически**: Создаются отдельные asset-файлы

## Environment

- Unity Version: 6000.0.69f1
- Platform: macOS (darwin 25.2.0)
- NovelCore Version: 1.16.0 (Constitution)
- Project Path: `/Users/selstrom/work/projects/novel-core/novel-core`

## Evidence

### SampleProjectGenerator (правильный подход)
```csharp
// Lines 420-426 in SampleProjectGenerator.cs
var line = CreateDialogueLine(content, fileName, lineIndex);
dialogueLines.Add(line);

// Add as sub-asset to the SceneData asset
AssetDatabase.AddObjectToAsset(line, scene);
```

### SceneEditorWindow (неправильный подход)
```csharp
// Lines 575-579 in SceneEditorWindow.cs
var newLine = CreateInstance<DialogueLineData>();
string linePath = AssetDatabase.GenerateUniqueAssetPath($"{directory}/DialogueLine.asset");

AssetDatabase.CreateAsset(newLine, linePath);  // ❌ Создает отдельный asset
```

## Impact

### Constitution Violations

**Principle I (Creator-First Design)**:
> "Content creators (writers, artists, game designers) are the primary users."

- ❌ Засорение Project Browser множеством файлов усложняет навигацию
- ❌ Риск broken references при копировании сцен
- ❌ Необходимость вручную удалять связанные assets

**Principle VI (Modular Architecture & Testing)**:
> "Scene as atomic unit of work"

- ❌ Dialogue lines существуют отдельно от сцены, нарушая атомарность
- ❌ Копирование сцены не копирует её реплики автоматически

### User Experience Issues

1. **Project Browser clutter**: Сцена с 50 репликами = 50+ файлов
2. **Broken references risk**: Случайное удаление dialogue line → broken reference
3. **Manual cleanup required**: При удалении сцены dialogue lines остаются orphaned
4. **Git noise**: Каждая правка dialogue = отдельный commit + .meta файл
5. **Inconsistency**: Sample проект использует sub-assets, но ручное создание — standalone

## Related Files

- `SceneEditorWindow.cs` - CreateNewDialogueLine() и CreateNewChoice()
- `SampleProjectGenerator.cs` - эталонная реализация с sub-assets
- `SceneData.cs` - содержит List<DialogueLineData> и List<ChoiceData>
- `DialogueLineData.cs` - ScriptableObject для реплик
- `ChoiceData.cs` - ScriptableObject для выборов
