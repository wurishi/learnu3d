Unity 2022.3.16f1

# 1. 火焰特效

## 1.1 环境

- 使用 3D URP 模板创建项目
- 安装 `Visual Effect Graph` 和 `Shader Graph`。
- Edit -> Preferences -> Visual Effects -> Experimental Operators/Blocks -> On

## 1.2 火焰

1. 创建一个空对象，命名为 vfx_fire_tut，并 Reset Transform。
2. 在 Project 右击 Create -> Visual Effects -> Visual Effect Graph，命名为 vfxgraph_fire。将其拖到 vfx_fire_tut 下，并将位置设置为 (0, 0, 0)。
3. 选中场景中的 vfxgraph_fire 在 Inspector 面板中点击 Asset Template 的 Edit 按钮进入编辑模式。（在 Shader Graph 界面点击 F 可以快速将内容放到可视区域的中心）
4. 可以手画几张texture或直接使用默认的texture。
5. vfxgraph_fire 添加 Color 属性，名称为 Color。值为 HSV(5, 91, 98)，Intensity 强度为 5。
6. 添加 Texture2D 属性，名称为 FlameTexture。Value 指定为 4. 的 texture 或随便一张 texture。
7. 将 FlameTexture 拖出来并连接到 Output Partical Quad 的 Main Texture 上。
8. 在 Project 右击 -> Create -> Shader Graph -> Blank Shader Graph，命名为 FireShader。
9. 双击打开 FireShader，Graph Inspector -> Active Targets -> 添加一个 Visual Effect -> Material -> Unlit。
10. FireShader 添加 Color 属性，名为 Color。选中 Color 在 Graph Inspector 中：
    - Mode -> HDR
    - Color -> White
11. 添加 Texture2D 属性，名为 MainTexture。
12. 将 MainTexture 拖出，并连接一个新节点 Sample Texture2D。
13. 将 Color 拖出，并连接一个新节点 Multiply: A。并将上一步的 Sample Texture2D 节点的 Texture 连接到 Multiply 的 B 上。
14. 将 Multiply 的 Out 连接到 Fragment 的 BaseColor 上。
15. 将 Sample Texture2D 节点的 Texture 再连接到 Fragment 的 Alpha 上可以去除黑色背景。
16. 回到 vfxgraph_fire ，在 Output Particle Quad 上将 Shader Graph 指定到 FireShader。
17. 将 Color 拖出并连接到 Output Particle Quad 的 Color 上。
18. **早期版本**：FireShader 中属性的 Reference 名字会有很长的后缀，可以选中属性并在 Graph Inspector 中修改。
19. **溶解效果**：在 FireShader 中将 MainTexture 的默认 texture 指定一下。
20. 将 Sample Texture2D 的 Texture 连接到新的节点 Multiply: A。
21. 创建新节点 Voronoi，将 Out 连接到新节点 Power: A 上。
22. 将 Power 的 Out 连接到 20. 的 Multiply 的 B 上。再将 20. Multiply 的 Out 连接到 13. 的 Multiply 的 B 上覆盖原有。
23. 再将 20. Multiply 的 Out 连接到 Fragment 的 Alpha 上覆盖原有。
24. **火的运动**：在 vfxgraph_fire 中修改 Initialize Particle -> Set Velocity Random (Per-component)：
    - A (-0.1, 0.2, -0.1)
    - B (0.1, 1, 0.1)
25. 按 Space 键创建 Set Angle 的 block。设置 Channels -> Z，并在 Inspector 中设置 Random -> Uniform。将在 block 中设置 A -> 360，B -> -360。
26. 设置 Lifetime Random 的 AB 值为 0.8-1.6，让火不会飞的太高。
27. 在 Output Particle Quad 创建 Set Size block。Random -> Uniform，AB 值为 0.6 - 2。
28. 由于 Set Size over Life 覆盖了 Set Size，所以目前大小不会有变化。选中 Set Size over Life 在 Inspector 中设置 Composition -> Multiply。并将曲线调整为由大到小。
29. 取消 Set Color over Life，我们将使用溶解效果进行淡入淡出。
30. 在 FireShader 中，添加 Float 属性，名为 DissolvePower，Default 为 1。并连接到 Power 的 B 上。
31. 添加 Float 属性，名为 DissolveScale，默认值为 6。并连接到 Voronoi 的 CellDensity 上。
32. 添加 Float 属性，名为 DissolveSpeed，默认值为 0.5。拖出并连接到新节点 Multiply: A 上。
33. 创建 Time 节点，将 Time 连接到 32. 的 Multiply 的 B 上。并将 Multiply 的 Out 连接上 voronol 的 AngleOffset 上。
34. 在 vfxgraph_fire 中，创建 Sample Curve 节点。编辑曲线，左下右上的曲线，最左点 Time 在 0.23 左右，最右点右击 Edit Key，Value 设置为 30。并连接到 Output Particle Quad 的 Dissolve Power 上。
35. 创建 Age Over Lifetime 节点，并连接到 Sample Curve 的 Time 上。此时可以调节曲线，观察效果。
36. 将所有节点选中 Group 命名为 FLAMES。
37. 创建 float 属性 FlamesRate，默认值为 16。 连接到 Spawn system 的 Rate 上。
38. 创建 Vector3 属性 FlamesVelocity，默认值为(0.1, 1, 0.1)，连接到 Initialize Particle -> Set Velocity Random 的 B 上。
39. 创建 float 属性 FlamesLifetime，默认值为 1.6 ，连接到 Initialize Particle -> Set Lifetime Random 的 B 上。
40. 创建 float 属性 FlamesSize，默认值为 2，连接到 Output Particle Quad -> Set Size Random 的 B 上。
41. FlamesSize 再连接到新的节点 Multiply上，Multiply B 设置 0.4。再将整个 Multiply 连接到 Set Size Random 的 A 上。

