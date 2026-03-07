# Установка необходимых пакетов Unity

## Критические пакеты (без них код не компилируется)

### 1. Unity Addressables 2.0+

**Статус**: ❌ НЕ УСТАНОВЛЕН

**Установка через Unity Package Manager**:
1. Откройте Unity Editor
2. Window → Package Manager
3. Нажмите "+" в левом верхнем углу
4. Выберите "Add package by name..."
5. Введите: `com.unity.addressables`
6. Нажмите "Add"

**Или через manifest.json**:
Добавьте в `Packages/manifest.json` в секцию `dependencies`:
```json
"com.unity.addressables": "2.0.8",
```

**Используется в**:
- `IAssetManager.cs` - interface для загрузки ассетов
- `AddressablesAssetManager.cs` - реализация через Addressables
- Все `AssetReference` поля в ScriptableObjects
- `SceneManager.cs` - загрузка сцен и персонажей

### 2. VContainer 1.14+

**Статус**: ❌ НЕ УСТАНОВЛЕН

**Установка**:
VContainer не является официальным пакетом Unity. Установка через:

**Способ 1 - OpenUPM (рекомендуется)**:
```bash
openupm add jp.hadashikick.vcontainer
```

**Способ 2 - Через Package Manager (git URL)**:
1. Window → Package Manager
2. "+" → "Add package from git URL..."
3. Введите: `https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.14.0`

**Способ 3 - Через manifest.json**:
Добавьте в `Packages/manifest.json`:
```json
"dependencies": {
  "jp.hadashikick.vcontainer": "https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.14.0"
}
```

**Используется в**:
- `GameLifetimeScope.cs` - DI контейнер
- Все конструкторы сервисов (DialogueSystem, SceneManager, etc.)
- Регистрация всех сервисов

### 3. Unity Localization 2.0+

**Статус**: ❌ НЕ УСТАНОВЛЕН

**Установка через Unity Package Manager**:
1. Window → Package Manager
2. "+" → "Add package by name..."
3. Введите: `com.unity.localization`
4. Нажмите "Add"

**Или через manifest.json**:
```json
"com.unity.localization": "2.0.1",
```

**Используется в**:
- `ILocalizationService.cs` - interface
- `UnityLocalizationService.cs` - обёртка (пока не реализован)
- Все DialogueLineData с локализацией текста

### 4. TextMeshPro

**Статус**: ⚠️ ПРОВЕРИТЬ

**Установка**:
1. Window → Package Manager
2. Найдите "TextMeshPro" в списке Unity пакетов
3. Нажмите "Install"

**Или через manifest.json**:
```json
"com.unity.textmeshpro": "3.0.9",
```

**Используется в**:
- `DialogueBoxController.cs` - TextMeshProUGUI для диалогов
- `ChoiceUIController.cs` - TextMeshProUGUI для кнопок выбора

## Опциональные пакеты (для расширенной функциональности)

### 5. Spine-Unity 4.2+

**Статус**: ❌ НЕ УСТАНОВЛЕН

**Установка**:
1. Скачайте Spine-Unity runtime с http://esotericsoftware.com/spine-unity-download
2. Импортируйте .unitypackage через Assets → Import Package → Custom Package

**Или**:
- Доступен через Unity Asset Store

**Используется в**:
- `SpineCharacterAnimator.cs` - анимация персонажей через Spine

**Важно**: Код уже написан для поддержки Spine, но будет работать только после установки пакета.

### 6. Steamworks.NET 20.2+

**Статус**: ❌ НЕ УСТАНОВЛЕН

**Установка**:
1. Скачайте с GitHub: https://github.com/rlabrecque/Steamworks.NET/releases
2. Импортируйте .unitypackage
3. Настройте steam_appid.txt

**Используется в**:
- `SteamPlatformService.cs` (пока не реализовано)
- Интеграция с Steam (достижения, облачные сохранения)

**Важно**: Требуется только для Steam сборок, не критично для MVP.

## Быстрая установка через manifest.json

Замените содержимое `novel-core/Packages/manifest.json` на:

