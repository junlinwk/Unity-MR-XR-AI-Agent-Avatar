# IDMR 2026 HW2 — Conversational MR Doctor Agent

A Unity Mixed Reality conversational agent featuring a virtual doctor that listens, explains, expresses concern, and gets frustrated through native LLM tool calling.

Course: IDMR 2026 — Homework 2

---

## Demo Video

**YouTube**：[待補上 — Coming Soon]

> Demo 影片因 **macOS 開發環境限制**（Meta Avatar SDK 不提供 macOS native binary，Mac Editor 無法顯示 Avatar）與 build / 部署時間問題，目前尚未錄製完成。
> 詳見 [`HW2_Report.pdf`](../HW2_Report.pdf) Section 1 平台限制說明。

---

## Tech Stack

- **Unity** 6000.0.50f1
- **Meta XR SDK** 78.0.0
- **Meta Avatars SDK** 40.0.1
- **OpenAI Realtime API**（GA，model: `gpt-realtime-mini`）

## Key Features

| 功能 | 說明 |
|---|---|
| 5-state Animator | Idle / Listening / Explaining / Concerned / Frustrated |
| LLM Native Function Calling | LLM 自主呼叫 `set_doctor_state` 切換 Avatar 情緒，**非關鍵字 matching** |
| Personality 設計 | 連續質疑 ≥3 次 → Frustrated（Mutant Jumping 跳腳）|
| Context-aware 物件 | Fever Thermometer / Medicine Jar / X-Ray Film |
| Lip-sync | Audio-driven FFT（template 提供）|
| Dev 工具 | `TextInputDebug` — 鍵盤輸入測試 UI（不需麥克風）|
| GA API 升級 | Realtime API 從 Beta 升級到 GA（事件改名、移除 Beta header）|

## Documentation

- [`HW2_Report.pdf`](../HW2_Report.pdf) — 完整作業報告
- [`HW2_手把手教學.md`](../HW2_手把手教學.md) — 從 template 到完成的完整實作教學

## Quick Start

```bash
git clone <this-repo-url>
# Unity Hub → 開啟，選 Unity 6000.0.50f1
# 開啟 Assets/Scenes/Conversational AI Template.unity
# 在 RealtimeAPI GameObject 的 RealtimeAPIWrapper.apiKey 填入 OpenAI API key
```

### Build to Quest

```
File → Build Settings → Platform: Android (Switch Platform)
Texture Compression: ASTC
Build → 產生 APK → adb install -r <output>.apk
```

> ⚠️ **macOS 使用者注意**：Unity Editor on Mac **無法顯示** Meta Avatar（SDK 不提供 macOS native binary）。對話 / 動畫 / lip-sync 邏輯可在 Mac Editor 從 Console log 驗證，**Avatar 視覺必須在 Windows Editor 或 Quest 真機上才看得到**。

## Project Structure

```
Unity-MR-XR-AI-Agent-Avatar/
├── Assets/
│   ├── Animations/
│   │   ├── DoctorAnimator.controller        (5 states, 8 transitions)
│   │   ├── Breathing Idle.fbx               (Idle)
│   │   ├── Talking.fbx                      (Listening / Explaining)
│   │   ├── Thinking.fbx                     (Concerned)
│   │   └── Mutant Jumping.fbx               (Frustrated)
│   ├── Resources/
│   │   ├── fever-thermometer/               (Sketchfab CC-BY)
│   │   └── medicine-jar/                    (capsule textures)
│   ├── Scenes/
│   │   └── Conversational AI Template.unity
│   └── Scripts/
│       ├── Animation/AnimationHandler.cs            (DoctorState enum + Queue)
│       ├── OpenAI API/RealtimeAPIWrapper.cs         (WebSocket + tool calling)
│       └── UI/TextInputDebug.cs                     (keyboard input dev tool)
```

## Credits

- Template：IDMR 2026 課程提供
- 動畫：Mixamo
- 3D 模型：Sketchfab（CC-BY licensed assets）