## 1.3 烟雾

1. 在 vfxgraph_fire 中，将整个 FLAMES group 复制一份，并命名为 SMOKE。
2. 将连接到 Output Particle Quad -> Color 的 Color 节点删除，手动设置为黑色。
3. 删除 FlamesSize 和相关的 Multiply 的节点。Set Size Random 的 AB 设置为 1.2 - 3。
4. 删除 FlamesLifetime 节点，Set Lifetime Random 的 AB 设置为 1.2 - 2.6。
5. 在 Project 选中 vfxgraph_fire，在 Inspector 中调整 Output Render Order 的顺序，让 SMOKE 先渲染。可以修改 FLAMES 和 SMOKE 的 Output Particle Quad 的名字这样在 Order 中观察起来更直观。
6. 创建 SmokeVelocity，SmokeRate，SmokeLifetime，SmokeSize 等属性和 Flames... 相同用来控制烟雾的各种属性。
7. 修改 FLAMES 和 SMOKE 的 Initialize Particle 的 Capacity 值为 1000。

## 1.4 火星飞舞

1. 在 vfxgraph_fire 中，将整个 FLAMES group 复制一份，并命名为 AMBERS。
2. 删除 FlameTexture 节点，手动指定 Main Texture 为默认的 Default-Particle。
3. 删除 FlamesSize 节点，设置 Set Size Random 的 AB 为 0.05 - 0.2。
4. 删除 FlamesLifetime 节点，Set Lifetime Random 的 AB 设置为 1.6 - 3.2。
5. 删除 FlamesVelocity 节点，Set Velocity Random 的 AB 的 y 设置为 0.4 和 1.6。
6. DissolveScale 设置为 2。删除 FlamesRate 节点，Spawn 的 Rate 为 30。

## 1.5 灯光系统

1. 在场景的 vfx_fire_tut 下创建 Effects -> Particle System ，命名为 LIGHT。
2. Particle System：
   - Start Speed -> 0
   - Emission：
     - Rate over Time -> 0
     - Bursts -> Add -> Count -> 1
   - Shape -> Off
   - Lights -> On
3. 在场景创建一个 Light -> PointLight。并将该 PointLight 拖为预制体，然后删除场景中的 Point Light。
4. Particle System -> Lights
   - Light -> 3. 创建的预制体灯光。（需要将预制体拖入 Light）
   - Ratio -> 1
   - Range Multiplier -> 10
   - Intensity Multiplier -> 10
5. Particle System -> Renderer -> Render Mode -> None。
6. Particle System -> Start Color -> 设置一个符合的颜色。
7. Particle System -> Color over Lifetime -> On。修改 Color，设置多个点，每间隔一个点调整 Alpha 透明度。
8. Emission -> Bursts -> Cycles -> Infinite，Lights -> Maximum Lights -> 1。可以解决每次 loop 第一帧闪烁的问题。

# 2. 风格化拖尾特效

## 2.1 初始化

- 使用 3D URP 模板创建项目
- 安装  `Shader Graph`。
- 创建一个 3D Object -> Plane，Position (0, 2, 0), Rotation (-90, 0, 0), Scale (1, 1, 0.33)。

## 2.2 开始

1. 目录下 Create -> Shader Graph -> Blank Shader Graph，命名为 TrailShader。
2. 双击打开，在 Graph Inspector -> Graph Settings -> Target Settings -> Active Targets 新建一个 Universal。
   - Surface Type -> Transparent
   - Blending Mode -> Alpha
   - Render Face -> Both
   - Alpha Clipping -> On
3. 创建两个 Color 属性，分别命名为 Color1, Color2。Mode 都为 HDR。Color1 红色，Color2 蓝色。
4. 新建 Lerp 节点。Color1 连接到 A，Color2 连接到 B。
5. 新建 UV 节点，连接新节点 Split，R 连接新节点 One Minus，Out 连接到 4. Lerp 的 T 上。产生渐变效果。
6. Lerp 连接新节点 Multiply: A。
7. 创建 Texture2D 属性，名为 MainTex，Default Texture 指定一张图片。
8. 拖出 MainTex，连接到新节点 Sample Texture2D。RGBA 连接到 6. Multiply 的 B 上。
9. 新建 Time 节点，Time 连接新节点 Multiply: A。
10. 创建 Vector2 属性为 MainTexSpeed。X 默认值为 -0.1。并将它连接到 9. Multiply 的 B 上。
11. 将 9. Multiply 连接新节点 Tiling And Offset，Out 连接到 Sample Texture 2D 的 UV 上。实现滚动。

## 2.3 拖尾未端淡出

1. 新建 UV 节点，连接新节点 Split，R 连接新节点 Subtract: B 上。Subtract A 属性设置为 0.76。
2. Subtract 连接新节点 Multiply: A。Sample Texture 2D 的 RGBA 连接到这步创建的 Multiply 的 B 上。再将 Multiply 连接到 2.2.6 创建的 Multiply 的 B 上覆盖原有。

## 2.4 嗓音

1. 新建 Sample Noise 节点。
2. 创建 Float 属性 DissolveScale，默认值 35。并连接到 Simple Noise 的 Scale 上。Noise 的 Out 连接到 2.3.1 Subtract 的 A 上。
3. 将 2.3.1 的 Split 的 A 属性连接新节点 One Minus，Out 连接新节点 Add: B。
4. Sample Noise 连接到 Add 的 A 上，再将 Noise 的 Out 连接到 2.3.1 Subtract 的 A 上覆盖原有。(原来删去了太多纹理，通过 One Minus 反向操作后变成保留大部分纹理)
5. 新建 Time 节点，Time 连接新节点 Multiply: A。
6. 创建 Vector2 属性为 DissolveSpeed，默认值 X 为 -0.5。并连接到 5. Multiply 的 B 上。
7. 将 5. Multiply 连接新节点 Tiling And Offset: Offset 上。再连接到 Simple Noise 的 UV 上。
8. 最后将 2.2.6 的 Multiply 连接到 Fragment 的 BaseColor 上。