```json
{
  "dependencies": {
    "com.unity.addressables": "2.0.8",
    "com.unity.localization": "2.0.1",
    "com.unity.textmeshpro": "3.0.9",
    "com.unity.collab-proxy": "2.11.3",
    "com.unity.feature.2d": "2.0.1",
    "com.unity.ide.rider": "3.0.39",
    "com.unity.ide.visualstudio": "2.0.27",
    "com.unity.inputsystem": "1.19.0",
    "com.unity.multiplayer.center": "1.0.0",
    "com.unity.render-pipelines.universal": "17.0.4",
    "com.unity.test-framework": "1.6.0",
    "com.unity.timeline": "1.8.10",
    "com.unity.ugui": "2.0.0",
    "com.unity.visualscripting": "1.9.7",
    "jp.hadashikick.vcontainer": "https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.14.0",
    "com.unity.modules.accessibility": "1.0.0",
    "com.unity.modules.ai": "1.0.0",
    "com.unity.modules.androidjni": "1.0.0",
    "com.unity.modules.animation": "1.0.0",
    "com.unity.modules.assetbundle": "1.0.0",
    "com.unity.modules.audio": "1.0.0",
    "com.unity.modules.cloth": "1.0.0",
    "com.unity.modules.director": "1.0.0",
    "com.unity.modules.imageconversion": "1.0.0",
    "com.unity.modules.imgui": "1.0.0",
    "com.unity.modules.jsonserialize": "1.0.0",
    "com.unity.modules.particlesystem": "1.0.0",
    "com.unity.modules.physics": "1.0.0",
    "com.unity.modules.physics2d": "1.0.0",
    "com.unity.modules.screencapture": "1.0.0",
    "com.unity.modules.terrain": "1.0.0",
    "com.unity.modules.terrainphysics": "1.0.0",
    "com.unity.modules.tilemap": "1.0.0",
    "com.unity.modules.ui": "1.0.0",
    "com.unity.modules.uielements": "1.0.0",
    "com.unity.modules.umbra": "1.0.0",
    "com.unity.modules.unityanalytics": "1.0.0",
    "com.unity.modules.unitywebrequest": "1.0.0",
    "com.unity.modules.unitywebrequestassetbundle": "1.0.0",
    "com.unity.modules.unitywebrequestaudio": "1.0.0",
    "com.unity.modules.unitywebrequesttexture": "1.0.0",
    "com.unity.modules.unitywebrequestwww": "1.0.0",
    "com.unity.modules.vehicles": "1.0.0",
    "com.unity.modules.video": "1.0.0",
    "com.unity.modules.vr": "1.0.0",
    "com.unity.modules.wind": "1.0.0",
    "com.unity.modules.xr": "1.0.0"
  }
}
```

**После изменения manifest.json**:
1. Сохраните файл
2. Unity автоматически обнаружит изменения и загрузит пакеты
3. Дождитесь завершения импорта (может занять несколько минут)

## Проверка установки

После установки всех пакетов проверьте:
1. Window → Package Manager → показывает все установленные пакеты
2. Нет ошибок компиляции в Console
3. Assets → Create → Addressables видны в меню (для Addressables)
4. VContainer namespace доступен в коде

## Текущий статус кода

**Код уже написан и готов к использованию**:
- ✅ DialogueSystem
- ✅ SceneManager  
- ✅ ChoiceUIController
- ✅ DialogueBoxController
- ✅ Character Animation System
- ✅ Data Models (SceneData, CharacterData, etc.)

**Но код не скомпилируется без**:
- Addressables (для AssetReference)
- VContainer (для DI)
- TextMeshPro (для UI текста)

## Следующие шаги

1. **Установите критические пакеты** (Addressables, VContainer, TextMeshPro, Localization)
2. **Откройте Unity Editor** и дождитесь компиляции
3. **Проверьте Console** на отсутствие ошибок
4. **Настройте Addressables** (T009 из tasks.md)
5. **Создайте UI префабы** (DialogueBox, ChoiceButton)

## Ссылки

- Unity Addressables: https://docs.unity3d.com/Packages/com.unity.addressables@latest
- VContainer: https://github.com/hadashiA/VContainer
- Unity Localization: https://docs.unity3d.com/Packages/com.unity.localization@latest
- TextMeshPro: https://docs.unity3d.com/Manual/com.unity.textmeshpro.html
- Spine-Unity: http://esotericsoftware.com/spine-unity
- Steamworks.NET: https://github.com/rlabrecque/Steamworks.NET
