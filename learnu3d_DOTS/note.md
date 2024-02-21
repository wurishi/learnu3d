Unity 2022.3.16f1

# 1. 概念

# 2. 安装

1. 使用 U3D(URP) 模板创建项目。

2. 点击 ReadMe 文件，点击 Remove ReadMe Assets 保持项目干净。

3. Windows -> Package Manager -> Add package by name 安装 -> `com.unity.entities.graphics` -> 会安装相对应的包和依赖包。

# 3. 调整设置

1. Edit -> Project Settings... -> Editor

2. Enter Play Mode Settings -> Enter Play Mode Options -> On

3. Reload Domain -> Off

4. Reload Scene -> Off

5. 2.3.4 用来决定点击 Play 时要不要重新载入场景和重新编译脚本。关闭后 Play 可以减少一次全局刷新和编译的过程。

6. Edit -> Preferences -> Entities

7. Baking -> Scene View Mode -> Runtime Data 修改后 ECS 的内容可以在 Play 的模式下在 Scene 中正常显示。

# 4. 创建目录结构

```
- Prefabs
- Scenes
- Scripts
	- Aspects
	- Authoring
	- Components
	- MonoBehaviours
	- Systems
- Settings
```

# 5. Sub Scene

1. 保持打开了 SimpleScene。
2. 在 Hierarchy 窗口右击 -> New Sub Scene -> Empty Scene -> 保存为 EntityScene，目录为默认的 SimpleScene/ 下。
3. 这步操作是 ECS 的基础，在 Entities 1.0 的规范里，任何放在 Sub Scene 里的 Game Object 的 Runtime 周期都会被转为 Entity 物件。可以理解为 ECS 都在 SubScene 里执行的。

# 6. 制作炮塔

1. 在 EntityScene 上右击 -> GameObject -> 3D Object -> Cube，命名为 Tank。Position(0, 0, 0), Rotation(0, 0, 0), Scale(1, 1, 1)。Box Collider -> Off
2. 在 Tank 上右击 -> 3D Object -> Sphere，命名为 Turret。Position(0, 0.5, 0), Rotation(45, 0, 0), Scale(1, 1, 1)。Sphere Collider -> Off
3. 在 Turret 上右击 -> 3D Object -> Cylider，命名为 Cannon。Position(0, 0.5, 0), Rotation(0, 0, 0), Scale(0.2, 0.5, 0.2)。Capsule Collider -> Off
4. 在 Cannon 上右击 -> Create Empty，命名为 SpawnPoint。Position(0, 1, 0), Rotation(-90, 0, 0), Scale(1, 1, 1)。这个点用来作为炮弹的基准点。

# 7. 旋转炮塔

1. 在 Scripts/System 下创建 TurretRotationSystem.cs。
2. `partial struct TurretRotationSystem : ISystem`
3. 在 `OnUpdate` 中使用 `SystemAPI.Query<LocalTransform>()` 查询所有的 LocalTransform。在当前 `Entities 1.0.16` 版本下，所有 SubScene 场景下的转换为 ECS 生命周期的 GameObject 默认是没有 LocalTransform 的，必须添加一个 Authoring 组件后才会自动添加 LocalTransform 组件。
4. 打开 Window -> Entities -> Hierarchy 可以观察到 Play 后 ECS 组件的情况。
5. 在 Scripts/Components 下创建 Turret Component，它的格式为 `struct Turret : IComponentData`
6. 在 Scripts/Authoring 下创建 TurretAuthoring，目前使用未来可能会被弃用的方法 `AddComponent<Turret>()`
7. 在 Turret 之上添加一个 Empty Game Object，并添加组件 TurretAuthoring。因为新版本需要使用 Authoring 把组件烘焙到 Entity 上。

# 8. 炮塔移动

1. 在 Scripts/Components 下创建一个 Tank。
2. 在 Scripts/Authoring 下创建一个 TankAuthoring。
3. 在 Tank 上添加 TankAuthoring 脚本。
4. 在 Scripts/System 下创建 TankMovementSystem。

# 9. 制作炮弹

1. 在 Scripts/Componets 下创建 CannonBall。

2. 在 Scripts/Authoring 下创建 CannonBallAuthoring。

3. 在 SampleScele 右击 GameObject -> 3D Object -> Sphere 命名为 CannonBall。Position(0, 0, 0), Rotation(0, 0, 0), Scale(0.2, 0.2, 0.2)。Sphere Collider -> Off。添加组件 CannonBallAuthoring。最后拖入 Prefabs 目录转换为 Prefab，并删除场景上的 CannonBall。

4. 打开 Scripts/Components 里的 Turret.cs，添加两个属性：

   ```csharp
   public Entity CannonBallSpawn;
   public Entity CannonBallPrefab;
   ```

5. 打开 Scripts/Authoring 里的 TurretAuthoring.cs，在 MonoBehaviour 中添加两个属性，并在 Baker 的 AddComponent 时将这二个属性传给 Turret。

6. 此时 TurretGroup 上就有了二个属性需要指定，CannonBallPrefab 指定 Prefab 目录下的 CannonBall，CannonBallSpawn 指定场景中的 SpawnPoint。

7. 在 Scripts/Aspects 下创建 TurretAspect 脚本。Aspect 常用来将多个 Componet 合在一起作为一个捷径来存取。

8. 在 Scripts/System 下创建 TurretShootingSystem。

# 10. 炮弹移动

1. 在 Scripts/Aspects 下创建 CannonBallAspect。
2. 在 Scripts/System 下创建 CannonBallSystem。

# 11. 生产炮塔

1. 把整个 Tank 拉到 Prefabs 目录下创建 prefab。并删除场景中的 Tank 物件。
2. 在 Scripts/Components 下创建 Config，用来设置 TankPrefab, TankCount 和 SafeZoneRadius。
3. 在 Scripts/Authoring 下创建 ConfigAuthoring。
4. 在 EntityScene 上右击，GameObject -> Create Empty，命名为 Config，并将 ConfigAuthoring 添加到物体上。并将 TankPrefab 和其他几个属性赋上正确的值。
5. 在 Scripts/System 下创建 TankSpawningSystem。
6. 修改 Scripts/System 的 TankMovementSystem，让每个 Tank 的 y 应用上 Entity.Index，让它们的位置和移动错开。

# 12. 指定颜色

1. 给 Tank Prefab 中的 Tank, Turret, Cannon 都加上 URPMaterialPropertyBaseColorAuthoring 组件。
2. 修改 TankSpawningSystem，让每个 Tank 有随机的颜色。
3. 修改 CannonBallAuthoring，添加 `AddComponent<URPMaterialPropertyBaseColor>();`，让每个 CannonBall 的 Entity 上也可以调整颜色。
4. 修改 TurretAspect ，也加上颜色属性。（因为 TurretAuthoring 添加在了 TurretGroup 这个空对象上，所以这个对象上也要添加 URPMaterialPropertyBaseColorAuthoring 组件）
5. 修改 TurretShootingSystem。

# 13. 安全范围

1. 在 Scripts/Components 下创建 Shooting。
2. 修改 TurretAuthoring，添加 Shooting 组件。
3. 在 Scripts/Systems 下创建 SafeZoneSystem。
4. 修改 TurretShootingSystem。

（Shoot | Turret 并不在 Tank 上无法获得 Transform，目前无法实现根据位置开关 Shooting）

# 14. 摄像头跟随

1. 在 Scripts/MonoBehaviours 下创建 CameraSingleton。
2. 在 Main Camera 上添加 CameraSingleton 组件。
3. 在 Scripts/Systems 下创建 CameraSystem。