## 2.5 测试

1. 在 Project 中 Create -> Material 命名为 TrailMaterial。
2. 将 TrailShader 拖到 TrailMaterial 上。
3. 将 TrailMaterial 拖到场景中的 Plane 上。
4. 将 2.2.6 Multiply 连接新节点 Clamp: In。将 Clamp 连接到 Fragment 的 Alpha 上。
5. 用 Photoshop 画素材~~~（需要左右贴合，有一定透明度的不规则波浪线）

## 2.6 Trail

1. 在场景中创建 Effects -> Trail。
   - Time -> 1
   - Materials[0] -> TrailMaterial
2. 修改 TrailShader 的 Graph Inspector -> Universal -> Material -> Unlit，使光照更合适。
3. 复制一个场景中的 Trail，并作为原 Trail 的 child。复制的 Time 为 1.5。
4. 复制两个 TrailMaterial，并赋给 Trail 和 Trail(1)。
5. TrailMaterial 1 的 Color 1 选择橙色，TrailMaterial 2 的 Color 1 选择更暗的橙色，Color2 选红色。
6. Trail(1) 的 Width 调大一些（2），并将 Trail -> Additional Settings -> Order in Layer -> 1。

## 2.7 发光点

1. 安装 `Visual Effect Graph`。
2. 在 Project 下 Create -> Visual Effects -> Visual Effect Graph。命名为 vfx_TrailParticles，并拖到场景的 Trail 下，Position (0, 0, 0)
3. 编辑 vfx_TrailParticles，Spawn -> Rate -> 8。
4. Initialize Particle：
   - 删除 Set Velocity Random
   - Set Lifetime Random 的 AB 设置为 0.4 和 0.3。
5. Output Particle Quad -> Main Texture -> Default-Particle。
6. Output Particle Quad 创建 Set Size，Size -> 1。
7. Output Particle Quad -> Set Size over Life -> Composition -> Multiply。曲线调整为左上右下的弧线。
8. Output Particle Quad 创建 Set Color。
9. 创建 Color 属性为 Color，默认值为橙色，并连接到 8.Set Color -> Color 上。
10. Output Particle Quad -> Set Color over Life
    - Composition -> Multiply
    - Alpha Composition -> Multiply

## 2.8 光源

1. 在场景的 Trail 下创建 Light -> Point Light。
2. Point Light 的颜色选择橙色，Intensity -> 2.5。

## 2.9 更多粒子

1. 在 vfx_TrailParticles 将之前所有节点 Group 命名为 PARTICLES。
2. 复制一份 PARTICLES 命名为 PARTICLES_SMALL。
3. 在 PARTICLES_SMALL -> Initialize Particle 下创建 Set Position(Sphere)，它的 Arc Sphere -> Sphere -> Radius -> 0.1。
4. Initialize Particle -> Capacity -> 1000
5. Spawn -> Rate -> 32
6. 在 Update Particle 下创建 Turbulence：
   1. Intensity -> 10
   2. Frequency -> 2
7. Output Particle Quad -> Set Size -> Random -> Uniform，AB 值为 0.05 - 0.25。
8. 创建 Color 属性为 ParticlesColor，颜色为更加强的红色。并连接到 Set Color -> Color，覆盖原有。
9. 点击 Initialize Particle 右上角的 LOCAL，切换为 WORLD。

# 3. 箭类特效 (未完成)

## 3.1 初始化

- 创建一个箭头类的 FBX 模型，命名为 Arrow。修改 ScaleFactor 为 100 并Apply。
- 在场景 Create Empty，命名为 vfx_ArrowAttack。并 Reset Transform。

## 3.2 开始

1. 在 Project 里创建新的 Visual Effect Graph，命名为 vfx_graph。并将其作为子物体拖到场景上 vfx_ArrowAttack 下面。
2. 编辑 vfx_graph，删除 Output Particle Quad，将 Update Particle 连接新的 Output Particle Mesh 节点。Mesh 指定为箭头。
3. 删除 Initialize Particle 中的 Set Velocity Random，Set Lifetime -> Random -> Off。
4. 创建 float 属性为 ArrowLifetime，默认值为 3。并连接到 Set Lifetime -> Lifetime。
5. Initialize Particle -> Bounds Mode -> Manual。
6. 删除 Spawn 的 Constant Spawn Rate，创建 Single Burst，Count -> 1。
7. 在 Output Particle Mesh 中创建 Set Size，Size -> 0.5。
8. 再创建 Set Scale 。
9. 再创建 Set Angle，将箭头调整为合适的方向。
10. 在 Update Particle 中创建 Add Angle，X -> 1。

## 3.3 箭头

1. 创建从上到下的黑白渐变的材质，命名为 Gradient。
2. Output Particle Mesh -> MainTexture 设置为 1. Gradient。
3. 在 Output Particle Mesh 中创建 Set Color，Color 为淡蓝色，Intensity -> 1。
4. Output Particle Mesh -> Blend Mode -> Additive。（非必需，Alpha 也可以）
5. 将 Output Particle Mesh 命名为 ARROW。
6. 创建 Simple Particle System。它的 Output Particle Quad 命名为 STRETCHED PARTICLES。
   - Blend Mode -> Additive
   - MainTexture -> Default-Particle
7. Orient -> Mode -> Alone Velocity。
8. 创建 Set Size, Random -> Uniform。AB 为 0.1 - 0.5。
9. 创建 Set Scale，Random -> Uniform。AB 为 (0.1, 0.5, 1) - (0.2, 1, 1)。
10. Set Size over Life 的 Size 曲线可以是从大到小的弧线。Composition -> Multiply。
11. Set Color over Life 的 Composition -> Multiply。
12. 创建 Set Color，颜色为蓝色，Intensity -> 1。
13. Initialize Particle 的 Set Velocity Random 的 AB 为 (-0.2, -0.2, -2) - (0.2, 0.2, -10)。
14. 创建 Set Position (Sphere)。

