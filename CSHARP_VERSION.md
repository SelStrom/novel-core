# C# Version Note: Unity 6 and C# 9

**Date**: 2026-03-07  
**Unity Version**: Unity 6 (LTS)  
**Supported C# Version**: C# 9.0

## Important Clarification

Unity 6 supports **C# 9.0**, not C# 10. The initial project documentation incorrectly referenced C# 10.0+ features.

## C# 9.0 Features Available

The following C# 9 features ARE available and can be used:

### ✅ Records
```csharp
public record CharacterEmotion(string emotionName, AssetReference sprite);
```

### ✅ Init-only Setters
```csharp
public string Name { get; init; }
```

### ✅ Pattern Matching Enhancements
```csharp
if (obj is not null) { }
value switch { ... }
```

### ✅ Target-typed New
```csharp
List<string> names = new();
```

### ✅ Covariant Returns
```csharp
public override BaseClass GetValue() => new DerivedClass();
```

## C# 10 Features NOT Available

The following C# 10 features are NOT supported in Unity 6:

### ❌ File-Scoped Namespaces
```csharp
// NOT SUPPORTED - Will cause compilation error
namespace NovelCore.Runtime;

public class MyClass { }
```

**Workaround**: Use traditional namespace blocks:
```csharp
namespace NovelCore.Runtime
{
    public class MyClass { }
}
```

### ⚠️ Global Usings (Partial Support)
```csharp
// GlobalUsings.cs works, but it's a C# 10 feature
// Unity 6 may have limited or no support for this
global using System;
```

**Recommendation**: If global usings don't work, revert to explicit using statements in each file.

### ❌ Record Structs
```csharp
// NOT SUPPORTED
public record struct Point(int X, int Y);
```

**Workaround**: Use readonly struct or class records.

### ❌ Lambda Improvements
```csharp
// NOT SUPPORTED
var parse = (string s) => int.Parse(s);
```

## Current Codebase Status

The current codebase uses:
- ✅ Traditional namespace blocks (compatible)
- ⚠️ Global usings in `GlobalUsings.cs` (may need to be removed if not supported)
- ✅ Target-typed new expressions
- ✅ Standard C# 9 features

## Action Required

If you encounter compilation errors related to:
1. **Global usings not working**: Remove `GlobalUsings.cs` and add explicit using statements to each file
2. **Namespace errors**: All namespaces currently use traditional block syntax (no changes needed)
3. **Record issues**: Use class records only (not record structs)

## Updated Documentation

The following files have been corrected:
- ✅ `constitution.md` → v1.2.3 (C# 9.0)
- ✅ `plan.md` → C# 9.0 specification
- ✅ `GlobalUsings.cs` → Updated comment to C# 9.0

## Testing Recommendation

After Unity packages are installed (T002), verify that:
1. Project compiles without errors
2. Global usings work (or remove them if not)
3. All C# 9 features work as expected

If global usings cause issues, they can be safely removed and replaced with per-file using statements.
