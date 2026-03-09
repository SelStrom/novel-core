# Navigation Buttons - Quick Test Guide

## ⚡ Quick Test (2 minutes)

1. Open Unity Editor
2. Load `Assets/Scenes/SampleScene.unity`
3. Press Play ▶️
4. Click through dialogue (Space or click screen)
5. Make a choice to go to second scene
6. Click **Back button** (bottom of screen)
7. ✅ **Expected**: Returns to first scene, dialogue restarts

---

## 📊 What To Check

### Visual Check
- [ ] Two buttons visible at bottom of screen
- [ ] Buttons have labels "◀ Back" and "Forward ▶"
- [ ] Back button is grey (disabled) on first scene
- [ ] Back button becomes darker (enabled) after going to second scene

### Console Check
After clicking Back button, look for these logs:
```
✅ SceneNavigationUI: Back button clicked!
✅ SceneManager: Navigating back to scene '...'
✅ SceneNavigationUI: Navigated back successfully
```

---

## 🐛 If It Doesn't Work

### Check Console for Error Messages

**If you see**: 
```
SceneNavigationUI: Cannot navigate back - no previous scenes in history
```
→ This is **normal on the first scene**. You need to navigate to a second scene first.

**If you see**:
```
NavigationUIManager: ISceneManager not injected!
```
→ Regenerate Sample Project: `Window → NovelCore → Generate Sample Project`

**If you see no logs at all**:
→ Check that `NavigationUIManager` GameObject exists in scene Hierarchy

---

## 🔧 What Was Fixed

**Problem**: Using `.ContinueWith()` for async operations doesn't work properly in Unity
**Solution**: Replaced with proper `async void` pattern

**Files changed**:
- `SceneManager.cs` - Fixed async navigation
- `SceneNavigationUI.cs` - Better error messages

---

## 📚 More Information

- **Troubleshooting**: See `NAVIGATION_BUTTONS_TROUBLESHOOTING.md`
- **Technical details**: See `NAVIGATION_FIX_SUMMARY.md`
- **Previous fixes**: See `NAVIGATION_FIXES_REPORT.md`

---

## ✅ Success Criteria

Navigation is working if:
1. Back button is disabled on first scene (grey)
2. Back button becomes enabled after navigating to second scene
3. Clicking Back button returns to previous scene
4. Dialogue restarts from beginning
5. Forward button becomes enabled after going back
6. Clicking Forward button goes forward again

---

**Test now and report results! 🚀**