## 3.4 烟雾 (需要素材)

1. 再创建 Simple Particle System，Output Particle Quad 命名为 SMOKE。

# 4. 召唤生物类特效

## 4.1 初始化模型和动画

1. 一个动物类模型，只需要一个 Run 动画，Inspector -> Animation -> Loop Time -> On
2. 场景创建空对象，命名为 vfx_Attack。并 Reset Transform。
3. 将动物模型作为 vfx_Attack 的子级拖入。
4. 如果没有动画，选择模型，Inspector -> Rig -> Avatar Definition -> Create From This Model。
5. 在 Project 中创建 Animator Controller，并拖到模型上。
6. 编辑 Animator Controller，右击 -> Create State -> Empty。Motion 指定 Run 动画。

## 4.2 菲涅尔效果

1. Project -> Create -> Shader Graph -> Blank Shader Graph。命名为 XxxShader。
2. 编辑 1.Shader，Graph Inspector -> Graph Settings -> Target Settings -> Active Targets -> Universal。
   - Material -> Unlit
   - Allow Material Override -> On
3. 创建 Fresnel Effect 节点，创建 Float 属性 FresnelPower，默认值为 1，并连接到 Fresnel Effect -> Power。
4. 创建 Color 属性 Color，Mode -> HDR，Default -> 白色，Alpha -> 100。并连接新节点 Multiply: A。
5. 将 3.Fresnel Effect 连接到 4.Multiply 的 B。
6. 将 4.Multiply 连接到 Fragment 的 BaseColor。
7. 创建 Material，命名为 HorseMat。将 HorseShader 拖到材质上。（或者直接右击 HorseShader 创建材质）
8. 选择场景中的 Horse，将 Skinned Mesh Renderer -> Materials 替换为 HorseMat。
9. 修改 HorseMat 的颜色为紫色，Intensity 适当加强。
10. 将整个 Horse 作为预制体保存一份到 Project 中。

## 4.3 腐蚀效果

1. 回到 HorseShader 编辑界面，创建 Simple Noise 节点。Scale -> 30。并连接到 Fragment 的 Alpha。
2. Graph Inspector -> Graph Settings -> Universal -> Alpha Clipping -> On。
3. 创建 Float 属性 Erode，Mode -> Slider，Min - Max 为 0 - 1。并连接到 Fragment 的 Alpha Clip Threshold。
4. 修改 HorseMat -> Alpha Clipping -> On。
5. 创建 C# 脚本，命名为 HorseAttack。并添加给场景中的 Horse。（脚本中添加了 ErodeObject 别忘了指定）
6. 创建 FPSCharacter。
7. Horse 的预制体添加 Rigidbody 组件

## 4.4 粒子效果

1. 创建 Visual EffectGraph，命名为 vfx_AttackEffect。并拖入场景，作为 vfx_Attack 的子物件。位置和动物模型居中。
2. 编辑 vfx_AttackEffect，将 Initialize Particle 的 LOCAL 修改为 WORLD。
3. 创建 Set Position (Sphere)，Arc Shpere 的 W 修改为 Local，Radius -> 0.1。
4. Spawn system -> Loop Duration 和 Loop Count 都设置为 Constant。
5. 创建 float 属性 Duration。默认值 1.5。并连接到 Spawn -> Loop Duration。
6. Spawn -> Rate -> 250，Initialize Particle -> Capacity -> 10000。
7. Output Particle Quad 创建 Set Size，Random -> Uniform，AB 设置为 0.008 - 0.04。
8. Set Size over Life，曲线调整上左上右下的弧线。Composition -> Multiply。
9. MainTexture -> Default-Particle。
10. 创建 Set Color，颜色为紫色，Intencity 适当加强。并禁用 Set Color over Life。
11. Set Velocity Random 的 AB 设置为 (-0.5, -0.5, -0.5) - (0.5, 0.5, 0.5)。
12. Update Particle 创建 Turbulence。
13. 创建 Random Number 节点，Min -> -5，Max -> 5，Constant -> Off。并连接到 Turbulence -> Intensity。

# 5. UV 动态扭曲

## 5.1 开始

1. Create -> Shader Graph -> URP -> Unlit Shader Graph。
2. Graph Inspector
   - Universal -> Surface Type -> Transparent
   - Allow Material Override -> On
3. 创建 Texture2D 属性 MainTexture，连接新节点 Sample Texture2D。
4. Sample Texture2D 的 RGBA 连接新节点 Multiply: B。
5. 创建 Color 属性 Color，连接到 4.Multiply 的 A。
6. 将 4.Multiply 连接新节点 Multiply: A。
7. 创建 Vertex Color 节点连接到 6.Multiply 的 B。
8. 将 6.Multiply 连接到 Fragment 的 Base Color。
9. 再将 6.Multiply 连接新节点 Split。将它的 R 连接到 Fragment 的 Alpha 上。(如果是透明图可以选用 A 连接到 Alpha 上)
10. 创建 Vector2 属性 MainSpeed。并连接新节点 Multiply: A。
11. 创建 Time 节点，将 Time 连接 10.Multiply -> B。
12. 创建 UV 节点，并连接新节点 Add: B。
13. 将 10.Multiply 连接到 Add -> A。
14. 将 Add 连接到 Sample Texture 2D  -> UV。
15. 此时调整 MainSpeed 就可以控制材质的滚动。

## 5.2 扭曲

