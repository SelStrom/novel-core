# Рефакторинг LocalizationService API

**Дата**: 2026-03-07  
**Автор изменений**: Refactoring per user request

## Изменения

### 1. ILocalizationService.cs
- ❌ Удалены методы:
  - `string GetLocalizedString(string key)` - возвращал ключ при неудаче (неоднозначно)
  - `string GetLocalizedString(string key, params object[] args)` - то же
  - `bool HasKey(string key)` - избыточно с Try-паттерном

- ✅ Добавлены методы:
  - `bool TryGetLocalizedString(string key, out string result)` - явное указание успеха/неудачи
  - `bool TryGetLocalizedString(string key, out string result, params object[] args)` - с форматированием

### 2. BasicLocalizationService.cs
- Реализован Try-паттерн
- Добавлена валидация: `string.IsNullOrEmpty(key)` → `return false`
- Добавлена обработка `FormatException` для форматирования
- При неудаче: `result = null`, возврат `false`
- При успехе: `result = localized string`, возврат `true`

### 3. Обновлены call sites (2 места)

**ChoiceUIController.cs** (строка 332):
```csharp
// БЫЛО
string localizedText = _localizationService.GetLocalizedString(textKey);
if (!string.IsNullOrEmpty(localizedText)) { ... }

// СТАЛО
if (_localizationService.TryGetLocalizedString(textKey, out string localizedText)) 
{
    return localizedText;
}
```

**DialogueBoxController.cs** (строка 209):
```csharp
// Аналогичное изменение
```

## Преимущества

1. **Явная обработка ошибок**: Нельзя спутать успех с неудачей
2. **Стандартный .NET паттерн**: Как `Dictionary.TryGetValue`, `int.TryParse`
3. **Производительность**: Не создаётся строка при неудаче (раньше возвращался ключ)
4. **Чище код**: Убраны проверки `!string.IsNullOrEmpty(localizedText)`

## Тестирование

После изменений нужно протестировать:
- ✅ Существующий ключ → `true`, корректный текст
- ✅ Несуществующий ключ → `false`, `result = null`
- ✅ Null/empty ключ → `false`, `result = null`
- ✅ Форматирование с аргументами → корректная подстановка
- ⚠️ Форматирование без аргументов → `false`, warning в лог

## Обратная совместимость

**Breaking change** - все вызовы `GetLocalizedString` требуют обновления.

**Статус**: Pre-MVP (v0.1.0) - нет внешних потребителей API, безопасно менять.
