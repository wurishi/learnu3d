Unity 2021.3.33f1
需要 Unity2022，使用 Entities 1.0 以上版本

# 1. 概念

# 2. 安装

1. 使用 U3D(URP) 模板创建项目。

2. 点击 ReadMe 文件，点击 Remove ReadMe Assets 保持项目干净。

3. Windows -> Package Manager -> Add package by name 安装 -> `com.unity.entities.graphics` 如果无法在列表中显示 entities 可能会提示安装失败，此时参见 4。

4. Windows -> Package Manager -> Add package from git URL -> `com.unity.entities` 以下为可能即使打开 Enable Pre-release 都无法可见的 packages:

   ```
   com.ptc.vuforia.engine
   com.unity.2d.entities
   com.unity.ai.planner
   com.unity.aovrecorder
   com.unity.assetbundlebrowser
   com.unity.assetgraph
   com.unity.barracuda
   com.unity.barracuda.burst
   com.unity.build-report-inspector
   com.unity.cloud.userreporting
   com.unity.collections
   com.unity.connect.share
   com.unity.dots.editor
   com.unity.entities
   com.unity.film-tv.toolbox
   com.unity.google.resonance.audio
   com.unity.immediate-window
   com.unity.mathematics
   com.unity.meshsync
   com.unity.multiplayer-hlapi
   com.unity.package-manager-doctools
   com.unity.package-manager-ui
   com.unity.package-validation-suite
   com.unity.physics
   com.unity.platforms
   com.unity.platforms.android
   com.unity.platforms.linux
   com.unity.platforms.macos
   com.unity.platforms.web
   com.unity.platforms.windows
   com.unity.playablegraph-visualizer
   com.unity.render-pipelines.lightweight
   com.unity.rendering.hybrid
   com.unity.renderstreaming
   com.unity.scene-template
   com.unity.simulation.client
   com.unity.simulation.core
   com.unity.simulation.capture
   com.unity.simulation.games
   com.unity.standardevents
   com.unity.streaming-image-sequence
   com.unity.test-framework.performance
   com.unity.tiny.all
   com.unity.transport
   com.unity.upm.develop
   com.unity.vectorgraphics
   com.unity.webrtc
   com.unity.xr.googlevr.android
   com.unity.xr.googlevr.ios
   com.unity.xr.legacyinputhelpers
   com.unity.xr.oculus.android
   com.unity.xr.oculus.standalone
   com.unity.xr.openvr.standalone
   com.unity.xr.arsubsystems
   com.unity.xr.interactionsubsystems
   com.unity.xr.windowsmr.metro
   ```

# 3. 调整设置

1. Edit -> Project Settings -> Editor

   找到 Enter Play Mode Settings

   - Enter Play Mode Options -> On
   - Reload Domain -> Off
   - Reload Scene -> Off

   主要是为了减少点击 Play 后编译的次数

2. Edit -> Preferences -> Entities 

   Baking -> Scene View Mode -> Runtime Data

3. 建立目录结构

   - Prefabs
   - Scenes
   - Scripts
     - Aspects
     - Authoring
     - Components
     - MonoBehaviours
     - Systems
   - Settings

# 4. Sub Scene

在 SampleScene 下，Hierarchy 窗口下按右键 -> New Sub Scene -> Empty Scene -> 创建 EntityScene 

根据 Entity1.0 规范，所有 Sub Scene 里的 GameObject 在 Runtime 周期都会被转换成 Entity，即 ECS 都是放在 SubScene 中执行的。

# 5. 建立炮台

1. 右击 EntityScene -> GameObject -> 3D Object -> Cube 命名为 Tank（p/r/s reset 一下保证初始化）
2. Tank 的 Box Collider -> Off
3. 右击 Tank -> 3D Object -> Sphere，命名为 Turret
4. Turret 的 Position(0, 0.5, 0)，Rotation(45, 0, 0)，Sphere Collider -> Off
5. 右击 Turret -> 3D Object -> Cylinder 命名为 Cannon
6. Cannon 的 Position(0, 0.5, 0)，Scale(0.2, 0.5, 0.2)，Capsule Collider -> Off
7. 右击 Cannon -> Create Empty 命名为 SpawnPoint
8. SpawnPoint 的 Position(0, 1, 0)，Rotation(-90, 0, 0) 这个点用来做为基准点

# 6. 旋转炮台

1. Scripts/Systems 下创建 TurretRotationSystem.cs
2. 