1. 创建 Lerp 节点，将 UV 连接到 Lerp -> A。
2. 创建 Float 属性 DistortionAmount，Mode -> Slider, Default -> 0.1(根据素材调整), Min -> -1, Max -> 1，并连接到 Lerp -> T。（Min Max 一般推荐 -0.1 - 0.1 或 0.2）
3. Lerp 连接到 Add -> B 上覆盖原有。
4. 创建 Vector2 属性 NoiseSpeed，连接新节点 Multiply: B。
5. 将 Time -> Time 连接到 5.Multiply -> A。
6. 将 5.Multiply 连接新节点 Add: B。
7. UV 连接到 7.Add -> A。
8. 将 7.Add 连接新节点 Gradient Noise: UV。
9. 将 Gradient Noise 连接 Lerp -> B。
10. 创建 Float 属性 NoiseScale，默认值为 10。并连接 Gradient Noise -> Scale。

# 6. 顶点动画

## 6.1 改变顶点

1. 创建 Lit Shader Graph，命名为 Animation_PBR。Allow Material Override -> On。
2. 创建 Lerp 节点，并连接到 Vertex -> Position。
3. 创建 Position 节点，Space -> Object。并连接到 Lerp -> A。
4. 再次创建 Position 节点，Space -> Object。并连接新节点 Split。
5. Split -> R 连接新节点 Add: B。并将 Add 连接到 Lerp -> B。
6. Add 再连接新节点 Combine: R。
7. 将 Split 的 G 和 B 分别连接到 Combine 的 G 和 B 上。
8. 将 Combine -> RGB 连接到 Lerp -> B 覆盖原有。
9. 创建 Float 属性 WobbleSpeed，默认值为 0.5。
10. 创建 Time 节点，Time 连接新节点 Multiply: A。
11. WobbleSpeed 连接到 Multiply -> B。
12. 创建 Position 节点，并连接新节点 Split。
13. 将 12.Split -> G 连接新节点 Multiply: A。
14. 创建 Float 属性 WobbleFrequency，默认值为 2。并连接到 13.Multiply -> B。
15. 将 13.Multiply 连接新节点 Add: B。将 10.Multiply 连接到 Add -> A。
16. 将 15.Add 连接到 5.Add -> A。
17. 创建 Vector3 属性连接到 Lerp -> T。
18. 将 15.Add 连接新节点 Sine，并连接新节点 Multiply: A。
19. 创建 Float 属性 WobbleDistance，默认值 0.5。并连接 18.Multiiply -> B。
20. 将 18.Multiply 连接 5.Add -> A。
21. 创建 Float 属性 WobbleAmount，Mode -> Slider。并连接到 Lerp -> T。最后删除原来的 Vector3 属性。

# 7. 球形畸变

## 7.1 初始化

- 使用 3D URP 模板创建项目
- 安装  `Shader Graph` 和 `Visual Effect Graph`

## 7.2 创建 Trails

1. 在 Project 中 Create -> Visual Effects -> Visual Effect Graph，命名为 MagicOrb。
2. 拖到场景上，并点击编辑，全选删除所有默认节点。
3. 按 Space 键，创建 Simple Heads And Trails。
4. 调整初始参数
   - Spawn -> Rate -> 50 (如果不调整 Capacity 是看不出效果的)
   - Initialize Particle -> Capacity -> 1000
   - Initialize Particle Strip -> Strip Capacity -> 1000
5. 创建 float 属性 TrailsSpawnRate，默认值 30。连接到 Spawn 的 Rate 上。
6. 删除 Initialize Particle 下的 Set Velocity Random 和 Set Color Random from Gradient。
7. 设置单一颜色，在 Initialize Particle 下按 Space 键，创建 Set Color block。
8. 创建 Color 属性 Color，连接到 Initialize Particle -> Set Color -> Color。设置为蓝色并增加 Intensity （4左右）
9. 在 Initialize Particle 下创建 Set Position (Sphere) block。
10. 创建 float 属性 Size，默认值 1。
11. Size 属性连接新节点 Multiply，Multiply: B 设置为 1。Multiply 连接到 Set Position (Sphere) -> Arc Sphere -> Sphere -> Radius。
12. 创建 float 属性 TrailsLifetime，默认值为 1 。
13. TrailsLifetime 属性连接新节点 Multiply，Multiply: B 为 1。并连接到 Set Lifetime Random -> A。
14. TrailsLifetime 属性连接新节点 Multiply，Multiply: B 为 3。并连接到 Set Lifetime Random -> B。

## 7.3 移动

在 Update Particle 中，Turbulence （湍流/旋涡）用来控制移动颗粒。

1. 在 Update Particle 创建 Conform to Sphere block。
2. Size 属性拖一个新的连接新节点 Multiply，Multiply: B 为 1.5。该 Multiply 连接到 Conform to Sphere -> Sphere -> Radius。
3. Conform to Sphere
   - Attraction Force -> 10
   - Stick Force -> 5
4. Initialize Particle Strip 中 删除 Set Lifetime。创建 Inherit Source Lifetime 代替。
5. Output Particle Strip Quad 中添加 Set Size block，并禁用 Set Size over Life。
6. 再次拖出 Size 属性，连接二个新节点 Multiply，B 分别为 0.001 和 0.02。
7. 创建新节点 Random Number，将 6. 创建的二个 Multiply 分别连接到 Min 和 Max 上，并禁用 Constant。并连接到 Output Particle Strip Quad -> Set Size -> Size。
8. 再次启用 Set Size over Lift，并在 Inspector 中设置 Composition -> Add。
9. 全选所有节点创建 Group，命名为 TRAILS。

## 7.4 波束

