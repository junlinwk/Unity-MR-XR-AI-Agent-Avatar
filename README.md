# IDMR 2026 HW2 — Conversational MR Doctor Agent

A Unity Mixed Reality conversational agent featuring a virtual doctor that listens, explains, expresses concern, and gets frustrated through native LLM tool calling.

Course: IDMR 2026 — Homework 2

---

## Demo Video

**YouTube**：<https://youtu.be/YHNDf6Sfygc>

---

## Catalogue

- [Demo Video](#demo-video)
- [Tech Stack](#tech-stack)
- [Key Features](#key-features)
- [Quick Start](#quick-start)
  - [Build to Quest](#build-to-quest)
- [給未來修這門課的 Mac 同學](#給未來修這門課的-mac-同學)
  - [前提：請放棄幻想](#前提請放棄幻想)
  - [開發步驟（如果你只有 Mac）](#開發步驟如果你只有-mac)
    - [第一次 build 會遇到簽章衝突](#第一次-build-會遇到簽章衝突)
    - [在頭顯啟動 app](#在頭顯啟動-app)
  - [小提醒](#小提醒)
- [Project Structure](#project-structure)
- [Credits](#credits)


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

## Quick Start

```bash
git clone https://github.com/junlinwk/Unity-MR-XR-AI-Agent-Avatar.git
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

> ⚠️ **macOS User特別注意一下**：Unity Editor on Mac **無法顯示** Meta Avatar（SDK 不提供 macOS native binary）。對話 / 動畫 / lip-sync 邏輯可在 Mac Editor 從 Console log 驗證，**Avatar 視覺必須在 Windows Editor 或 Quest 真機上才看得到**。

---

## 給未來修這門課的 Mac 同學

### 前提：請放棄幻想

1. Mac 上**不能**正常用 **PC版** Meta Quest Link（PC VR Streaming）— PD 虛擬機 / CrossOver 都試過了，都不行
2. Mac 也**不能 Build And Run** 直接部署 Quest — GitHub / Unity 同步給 PC 是必須的
3. Avatar **連 Meta XR Simulator 也跑不出來**（SDK 完全不支援 macOS）

如果你看到下面這些 log，就是這個問題：

```
[ovrAvatar2] Lib Assets/Oculus/Avatar2/Plugins/Macos/
  libovrplugintracking.framework/libovrplugintracking not found
[ovrAvatar2 manager] DllNotFound: libovrgpuskinning.framework/libovrgpuskinning
[ovrAvatar2 manager] ovrGpuSkinning_Initialize failed
[ovrAvatar2 manager] `libovravatar2` binary was not found,
  shutting down AvatarSDK
```

> → 三個 native binary（libovrplugintracking / libovrgpuskinning / libovravatar2）Meta 沒 mac 版，whole Avatar SDK shutdown。

### 開發步驟（如果你只有 Mac）

必須 **Sideload**：Mac build APK → 傳到教室 PC → SideQuest 安裝到 Quest

**以下教你 Side Load**

1. Mac 上選 **File → Build Settings → Build**（不是 Build and Run，那個會 fail）
2. 把產生的 `.apk` 想辦法弄到 028 教室 PC（GitHub / Google Drive / 隨身碟 / 某種方式Sync都可）
3. 教室 PC 開 **SideQuest** App（如果沒裝 → <https://sidequestvr.com/>），應該都有
4. Quest 用 USB-C 接 PC，戴頭顯**允許 USB debugging**，SideQuest 左上角會顯示綠色 Connected
5. SideQuest 工具列右上 **「Install APK file from folder」** 按鈕 → 選你的 APK 載入

#### 第一次 build 會遇到簽章衝突

報錯：
```
INSTALL_FAILED_UPDATE_INCOMPATIBLE
Package com.urpblank signatures do not match newer version
```

**原因**：URP template 預設 package name `com.urpblank`，跟之前其他同學殘留在 Quest 上的版本撞 keystore。

**解法**：
1. SideQuest 工具列上方 **箱子圖案**（Currently Installed Apps）
2. 列表找 `com.urpblank`（或結尾 `.urpblank` 的）→ 右邊 **垃圾桶** 刪掉
3. 重新 Install APK → 成功（理論上要成功，如果你在這失敗了請出門右轉問AI）

> 一勞永逸：Unity → Project Settings → Player → Android → Other Settings → **Package Name** 改成自己的（例：`com.idmr2026.hw2doctor`），就不會再撞了

#### 在頭顯啟動 app

1. 戴 Quest 頭顯
2. 點右手 controller 的 **Meta 按鈕**（8字那個）
3. 下面工具列選 **九個點的圖示**（Library / 應用程式庫）
4. 左邊分類點 **「未知來源」**（Unknown Sources）
5. 找到你的 app 點開

### 小提醒

1. 助教 Project Settings 預設用 **Oculus**，沒關係改成 **OpenXR** 也能用
2. **API key 不要 commit 進 git**！記得 build 前在 RealtimeAPIWrapper Inspector 填，每次重新 clone 都要重填
3. Mac Editor 雖然看不到 Avatar，但可以從 Console 驗證對話 / 動畫邏輯（看 `[AnimationHandler] doctorState -> Concerned` 之類 log）

---

## Project Structure

```
Unity-MR-XR-AI-Agent-Avatar/
├── Assets/
│   ├── Animations/
│   │   ├── DoctorAnimator.controller        (5 states, 8 transitions, 主用)
│   │   ├── AvatarControllerExample.controller (template 原本的，已不用)
│   │   ├── Breathing Idle.fbx               (Idle 動畫)
│   │   ├── Talking.fbx                      (Listening / Explaining)
│   │   ├── Thinking.fbx                     (Concerned)
│   │   ├── Mutant Jumping.fbx               (Frustrated)
│   │   └── Head Nod Yes.fbx                 (備用)
│   │
│   ├── Materials/
│   │   ├── Box.mat / glass.mat / jar.mat / xray.mat   (PBR 材質)
│   │
│   ├── Resources/
│   │   ├── fever-thermometer/   (溫度計模型 + 4 套 PBR 貼圖)
│   │   ├── medicine-jar/        (藥罐 + capsule 貼圖)
│   │   └── Humanoid Avatar Rig v5 Variant - Custom Animation Only.prefab
│   │
│   ├── Scenes/
│   │   └── Conversational AI Template.unity   (主場景)
│   │
│   ├── Scripts/
│   │   ├── Animation/
│   │   │   └── AnimationHandler.cs           (DoctorState enum + thread-safe Queue)
│   │   ├── Audio/
│   │   │   ├── AudioPlayer.cs                (FFT lip-sync, template)
│   │   │   ├── AudioRecorder.cs              (VAD recording, template)
│   │   │   └── AudioProcessingUtils.cs
│   │   ├── Objects/
│   │   │   └── ContextAwareObjectHandler.cs  (trigger → 送 context 給 LLM)
│   │   ├── OpenAI API/
│   │   │   ├── RealtimeAPIWrapper.cs         (WebSocket + tool calling 核心)
│   │   │   ├── RealtimeAPIConnection.cs      (連線狀態管理)
│   │   │   ├── GPTRequest.cs                 (.NET ChatClient 範例，未啟用)
│   │   │   └── functionANDTool.cs            (function call 範例)
│   │   └── UI/
│   │       ├── TextInputDebug.cs             (鍵盤輸入測試)
│   │       ├── Transcript.cs                 (顯示對話)
│   │       └── MessageBlock.cs
│   │
│   ├── StreamingAssets/
│   │   └── Oculus/OvrAvatar2Assets.zip       (preset avatar 資料)
│   │
│   └── Samples/Meta Avatars SDK/40.0.1/       (Avatar SDK 範例)
│
├── Packages/
│   └── manifest.json                          (Unity 套件清單)
│
├── ProjectSettings/                           (Unity 專案設定)
│
└── README.md                                  (本檔)
```

---

## Credits

- **Template**：IDMR 2026 Course (NCTU)
- **動畫**：[Mixamo](https://www.mixamo.com/) — Breathing Idle, Talking, Thinking, Mutant Jumping
- **3D 模型**：[Sketchfab](https://sketchfab.com/) — CC-BY licensed thermometer & medicine jar
- **LLM**：OpenAI Realtime API (gpt-realtime-mini)
- **Avatar**：Meta Avatars SDK 40.0.1