1. 创建 Empty Particle System。
2. 在 Spawn 中创建 Periodic Burst block，Count -> 1，Delay -> 1。
3. 在 Initialize Particle 中创建 Set Lifetime block，Lifetime -> 2。
4. 在 Output Particle Quad 中指定 MainTexture -> Default-Particle。
5. 在 Output Particle Quad 中创建 Set Size。
6. 再次拖出 Size 属性，连接新节点 Multiply，Multiply: B 为 10，然后连接到 Output Particle Quad -> Set Size -> Size。
7. 在 Output Particle Quad 中创建 Set Color block，拖出 Color 属性并连接到 Set Color -> Color。
8. 为了减弱 Color，将 7. 拖出的 Color 属性连接新节点 Divide，Divide: B 为 4，并连接 Output Particle Quad -> Set Color -> Color，覆盖原有。
9. 修改 Output Particle Quad -> Blend Mode -> Additive。
10. 在 Output Particle Quad 中创建 Add Color over Life，调整 Color，0% -> Alpha 0% ; 50% -> Alpha 100%; 100% -> Alpha 0%;。
11. 全选这一步的所有节点 group 命名为 BEAM。

## 7.5 光点

1. 创建新的 Simple Particle System。
2. Output Particle Quad：
   - Main Texture -> Default-Particle
   - Blend Mode -> Additive
3. Initialize Particle 中创建 Set Position (Sphere)。
4. 再次拖出属性 Size，连接新节点 Multiply，Muliply: B 为 2。连接到 Initialize Particle -> Set Position (Sphere) -> Arc Sphere -> Sphere -> Radius。
5. Initialize Particle -> Set Velocity Random 的 AB 设置为 (-0.5, -0.5, -0.5)，(0.5, 0.5, 0.5)。
6. Initialize Particle -> Capacity -> 100000
7. 创建 float 属性 SpawnRate，默认值为 5000，并连接到 Spawn -> Rate。
8. Output Particle Quad 创建 Set Size，禁用 Set Size over Life。
9. 创建 Random Number 节点，Min -> 0.01，Max -> 0.02。并连接到 Output Particle Quad -> Set Size -> Size。
10. 禁用 Set Color over Life，创建 Set Color。
11. 拖出 Color 属性连接到 Set Color -> Color。
12. 在 Update Particle 中创建 Turbulence 和 Conform to Sphere。
13. 再次拖出 Size 属性，连接新节点 Multiply，Multiply: B 为 2。并连接到 Update Particle -> Conform to Sphere -> Sphere -> Radius。
14. SpawnRate 属性默认值修改为 50000。
15. 创建 Get Attribute: position 节点，连接新节点 Add。
16. 创建 Total Time (Game) 节点，连接 15.Add 的 B。
17. 创建 Perlin Noise 2D 节点，将 15.Add 的 x 和 y 连接到 Coordinate 的 x 和 y。
18. 将 17. Perlin Noise 2D 的 Derivatives 连接新节点 Remap：
    - Old Range Min -> -6
    - Old Range Max -> 6
    - New Range Min -> -1
    - New Range Max -> 1
19. 将 18. Remap 的 x 和 y 连接到 Update Particle -> Sphere -> Center 的 x 和 y。
20. 将 18. Remap 的 x 和 y 连接到 Update Particle -> Turbulence -> Field Transform -> Position 的 x 和 y，并修改：
    - Intensity -> 5
    - Drag -> 5
    - Octaves -> 3

# 8. 冲击波型运动

## 8.1 开始

1. 创建 Unlit Shader Graph 命名为 Burst。
2. 编辑 Burst，Graph Inspector -> Graph Settings -> Universal：
   - Allow Material Override -> On
   - Cast Shadows -> Off
   - Surface Type -> Transparent
   - Render Face -> Both
3. 创建 Rounded Rectangle 节点。
4. 创建 Tiling And Offset 节点，并连接到 Rounded Rectangle -> UV。
5. 调整 Rounded Rectangle -> Width -> 1.3 左右，Tiling And Offset -> Offset -> Y -> 0.5 左右。让整块白色在底部并且左右撑满，且调整 Height 可以看到是从下到上盖满的效果。
6. 创建 UV 节点，连接到新节点 Lerp: A。
7. 创建 Simple Noise 节点，X -> 25。连接到 Lerp -> B。
8. 创建 Slider 节点，值为 0.5 左右。连接到 Lerp -> T。
9. 将 Lerp 连接到 Tiling And Offset -> UV。
10. 创建 Tiling And Offset 节点，Offset -> Y -> 0.55。并连接到 Simple Noise -> UV。
11. 创建 Time 节点，Time 连接到新节点 Multiply: A。
12. 将 11.Multiply 连接到 10.Tiling And Offset -> Offset。
13. 创建 Vector2 节点，y -> -0.2。并连接 11.Multiply -> B。
14. 创建 Vector2 节点，x -> 1，y -> 1。并连接 10.Tiling And Offset -> Tiling。
15. 将 5.Rounded Rectangle 连接到 Fragment -> Base Color。
16. 将 5.Rounded Rectangle 再连接新节点 Split。Split -> A 连接到 Fragment -> Alpha。（可能会完全透明，可以换成17）
17. 或者将 5.Rounded Rectangle 再连接一条到 Fragment -> Alpha。
18. 创建材质命名为 CylinderMat，并应用到场景上的 Cylinder 物体。

## 8.2 优化

1. 创建 Float 节点，默认值 1。连接到 5.Rounded Rectangle -> Height。
2. 右击 Float 节点，Convert To -> Property 转换为属性，命名为 Height。
3. 将 8.Slider 转换为属性，命名为 Distortion。
4. 创建 Float 节点，默认值 25。连接到 Simple Noise -> Scale。再转换为属性，命名 DistortionScale。
5. 将 14.Vector2 转换为属性，命名为 DistortionTiling。
6. 将 13.Vector2 转换为属性 DistortionSpeed。
7. 创建 Color 节点，Mode -> HDR，颜色为白色。连接到新节点 Multiply: A。
8. 将 5.Rounded Rectangle 连接到 8.2.7 Multiply -> B。
9. 将 8.2.7 Multiply 连接 Fragment -> Base Color 覆盖原有。
10. 将 7.Color 转换为属性。
11. 可以调整 DistortionTiling -> X -> -8 左右，DistortionScale -> 12 左右。来解决边缘不循环的问题。
12. 可以往场景添加 Volume，指定 Profile。Camera -> Post Processing -> On。增加高光。

## 8.3 增加颜色

1. 新增 Rounded Rectangle，UV 和 Width 使用原来的 Tiling And Offset 配置。
2. Height 连接新节点 Multiply: A。B -> X -> 0.7。然后连接到 8.3.1 Rounded Rectangle -> Height。
3. 将 8.1.3 Rounded Rectangle 连接新节点 Subtract: A， 8.3.1 Rounded Rectangle 连接到 Subtract -> B。
4. 将 Subtract 连接 8.2.7 Multiply -> B 覆盖原有。
5. 将 8.3.1 Rounded Rectangle 连接新节点 Multiply: A。
6. 创建 Color 节点，设置个其他颜色，并连接到 8.3.5 Multiply -> B。
7. 将 8.3.5 Multiply 连接新节点 Add: B，再将 8.2.7 Multiply 连接到 Add -> A。
8. 将 8.3.7 Add 连接到 Fragment -> Base Color 覆盖原有。
9. 创建 Float 属性 BottomColorHeight，默认值为 0.7。连接 8.3.2 Multiply -> B。
10. 将 8.3.6 Color 转换为属性 BottomColor。
11. 修改 CylinderMat -> Depth Write -> ForceEnabled，Alpha Clipping -> On。

# 9. 蛇形顶点螺旋动画

模型有特殊需求

# 10. 龙息术

## 10.1 开始

1. 场景中 Create Empty，命名为 vfx_DragonBreath 并 Reset Transform。
2. 创建 Visual Effect Graph，命名为 vfxgraph_DragonBreath，并且作为 vfx_DragonBreath 的子物件拖到场景中。
3. 编辑 vfxgraph_DragonBreath，删除 Constant Spawn Rate，创建 Single Burst，Count -> 1。
4. 删除 Set Velocity Random。Set Lifetime Random -> Random -> Off，Lifetime -> 2.5。
5. Update Particle 创建 Set Velocity。
6. 创建 AnimationCurve 属性 FireTrailMotion，曲线为左上右下，最后一帧为 0.3, 0.13。并连接新节点 Sample Curve。
7. 创建 Age Over Lifetime 节点，连接到 Sample Curve -> Time。
8. 创建 Float 属性 FireTrailSpeed，Value -> 13，连接新节点 Multiply。
9. Sample Curve 连接 Multiply -> B。Multiply 连接 Set Velocity -> Z。
10. Update Particle 创建 Trigger Event Rate，Rate -> 30。
11. 创建 Simple Heads And Trails，删除 Heads 部分，将 Trigger Event Rate 连接到剩下部分中的 GPUEvent。

## 10.2 拖尾

1. 在 Trail Bodies 中，Strip Capacity -> 64，Lifetime -> 1.5，删除 Turbulence。
2. 在 Output Particle Quad 添加 Set Size，Size -> 1.6。（Set Size 要在 Set Size over Life 上面）
3. Set Size over Life -> Size 曲线修改为从大到小的弧线，Composition -> Multiply。

## 10.3 火焰痕迹

1. 创建 Blank Shader Graph，命名为 DragonBreathShader。
2. 编辑 DragonBreathShader，GraphInspector -> Graph Settings -> Target Settings 添加 Visual Effect。
3. 创建 Color 属性 Color01 和 Color02，Texture2D 属性 MainTex。
4. Color01，Mode -> HDR，Default -> 黄色。
5. Color02，Mode -> HDR，Default -> 红色。
6. Color01 连接新节点 Lerp: A。Color02 连接 Lerp -> B。
7. 创建 UV 节点连接新节点 Split，Split -> R 连接 Lerp -> T。
8. Lerp 连接新节点 Multiply: A。
9. MainTex 连接新节点 Sample Texture 2D，Sample Texture 2D -> RGBA 连接 8.Multiply -> B。
10. 创建 Tiling And Offset 节点连接到 Sample Texture 2D -> UV。
11. 创建 Vector2 属性 MainTexTiling 和 MainTexSpeed。
12. MainTexTiling 的 XY 为 (1, 1)，连接到 Tiling And Offset -> Tiling。
13. 创建 Time 节点，Time 连接新节点 Multiply: A。
14. MainTexSpeed，X -> 0.1。并连接 13.Multiply -> B，13.Multiply 连接 Tiling And Offset -> Offset。
15. 将 8.Multiply 连接 Fragment -> BaseColor 和 Alpha。
16. 回到 vfxgraph_DragonBreath，删除最早创建的 Output Particle Quad。（不是 Trail Bodies 的）
17. Trail Bodies 的 Output ParticleStrip Quad -> Shader Graph -> DragonBreathShader，Blend Mode -> Alpha。
18. 在 DragonBreathShader，将 Sample Texture 2D 连接新节点 Multiply: A。新节点 Multiply 再连接 8.Multiply -> B 覆盖原有。
19. 创建 Voronol 节点，连接新节点 Power: A，Power -> B -> 4。
20. 创建 Float 属性 DissolveScale，X -> 3，连接 Voronoi -> CellDensity。
21. 创建 Tiling And Offset 节点连接 Voronol -> UV。
22. 创建 Time 节点，Time 连接新节点 Multiply: A。
23. 创建 Vector2 属性 DissolveSpeed，X -> -0.5，连接 22.Multiply -> B。
24. 将 22.Multiply 连接 21.Tiling And Offset -> Offset。
25. 将 19.Power 连接 18.Multiply -> B。
26. 创建 Float 属性 DissolvePower，X -> 2，连接 19.Power -> B。
27. 创建 UV 节点，连接新节点 Split，Split -> R 连接新节点 Multiply: B。
28. 将 19.Power 连接 27.Multiply -> A。
29. (倒置)将 27.Split 连接新节点 One Minus，One Minus 连接 27.Multiply -> B。将 27.Multiply 连接 18.Multiply -> B。
30. 编辑 vfxgraph_DragonBreath，Sample Curve 连接新节点 Compare，Condition -> Greater Or Equal，Right -> 0.17(Sample Curve 的最右边的最小值)。
31. Compare 连接新节点 Branch，True -> 30，Branch 连接 Trigger Event Rate -> Rate。
32. 将 GPUEvent 之前的所有节点合成组命名为 FIRE。

## 10.4 火焰烟雾

1. 将 FIRE 组复制一份，命名为 FLAMES。
2. 删除 FLAMES 中的 Single Burst，创建 Constant Spawn Rate block，Rate -> 32。
3. Spawn system -> Inspector：
   - Loop Duration -> Constant -> 1.7
   - Loop Count -> Constant
4. 将 Update Particle 的 Set Lifetime -> Random -> Uniform，AB 为 1 - 1.5。
5. 在 Update Particle 创建 Set Angle，Random -> Uniform。AB 的 Z 为 (360, -360)。
6. Update Particle 连接 Output Particle Quad。
7. 删除 Trigger Event Rate 和相关的节点。
8. 在 Output Particle Quad 创建 Orient: Face Camera Plane。
9. 创建 Set Size，Random -> Uniform，A -> 1.5，B -> 3.5。
10. 创建 Set Size over Life，Size -> 从小到大的曲线，从 0.35 左右开始。Composition -> Multiply。
11. Shader Graph -> DragonBreathShader。MainTex -> Default-Particle。
12. 创建 Sample Gradient，Gradient -> 第一个颜色 HSV(7, 95, 75)，Intensity -> 9，位置在 10%。第二个颜色 HSV(25, 75, 95)，Intensity -> 2，位置在 75%。并连接 Color01 和 Color02。
13. 创建 Age Over Lifetime 节点，连接 Sample Gradient -> Time。
14. 删除 FireTrailSpeed，原来连接的 Multiply -> A -> 20。
15. 创建 Sample Curve 节点，创建 Age Over Lifetime 节点连接 Sample Curve -> Time。
16. 将 15.Sample Curve -> Curve -> 第一个点 V -> 6.5，第二个点 -> 4，第三个点 -> 12。并连接 DissolvePower。
17. 创建 Random Number 节点，Min -> 2，Max -> 7。连接 Dissolve Scale。

## 10.5 小火焰

1. 复制 FLAMES，命名为 SMALL。
2. Constant Spawn Rate -> Rate -> 16。
3. Set Lifetime Random -> (0.5, 1)
4. 删除 Set Velocity。
5. 创建 Set Position，Position -> Z -> 0.5。
6. Random Number -> Min -> 3，Max -> 9。
7. Set Size Random -> A -> 1，B -> 3。

# 11. 风格化火焰

## 11.1 开始

1. 创建 Unlit Shader Graph 命名为 FlameShader，
   - Allow Material Override -> On
   - Main Preview -> Quad
   - Render Face -> Both
   - Surface Type -> Transparent
2. 创建 Texture2D 属性 MainTexture，Default -> Default-Particle。连接新节点 Sample Texture 2D。
3. Sample Texture 2D -> RGBA 连接 Fragment -> Base Color，A 连接 Fragment -> Alpha。
4. 创建 Color 属性 Color，橙色，Mode -> HDR。连接新节点 Multiply: A。
5. Sample Texture 2D -> RGBA 连接 Multiply -> B。
6. Multiply 连接 Fragment -> Base Color 覆盖原有。
7. 创建 Gradient Noise 节点。
8. 创建 Tiling And Offset 节点，连接 Gradient Noise -> UV。
9. 创建 Time 节点，Time 连接新节点 Multiply: B。
10. 创建 Vector2 节点，Y -> -0.2，连接 9.Multiply -> A。将这个 Multiply 连接 Tiling And Offset -> Offset。
11. 将 Gradient Noise 连接新节点 Lerp: B，创建 UV 节点连接 Lerp -> A。
12. 创建 Float 属性 DistortionAmounts，X -> 0.1，连接 Lerp -> T。
13. Lerp 连接 Sample Texture 2D -> UV。

## 11.2 溶解

1. 复制 Vector2 -> Multiply -> Tiling And Offset 三个节点。Time -> Time 连接到新复制的 Multiply -> B。
2. 创建 Voronol 节点，将 11.2.1 Tiling And Offset 连接到 Voronol -> UV。
3. Vector2  -> (-0.1, -0.5)。
4. Voronol -> Out 连接新节点 Power: A。
5. Power 连接新节点 Multiply: B。Gradient Noise 连接 Multiply -> A。
6. 将 5.Multiply 连接新节点 Multiply: B，再将 Sample Texture 2D -> RGBA 连接 Multiply -> A。
7. 将 11.2.6 Multiply 连接 11.1.4 Multiply -> B 覆盖原有。
8. 创建 FlameShader 对应的 FlameMat 材质。
9. 在场景创建 Quad，并将 FlameMat 赋于给它。

## 11.3 创建属性

1. 将 11.1.10 Vector2 转换成属性 DistortionSpeed。11.2.3 Vector2 转换成属性 DissolveSpeed。
2. 创建 Float 属性 DissolveScale，X -> 2。连接 Voronol -> CellDensity。
3. 创建 Float 属性 DistortionScale，X -> 5。连接 Gradient Noise -> Scale。
4. 创建 Float 属性 DissolveAmounts，X -> 1.2。连接 Power -> B。