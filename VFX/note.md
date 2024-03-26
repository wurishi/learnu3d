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

# 3. 箭类特效

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

## 3.4 烟雾

1. 再创建 Simple Particle System，Output Particle Quad 命名为 SMOKE。
2. Main Texture -> Smoke8X8，Uv Mode -> FlipBook Blend，Flip Book Size -> 8x8。
3. 创建 Set Tex Index over Life，Tex Index -> 从小到大的直线，最后一帧 Value -> 63。
4. St Size over Life -> 从大到小抛物线，最后一帧 Value -> 0.7左右。
5. Set Color over Life -> Color -> Alpha  调整淡入淡出，Composition 和 Alpha Composition -> Multiply。
6. 创建 Set Color，Color -> (0.45, 0.66, 0.75)。Intensity 加强。
7. Blend Mode -> Additive。
8. 创建 Set Size，Random -> Uniform，AB -> (1.2, 2.6)。Set Size over Life，Composition -> Multiply。
9. 创建 Set Angle，Random -> Uniform，AB 的 Z -> -360 到 360。
10. Set Velocity Random -> (-0.2, -0.2, -1) - (0.2, 0.2, -4)。Set Life Random -> B -> 2.5。
11. 创建 Set Alpha，Random -> Uniform，AB -> (0.1, 0.4)。
12. Spawn -> Loop Duration 和 Loop Count -> Constant，ArrowLifetime 连接新节点 Multiply，Multiply -> B -> 0.95，连接 Loop Duration。
13. 对 STRETCHED PARTICLES 的 Spawn 作相同操作。

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

# 12. 钻石发光效果

## 12.1 开始

1. 创建 Blank Shader Graph，命名为 CrystalShader。添加 Universal -> Lit。
2. 创建 Float 属性 Metallic 和 Smoothness，Mode -> Slider。并连接到对应的 Fragment 的属性上。
3. 创建 Color 属性 BaseColor，白色，Mode -> HDR。并连接 Fragment -> Base Color。
4. 创建 Fresnel Effect 节点，连接 Fragment -> Emission。
5. 创建 Color 属性 TopColor 和 BottomColor，Mode -> HDR，都为白色。
6. TopColor 连接新节点 Multiply: A。Fresnel Effect 连接 Multiply -> B。Multiply 连接 Fragment -> Emission 覆盖原有。
7. 创建 CrystalMat 材质调整 Metallic -> 0.54，Smoothness -> 0.12，BaseColor 和 BottomColor。（需要 Volume -> Bloom，Intensity 加强才有效果）

## 12.2 底部发光

1. 创建 UV 节点，连接新节点 Split。Split -> G 连接新节点 Multiply: A。Multiply -> B -> 0.8。Multiply 连接新节点 Smoothstep: Edge1，Smoothstep -> In -> 0.4。
2. 将 12.2.1 Multiply 再连接新节点 One Minus，One Minus 连接新节点 Smoothstep: Edge1， Smoothstep -> In -> 0.5。
3. 创建 Float 属性 TopLine 和 BottomLine。TopLine -> 0.65，BottomLine -> 0.35。
4. TopLine 连接 2.Smoothstep -> In，BottomLine 连接 1.Smoothstep -> In。
5. 将 1.Smoothstep 连接新节点 Multiply: A，12.1.6 Multiply 连接这个 Multiply -> B。
6. BottomColor 连接新节点 Multiply: B，Fresnel Effect 连接这个 Multiply -> A。
7. 将 1.Smoothstep 连接新节点 Multiply: A，12.2.6 Multiply 连接这个 Multiply -> B。
8. 将 12.2.5 Multiply 连接新节点 Add: A，12.2.7 Multiply 连接这个 Add -> B。再将这个 Add 连接 Fragment -> Emission 覆盖原有。

# 13. 卡萨丁技能特效 （未完成）

## 13.1 开始（需要特制材质）

1. 创建 Visual Effect Graph 命名为 vfxgraph_KassadinSlash，并拖入场景。
2. 编辑 vfxgraph，删除 Constant Spawn Rate，创建 Single Burst，Count -> 1。
3. 删除 Output Particle Quad，Update Particle 连接新 Output Particle Mesh block，并指定 Mesh。
4. 创建 Set Size，Size -> 1。
5. 删除 Set Velocity Random。修改 Set Lifetime， Random -> Off，Lifetime -> 1。
6. 添加 Set Angle，设置角度让 Mesh 平铺。

# 14. 小型爆炸

## 14.1 使用普通的粒子系统

1. 创建空物体，命名为 vfx_EasyExplosion，Reset Transform，Position -> Y -> 2。
2. 在 vfx_EasyExplosion 下创建 Effects -> Particle System，命名为 Sparks：
   - Looping -> Off
   - Start Lifetime -> Random -> (0.2, 0.8)
   - Start Speed -> Random -> (2, 20)
   - Emission
     - Rate over Time -> 0
     - Bursts -> Count -> Random -> (15, 25)
   - Shape
     - Shape -> Sphere
     - Radius -> 0.2
   - Start Size -> Random -> (0.05, 0.4)
   - Renderer
     - Render Mode -> Stretched Billboard
     - Speed Scale -> 0.03
3. 创建材质 Particle_Orange
   - Shader -> Universal Render Pipeline / Particle / Unlit
   - Emission -> On
   - Emission Map -> 橙色 Intensity -> 2
   - Base Map -> Default-Particle
   - Surface Type -> Transparent
4. Sparks -> Renderer -> Material -> Particle_Orange。
5. Sparks -> Size over Lifetime -> Size -> 从大到小弧线

## 14.2 闪光

1. 在 vfx_EasyExplosion 下创建 Effects -> Particle System，命名为 Flash：
   - Looping -> Off
   - Start Lifetime -> 0.1
   - Start Speed -> 0
   - Start Size -> 5
   - Emission
     - Rate over Time -> 0
     - Bursts -> Count -> 1
   - Shape -> Off
   - Size over Lifetime -> On，Size -> 从大到小斜线
   - Renderer -> Material -> Particle_Orange

## 14.3 火焰和烟雾

1. 在 vfx_EasyExplosion 下创建 Effects -> Particle System，命名为 Fire：
   - Start Lifetime -> Random -> (0.2, 0.4)
   - Start Speed -> Random -> (0.5, 3)
   - Start Size -> Random -> (0.5, 1.5)
   - Looping -> Off
   - Emission
     - Rate over Time -> 0
     - Bursts -> Count -> 10
   - Shape -> Shape -> Sphere，Radius -> 0.2
   - Size over Lifetime -> On，Size -> 从大到小抛物线
   - Renderer -> Material -> Particle_Orange
2. 复制 Fire 命名为 Smoke，
   - Renderer -> Order in Layer -> -1
   - Start Color -> 黑色
   - Start Lifetime -> (0.4, 0.6)
   - Start Speed -> (0.5, 2)
   - Start Size -> (1.5, 2)
3. 复制 Particle_Orange 命名为 Particle_Dark，Emission -> Off，并且设置给 Smoke -> Renderer -> Material。

## 14.4 使用 VFX

1. 场景上创建空对象，vfxgraph_EasyExplosion
2. 创建 Visual Effect Graph，命名为 vfxgraph_Explosion，并拖到 vfxgraph_EasyExplosion 下，Reset Transform。
3. 编辑 vfxgrqaph_Explosion，删除 Constant Spawn Rate，创建 Single Burst。
4. 创建 Random Number 节点，并连接到 Single Burst -> Count
   - Seed -> Per Component
   - Constant -> Off
   - Min -> 15
   - Max -> 25
5. 全选所有节点组成 SPARK。
6. 删除 Set Velocity Random，创建 Set Position(Sphere)，Arc Shpere -> Sphere -> Radius -> 0.1
7. 创建 Set Velocity from Direction & Speed (Spherical)，
   - Speed Mode -> Random
   - Min Speed -> 5
   - Max Speed -> 19
8. Set Lifetime Random -> (0.4, 0.7)
9. Main Texture -> Default-Particle。
10. Set Size over Life -> Composition -> Multiply，Size -> 从上到下的弧线。
11. 删除 Set Color over Life，创建 Set Color，橙色，Intensity 加强。
12. 在 Multiply Size over Life 前创建 Set Size，Random -> Uniform，AB -> (0.1, 1)
13. 在 Set Size 前创建 Set Scale，Random -> Uniform，A -> (0.2, 1, 1)，B -> (0.8, 2, 1)
14. Orient -> Mode -> Along Velocity。
15. Output Particle Quad 命名为 SPARKS。

## 14.5 VFX Flash

1. 创建 Simple Particle System，组名为 FLASH，它的 Output Particle Quad 命名为 FLASH。
2. 删除 Set Velocity Random，Set Lifetime Random -> Random -> Off，Lifetime -> 0.1。
3. 删除 Constant Spawn Rate，创建 Single Burst，Count -> 1。
4. MainTexture -> Default-Particle。
5. 创建 Set Size，Size -> 8。
6. Set Size over Life -> Composition -> Multiply。Size -> 从大到小的斜线。
7. 删除 Set Color over Life。
8. 按住 Ctrl，从 SPARKS 拖出 Set Color block 到 FLASH，可以实现复制 block。

## 14.6 VFX Fire and Smoke

1. 创建 Simple Particle System，组名为 FIRE，Output Particle Quad 命名为 FIRE。
2. 删除 Constant Spawn Rate，创建 Single Burst，创建 Random Number，Min -> 10，Max -> 12，并连接到 Single Burst -> Count。
3. 删除 Set Velocity Random，Set Lifetime -> (0.2, 0.4)。
4. 从 SPARKS 复制 Set Position 和 Set Velocity 到 FIRE。Set Velocity -> Min & Max Speed -> (0.5, 2.5)
5. Main Texture -> Default-Particle。Set Size over Life -> Composition -> Multiply，Size -> 从大到小的抛物线。
6. 创建 Set Size，Random -> Uniform，AB -> (1.25, 2.25)。
7. 删除 Set Color over Life，并从 FLASH 复制 Set Color 到 FIRE。
8. 复制 FIRE 命名为 SMOKE，Output Particle Quad 命名为 SMOKE。
9. Set Color -> 黑色，Set Size Random -> (1.75, 3)，Set Lifetime Random -> (0.4, 0.6)，Set Velocity from -> Min & Max Speed -> (0.25, 2)。
10. vfxgraph_Explosion -> Inspector -> Output Render Order -> SMOKE 拖到顶部。

# 15. 将 2 个物体连接起来的特效

## 15.1 开始

1. 创建空物体名为 vfx_ElectricArc，Reset Transform，Y -> 2。
2. 创建 Visual Effect Graph，名为 vfxgraph_ElectricArc，并拖到 vfx_ElectricArc 下。
3. 编辑 vfxgraph_ElectricArc，Bounds Mode -> Manual。
4. 删除 Constant Spawn Rate，创建 Single Burst，Count -> 100。
5. Capacity -> 150，删除 Set Velocity Random 和 Set Lifetime Random，创建 Set CustomAttribute。
   - Attribute -> Progress
6. 创建 Get Attribute: spawnIndex 节点，连接新节点 Divide，B -> 100。将 Divide 连接 Set Progress -> Progress。
7. 在 Update Particle 创建 Set Position，创建 Get CustomAttribute，Attribute -> Progress，并连接到 Set Position -> Position。
8. 创建 Sample Bezier 节点，将 Get Progress 连接到 T，再将 Sample Bezier -> Position 连接到 Set Position -> Position 覆盖原有。(A: 0 0 0 B: 1 0 0 C: 2 0 0 D: 3 2 0)。ABCD 都是 World。
9. Initialize Particle -> Data Type -> Particle Strip。
10. Update Particle Strip 连接新 Output ParticleStrip Quad，并删除 Output Particle Quad。
11. 修改 Single Burst -> Count -> 200， Particle Per Strip Count -> 200，Divide -> B -> 200，
12. 创建 Get Attribute: spawnIndexInStrip 节点，连接 Divide -> A。
13. Main Texture -> Default-ParticleSystem。
14. 创建 Set Size，创建 float 属性 Thickness，Value -> 0.1，连接 Set Size -> Size。
15. 创建 Set Color，创建 Color 属性 Color，蓝色，并连接 Set Color -> Color。

## 15.2 控制贝塞尔曲线

1. 创建四个 Vector3 属性 Pos1, Pos2, Pos3, Pos4。并一一连接 Sample Bezier -> ABCD。
2. 场景上的 vfxgraph_ElectricArc -> Inspector 添加 VFX Property Binder 脚本。
   - vfx_ElectricArc 创建空物体，命名为 Pos1。
   - 创建 Transform -> Position，Property -> Pos1，Transform -> Pos1。
   - 重复设置 Pos2, Pos3, Pos4。
3. Update Particle Strip 中创建 Add Position。
4. 创建 Value Noise 3D 节点。
5. 创建 Get Attribute: position 节点，连接 Value Noise 3D -> Coordinate。Value Noise 3D -> Derivatives 连接 Add Position -> Position。
6. 创建 Add 节点，Get Attribute: position 连接到 Add -> A。
7. 创建 Total Time 节点，连接 Add -> B。Add 连接 Value Noise 3D -> Coordinate 覆盖原有。
8. Total Time 连接新节点 Multiply。
9. 创建 float 属性 NoiseSpeed，Value -> 2，连接 Multiply -> B。
10. Multiply 连接 Add -> B 覆盖原有。
11. 创建 float 属性 NoisePower，Value -> 0.1，连接 Value Noise 3D -> Range -> X。
12. NoisePower 连接新节点 Negate，Negate 连接 Value Noise 3D -> Range -> Y。
13. 创建 float 属性 NoiseFrequency，Value -> 1，连接 Value Noise 3D -> Frequency。

## 15.3 火花

1. 在 vfxgraph_ElectricArc 中创建 Simple Particle System，Bounds Mode -> Manual。
2. 创建 Set Position Sphere，Arc Sphere -> World。Arc Sphere -> Sphere -> Radius -> 0.1。
3. Pos1 连接 Set Position Sphere -> Arc Shpere -> Sphere -> Transform -> Position。
4. 删除 Set Velocity Random。创建 Set Velocity from Direction & Speed (Spherical)，Speed Mode -> Random，Min & Max Speed -> (5, 18)。Direction Blend -> 0。
5. Set Lifetime Random -> (0.05, 0.2)。Capacity -> 1000。Rate -> 50。
6. Output Particle Quad 创建 Set Size，Random -> Uniform，AB -> (0.05, 0.2)。
7. Set Size over Life -> Composition -> Multiply，Size -> 从大到小抛物线。
8. 创建 Set Scale，Random -> Uniform。AB -> (0.3, 1, 1)-(0.8, 2, 1)。
9. Orient -> Along Velocity。MainTextulre -> Default-Particle。
10. 创建 Set Color。
11. Color 属性连接新节点 Multiply，Multiply -> B -> 3。
12. Set Color over Life，Composition -> Multiply。

# 16. 电弧喷射

## 16.1  开始

1. 创建空对象 vfx_ElectricityDirectional，Reset Transform。Position -> (0, 1.5, -1)。
2. 创建 Visual Effect Graph 名为 vfxgraph_ElectricityDirectional，并拖到 vfx_ElectricityDirectional 下，Position -> (0, 0, 0)
3. 编辑 vfxgraph_ElectricityDirectional，Set Velocity Random -> (-0.33, 0.2, 5) - (0.33, 1, 10)。
4. 创建 Set Position Cone，创建 Cone 属性 Cone，并连接 Set Position Cone -> Cone。Cone -> Transform -> Angles -> X -> 90，Base Radius -> 0.01，Top Radius -> 0.05。
5. 在 Update Particle 中创建 Trigger Event Rate，Rate -> 30。
6. 删除 Output Particle Quad。
7. 创建 Simple Head and Trails，删除左半部，剩下 GPUEvent 和 Trail。
8. Trigger Event Rate 连接 GPUEvent。
9. Strip Capacity -> 256，Particle Per Strip Count -> 16，Bounds Mode -> Manual。
10. 删除 Inherit Source Color 和 Set Lifetime。创建 Inherit Source Lifetime。
11. 删除 Turbulence 和 Subpixel Anti-Aliasing。
12. 回到原来的 Particle System，Set Velocity Random -> Random -> Off。
13. 创建 Get Attribute: Position 节点，连接新节点 Multiply。Multiply 连接 Set Velocity -> Velocity。
14. 创建 float 属性 Speed，Value -> 35，连接 Multiply -> B。
15. 创建 float 属性 Lifetime，Value -> 0.4，连接 Set Lifetime Random -> B。
16. Lifetime 连接新节点 Multiply，Multiply -> B -> 0.2。Multiply 连接 Set Lifetime Random -> A。
17. 来到 Trail Bodies，创建 Set Color 和 Set Color over Life。
18. Set Color over Life 的 Composition 和 Alpha Composition -> Multiply。
19. 创建 Color 属性 Color，颜色为蓝色，Intencity 加深。连接 Set Color -> Color。

## 16.2 嗓点

1. 回到原来的 Particle System -> Update Particle。创建 Add Position。
2. 创建 Value Noise 3D 节点，Derivatives 连接 Add Position -> Position。Range -> (-0.05, 0.05)。Frequency -> 10。Octaves -> 3，Roughness -> 1，Lacunarity -> 2。
3. 创建 Get Attribute: position 节点，连接 Value Noise 3D -> Coorinate。
4. 创建 float 属性 NoisePower，Value -> 0.05，连接 Value Noise 3D -> Range -> Y。
5. NoisePower 连接新节点 Negate，Negate 连接 Value Noise 3D -> Range -> X。
6. 创建 float 属性 Rate，Value -> 32，连接 Rate。
7. Capacity -> 100，Bounds Mode -> Manual。两组的 Initialize Particle -> World。
8. 暂时删除 Velocity 的连线，并禁用 Add Position。
9. 创建 Get Attribute: direction 代替 Get Attribute: Position。
10. 恢复 Velocity 连线，启用 Add Position，Position -> World。

## 16.3 电火花

1. 将 16.1.4 Set Position Cone 替换成 Set Position Sphere。
   - Arc Sphere -> Local
   - Arc Shpere -> Sphere -> Radius -> 0.1
2. Speed -> 3

# 17. 风格化光束 (未完成)

## 17.1 开始（需要 mesh）

1. 创建空对象，vfx_StylizedBeam，Reset Transform，Y -> 2。
2. 创建 Visual Effect Graph，vfxgraph_StylizedBeam，并作为 vfx_StylizedBeam 的子物体拖到场景中。
3. 编辑 vfxgraph_StylizedBeam，删除 Constant Spawn Rate，创建 Single Burst，Count -> 1。
4. 删除 Output Particle Quad，Update Particle 连接新 block，Output Particle Mesh。
5. 创建 Set Size，Size -> 3。
6. 删除 Set Velocity，创建 Set Angle，Set Lifetime -> Random -> Off，Lifetime -> 3。
7. 创建 float 属性 Duration，Value -> 3，连接 Set Lifetime -> Lifetime。

# 19. 火雨术

## 19.1 开始

1. 创建 Visual Effect Graph，vfxgraph_MeteorRain。并拖入场景。
2. 编辑 vfxgraph_MeteorRain，创建 Set Position Cone。
3. 创建 float 属性 Radius，Value -> 5，连接 Set Position Cone -> Arc Cone -> Cone -> Base Radius 和 Top Radius。Position Mode -> Volume（Surface 只会出现在圆柱体边缘，Volume 可以出现在整个圆柱体范围内）。
4. 创建 Vector3 属性 InitialPosition，Y -> 10，连接 Set Position Cone -> Arc Cone -> Cone -> Transform -> Position。
5. 创建 float 属性 Rate，Value -> 5，连接 Constant Spawn Rate -> Rate。
6. 创建 Vector3 属性 Direction，Value -> (0, -25, 10)，连接 Set Velocity Random -> B。
7. Direction 连接新节点 Multiply，Multiply -> B -> 0.8，连接 Set Velocity Random -> A。
8. 创建 float 属性 MeteorLifetime，Value -> 3，连接 Set Lifetime Random -> A 和 B。
9. 在 Update Particle 中创建 Trigger Event Rate。
10. 创建 Simple Ribbon system，删除它的 Spawn，创建 GPUEvent 节点，连接 Ribbon 的 Initialize Particle Strip，Trigger Event Rate 连接 GPUEvent -> Evt。
11. 删除 Ribbon -> Turbulence，删除原来 Particle 的 Set Size over Life，创建 Set Size，Size -> 1。
12. Ribbon -> Strip Capacity -> 1000，Particle Per Strip Count -> 100。删除 Add Position Circle，创建 Inherit Source Position。
13. 创建 float 属性 TrailLifetime，Value -> 0.4，连接 Set Lifetime -> Lifetime。
14. 删除原有 System 的 Update Particle 和 Output Particle Quad 的连接。
15. Set Size over Life -> Size -> 从大到小的 S 型曲线。Orient -> Mode -> Face Camera Position。
16. Trigger Event Rate -> Mode -> Over Distance。
17. 删除 Set Color，Set Color over Life -> Color -> 深红到黑色。
18. Blend Mode -> Alpha。
19. 创建 Set Size，Set Size over Life -> Composition -> Multiply。
20. 创建 float 属性 TrailSize，Value -> 0.8，连接 Set Size -> Size。

## 19.2 前景

1. 将 GPU 前的所有节点组成 METEORS，剩余节点组成 TRAIL_BACK。
2. 复制 TRAIL_BACK，名为 TRAIL_FRONT。
3. TRAIL_FRONT 中的 TrailSize 连接新节点 Multiply，Multiply -> B -> 0.45，连接 Set Size -> Size 覆盖原有。Set Color over Life -> Color -> 金黄到红色。(第一个键的 Intensity -> 10，第二个为 4 左右)
4. Trigger Event Rate 连接 TRAIL_FRONT 的 GPUEvent。
5. 创建 Texture2D 属性 TrailTexture，Value -> Default-Particle，分别连接 TRAIL_FRONT 和 TRAIL_BACK 的 MainTexture。
6. TRAIL_FRONT 的 TrailLifetime 连接新节点 Multiply，Multiply -> B -> 0.5，连接 Set Lifetime -> Lifetime 覆盖原有。

## 19.3 火花

1. 在 METEORS 中创建 Collide with Plane，Lifetime Loss -> 1。
2. 创建 Trigger Event On Die，Count -> 10。
3. 创建 Simple Particle System，删除 Spawn block，Initialize Particle 顶部连接新节点 GPUEvent，Trigger Event On Die 连接这个 GPUEvent -> evt。
4. 创建 Inherit Source Position。
5. Capacity -> 100。
6. Set Lifetime Random -> (0.3, 0.8)
7. Set Velocity Random -> (5, 2, 5) - (-5, 7, -5)
8. MainTexture -> Default-Particle，Orient -> Mode -> Along Velocity。
9. 创建 Set Scale，Random -> Per Component，XY -> (0.1, 0.5, 1) - (0.3, 1, 1)。
10. 创建 Set Size，Random -> Uniform，AB -> (0.8, 1.2)。
11. Set Size over Life -> Composition -> Multiply。Size -> 从大到小抛物线。
12. Set Color over Life -> Color 复制 TRAIL_FRONT 的 Color。
13. Blend Mode -> Additive。
14. 创建 Gravity。创建 Random Number 节点，Max -> -10，连接 Gravity -> Force -> Y。
15. 将这些节点组为 SPARKS。

## 19.4 地面效果

1. 创建 Simple Particle System，组名 IMPACT_DECAL。
2. 删除 Output Particle Quad，Update Particle 连接新 Output Particle Forward Decal。
3. 创建 Set Size，Random -> Uniform，AB -> (1.5, 3.3)。
4. 创建 Set Angle，X -> 90。
5. MainTexture -> Default-Particle。复制 SPARKS 的 Set Color over Life 到 IMPACT_DECAL。Color -> 深红到黑。
6. 删除 Set Velocity Random 和 Spawn，Initialize Particle 顶点连接新节点 GPUEvent，
7. 创建 Trigger Event On Die，Count -> 1，连接 6.GPUEvent。
8. 创建 Inherit Source Position。
9. 复制 Output Particle Forward Decal，Update Particle 连接它。
10. Set Size Random -> (1, 2.5)
11. Set Color over Life -> Color -> 亮金到红，50% Alpha 0。
12. Blend Mode -> Additive。

# 20. 粒子特效跟随路线轨迹（未完成）

## 20.1 需要材质和第三方插件

# 21. URP 和 HDRP 制作空间扭曲

# 22. 魔法球

## 22.1 开始

1. 创建 Visual Effect Graph，vfxgraph_Orb，并拖入场景，Y -> 1.8。
2. 编辑 vfxgraph_Orb，创建 float 属性 Duration，Value -> 10。
3. Spawn system -> Loop Duration 和 Loop Count -> Constant，Duration 属性连接 Loop Duration。
4. Constant Spawn Rate -> Rate -> 2。
5. 创建 Single Burst，Count -> 1。
6. 删除 Set Velocity Random，Set Lifetime Random -> Random -> Off，Bounds Mode -> Manual，MainTexture -> Default-Particle。
7. 删除 Set Size over Life ，创建 Set Size，Size -> 5。Use Soft Particle -> On。
8. Set Color over Life -> Color -> Alpha2 L -> 22%，Alpha3 L -> 72%，Color1 -> HSV(251, 94, 9) ，删除 Color2。
9. Output Particle Quad 命名为 FLARE_DARK，复制所有节点组名为 DARK，新 Output Particle Quad 命名为 FLARE_BRIGHT。
10. Set Size -> 2，Set Color over Life -> Color -> Alpha2 L -> 12%，Alpha3 L -> 54%，Color1 -> HSV(254, 91, 86) Intencity -> 5，Color2 -> 白色。

## 22.2 飘浮物体

1. 创建 Simple Particle System，Output Particle Quad 命名为 PARTICLES_FLOATING。
2. MainTexture -> Default-Particle，在 Set Size over Life 前创建 Set Size，Random -> Uniform，AB -> (0.01, 0.05)，Set Size over Life -> Composition -> Multiply，Size -> 从大到小抛物线。
3. 删除 Set Color over Life，创建 Set Color，Color -> HSV(254, 88, 98)，Intensity -> 8.6。
4. 删除 Set Velocity Random，创建 Set Position Sphere，Arc Sphere -> Sphere -> Radius -> 0.4。
5. 创建 Turbulence，创建 Random Number 节点，Min/Max -> (0.5, 3)，Frequency -> 10，连接 Turbulence -> Intensity。
6. Spawn system -> Loop Duration 和 Loop Count -> Constant，Duration 属性连接 Loop Duration。

## 22.3 外部旋转

1. 复制 DARK，命名为 OUTSIDE，删除 Constant Spawn Rate，Spawn system -> Loop Duration 和 Loop Count -> Infinite，Duration 属性连接 Set Lifetime。
2. 删除 Output Particle Quad，Update Particle 连接新模块 Output Particle Mesh，Mesh -> Sphere。
3. 创建 Set Size，Size -> 1。

## 22.4 Shader

1. Create -> Shader Graph -> URP -> Unlit Shader Graph，命名为 ShaderOrb。
2. 编辑 ShaderOrb，Allow Material Override -> On，Render Face -> Both，Alpha Clipping -> On，Support VFX Graph -> On。
3. 创建 Color 属性 FrontColor 和 BackColor，都为白色，Alpha -> 100，Mode -> HDR。
4. 创建 Texture2D 属性 MainTex，连接新节点 Sample Texture 2D，Sample Texture 2D -> RGBA 连接新节点 Power: A。
5. 创建 float 属性 MainTexPower，X -> 1，连接 Power -> B。
6. FrontColor 连接新节点 Multiply: A，Power 连接 Multiply -> B。Multiply 连接 Fragment -> BaseColor。
7. BackColor 连接新节点 Multiply: A，Power 连接 Multiply -> B。
8. Power 连接 Fragment -> Alpha。
9. 创建 float 属性 Clip，Mode -> Slider，连接 Fragment -> Alpha Clip Threshold。
10. 创建 Is Front Face 节点，连接新节点 Branch: Predicate，FrontColor -> Multiply 连接 Branch -> True，BackColor -> Multiply 连接 Branch -> False，Branch 连接 Fragment -> BaseColor 覆盖原有。
11. 回到 vfxgraph_Orb，OUTSIDE -> Shader Graph -> ShaderOrb，Main Tex -> （类似 Voronoi 的纹理）。
12. 创建 Set Angle，Angle 调整纹理的角度。
13. 设置 Front Color，Back Color，MainTex，调整 Clip 值。

## 22.5 旋转

1. 编辑 OrbShader，创建 Tiling And Offset 连接 Sample Texture 2D -> UV。
2. 创建 Time 节点，连接新节点 Multiply: A。
3. 创建 Vector2 属性 MainTexSpeed，连接 2.Multiply -> B。Multiply 连接 Tiling And Offset -> Offset。
4. 回到 vfxgraph_Orb，设置 MainTexSpeed -> (0.6, -0.1)。

# 23. 围绕的拖尾小球

## 23.1 开始

1. 创建 Visual Effect Graph, 命名为 vfxgraph_orbs，并拖入场景。
2. 编辑 vfxgraph_orbs，创建 Set Position (Sequential: Circle)，Mode -> Wrap。Normal -> (0, 1, 0)，Up -> (1, 0, 0)，Count -> 3。
3. 删除 Set Velocity Random，创建 Single Burst，Count -> 3。Constant Spawn Rate -> Off。
4. 删除 Set Color over Life，删除 Set Lifetime Random，删除 Set Size over Life。创建 Set Size -> 0.66。MainTexture -> Default-Particle。Blend Mode -> Additive。
5. 创建 Set Color，设置颜色。
6. 创建 int 节点，Value -> 3，连接 Single Burst -> Count 和 Set Position -> Count。
7. Update Particle 中创建 Set Position，创建 Rotate 3D 节点，它的 X 和 Z 连接到 Set Position -> Position 的 X 和 Z，创建 Get Attribute: Position，连接到 Rotate 3D -> Position。
8. 创建 Delta Time (VFX) 节点，连接新节点 Multiply，连接到 Rotate 3D -> Angle。Multiply -> B 连接新节点 float，float 节点 Convert to Exposed Property 转换成属性 RotationSpeed，Value -> 2。
9. 创建 Perlin Noise 2D，Noise 连接到 Set Position -> Position -> Y。
10. 创建 Get Attribute: position，Get Attribute: position -> X 和 Z , 连接新节点 Vector2 的 X 和 Y。Vector2 连接 Perlin Noise 2D -> Coordinate。Perlin Noise 2D -> Range -> (-0.2, 0.2)。Frequency -> 0.6，Roughness -> 1。
11. 创建 Trigger Event Rate，Rate -> 60。连接新节点 GPUEvent，GPUEvent 连接新节点 Initialize Particle Strip，然后连接 Update Particle Strip，再连接 Output ParticleStrip Quad。
12. Strip Capacity -> 12，创建 Inherit Source Position，创建 Set Lifetime，Lifetime -> 0.7。
13. 创建 Orient: Face Camera Position，Main Texture -> 合适的拖尾纹理。
14. 创建 Set Color over Life，设置颜色。

## 23.2 Shader

1. 创建 Shader Graph -> VFX Shader Graph，命名为 OrbsShader。
2. 修改 OrbsShader，创建 Sample Texture 2D 节点，Texture 连接新节点 Texture 2D Asset，选择之前拖尾使用的纹理。RGBA 连接 Fragnemt -> BaseColor。UV 连接新节点 Tiling And Offset。
3. 创建 Time 节点，Time 连接 Multiply: A，Multiply 连接 Tiling AndOffset -> Offset。Multiply -> B 连接新节点 Vector2，Vector2 -> X -> 1。
4. Vector2 转换成属性 ScrollSpeed。Texture 2D Asset 转换成属性 Texture。
5. Sample Texture 2D -> RGBA 连接新节点 Multiply: B，Multiply 连接 Fragment -> BaseColor。Multiply -> A 连接新节点 Color，并转换为属性 Color。
6. Sample Texture 2D -> RGBA 再连接新节点 Multiply: B，Multiply -> A 连接新节点 Float，Float 转换成属性 Alpha，Value -> 1。Multiply 连接 Fragment -> Alpha。
7. 回到 vfxgraph_orbs，拖尾的 Shader Graph -> OrbsShader。
8. 创建 Set Size，Size -> 1。创建 Multiply Size over Life，Size -> 从大到小抛物线。
9. 创建 Get Attribute: color 连接到 vfx -> Color。创建 Get Attribute: alpha 连接到 vfx -> Alpha。
10. 将 int 节点转换成属性 Count。

# 25. 角色表面发射粒子

## 25.1 开始

1. 创建 Visual Effect Graph，命名为 vfxgraph_CharacterEffect，并拖入场景。
2. 编辑 vfxgraph_CharacterEffect，Bounds Mode -> Manual。
3. 创建 Set Position (Skinned Mesh)，创建 SkinnedMeshRenderer 属性 SkinnedMeshRenderer，连接 Skinned Mesh。
4. 在 Set Position (Skinned Mesh) 之后创建 Set Position，创建 Transform (Position) 节点，并连接到 Set Position -> Position。
5. 创建 Transform 属性 Transform，连接 Transform (Position) -> Transform。创建 Get Attribute: Position 节点连接 Transform (Position) -> Position。
6. 选中场景中的 vfxgraph_CharacterEffect，SkinnedMeshRenderer 指定场景中的 Mesh。
7. Add Component -> VFX Property Binder，添加 Transform -> Transform，并指定 Traget。
8. 回到 vfxgraph_CharacterEffect 编辑界面，Constant Spawn Rate -> 1000，Capacity -> 100000。
9. Set Velocity Random -> (0, 0.1, 0) - (0, 0.4, 0)，Set Lifetime Random -> (0.15, 0.6)。
10. 创建 Set Size，Random -> Uniform，AB -> (0.015, 0.03)。Set Size over Life -> Composition -> Multiply，Size -> 从大到小抛物线。
11. Main Texture -> Default-Particle。Set Color over Life 的 Composition 和 Alpha Composition -> Multiply。
12. 创建 Set Color，创建 Color 属性 ParticleColor，连接 Set Color -> Color。

## 25.2 烟雾

1. 将全部节点组为 SKIN，复制一个名为 SMOKE。
2. Main Texture -> Smoke8X8，Uv Mode -> Flipbook Blend，Flip Book Size -> 8X8。
3. 创建 Set Tex Index over Life -> Size -> 从小到大直线，最后一帧 Value -> 63。
4. 删除 ParticleColor 属性节点，Set Size -> (0.5, 1)，Set Lifetime Random -> (1, 3)。Rate -> 16。
5. Use Soft Particle -> On，Soft Particle Fade Distance -> 0.3。
6. 复制 SMOKE，名为 FIRE。
7. Set Size Random -> (0.33, 0.66)，Set Color -> 橙色。Set Velocity Random -> (0, 0.01, 0) - (0, 0.1, 0)。Set Lifetime Random -> (0.4, 1)。Multiply Size over Life -> Size -> 从小到大。Rate -> 32。

# 26. 武器表面流光

## 26.1 开始

1. 创建 Lit Shader Graph，命名为 SwordShader。
2. 编辑 SwordShader，创建 Color 属性 Color，颜色 -> 白色，Alpha -> 100，Mode -> HDR。连接新节点 Multiply: A。
3. 创建 Texture2D 属性 MainTex，连接新节点 Sample Texture 2D，然后 Sample Texture 2D -> RGBA 连接 Multiply -> B。
4. Multiply 连接 Fragment -> BaseColor。
5. 创建 Float 属性 Metallic，X -> 0.5，连接新节点 Multiply: A。
6. 创建 Texture2D 属性 MetallicMap 连接新节点 Sample Texture 2D，它的 RGBA 连接 5.Multiply -> B。该 Multiply 连接 Fragment -> Metallic。
7. 创建 Float 属性 Smoothness，X -> 0.5，连接 Fragment -> Smoothness。
8. 创建的这些节点组为 PBR_SETUP。
9. 创建 Gradient Noise，连接新节点 Clamp: in，该 Clamp 连接新节点 Power: A，Power 连接 Add: A。MainTex -> Sample Texture 2D -> RGBA 连接 Add -> B。该 Add 连接 Fragment -> Emission。
10. 创建 Float 属性 GradientNoiseScale，X -> 15， 连接 Gradient Noise -> Scale。
11. 创建 Float 属性 GradientNoisePower，X -> 5，连接 Power -> B。
12. 创建 Color 属性 GradientNoiseColor，白色，Alpha -> 100，Mode -> HDR。连接新节点 Multiply: A，Power 连接该 Multiply -> B。该 Multiply 连接 Add -> A，覆盖原有。
13. 创建 Time 节点，Time 连接新节点 Multiply: A，创建 Vector2 属性 GradientNoiseSpeed，Value -> (0, 0.2)，连接 Multiply -> B。该 Multiply 连接新节点 Add: A，创建 UV 节点连接 Add -> B。Add 连接 Gradient Noise -> UV。
14. 创建 Texture2D 属性 Mask，Default -> 只包含效果部分的纹理，连接新节点 Sample Texture 2D，RGBA 连接新节点 Multiply: A，Power 连接 Multiply -> B，该 Multiply 连接 12.Multiply -> B，覆盖原有。
15. 基于 SwordShader 创建材质 SwordMat，并赋给模型。

## 26.2 VFX

1. 创建 Visual Effect Graph，命名为 vfxgraph_Sword，并拖入场景。
2. 编辑 vfxgraph_Sword，MainTexture -> Smoke8x8，Uv Mode -> Flipbook Blend，Flip Book Size -> (8, 8)。
3. 创建 Set Size，Random -> Uniform，XY -> (0.2, 0.6)，Seet Size over Life -> Composition -> Multiply。
4. 创建 Add Tex Index over Life，Tex Index -> 从小到大的斜线，最后一个点的 value -> 63 （8x8-1)。
5. 创建 Color 属性 SmokeColor，红色。创建 Set Color，SmokeColor 连接 Set Color -> Color。
6. Set Color over Life 的 Composition 和 Alpha Composition -> Multiply，调整 Color 的 Alpha 让它消失的更快。
7. 删除 Set Velocity Random，创建 Set Position AABox，调整 Center 和 Size，让 Box 差不多包裹模型。
8. Rate -> 20，Capacity -> 1000，Use Soft Particle -> On，Soft Particle Fade Distance -> 0.1。创建 Set Angle，Random -> Uniform，AB 的 Z -> (360, -360)。
9. 将所有节点组为 SMOKE，并复制它为 PARTICLE。
10. Uv Mode -> Default，Main Texture -> Default-Particle，Blend Mode -> Additive。
11. 创建 Color 属性 Particle，蓝色，连接 Set Color -> Color 覆盖原有。
12. Set Size Random -> (0.04, 0.06)，Set Position AABox 调整尺寸。
13. Update Particle 创建 Set Position，创建 Rotate 3D 节点，创建 Get Attribute: position 连接 Rotate 3D -> Position，Rotate 3D -> Position 连接 Set Position。
14. 创建 Random Number，Min/Max -> (0.05, 0.1)，连接 Rotate 3D -> Angle。
15. Set Lifetime Random -> B -> 1.5。创建 Set Velocity，Random -> Uniform，AB -> (-0.5, 0.75, -0.5) - (0.5, 1.5, 0.5)。
16. 创建 Trigger Event Rate，Rate -> 20。
17. 创建 Simple Heads And Trails，删除 GPUEvent 前的节点，Trigger Event Rate 连接 GPUEvent。
18. Strop Capacity -> 10，Particle Per Strip Count -> 1000。
19. 删除 Set Lifetime，创建 Inherit Source Size 和 Inherit Source Lifetime。创建 Multiply Lifetime -> 0.2。
20. 删除 Turbulence，MainTexture -> None，Size over Life -> Composition -> Multiply。
21. 创建 Set Color over Life，创建 Gradient 属性 TrailGradient，连接 Set Color over Life -> Color。颜色橙到蓝。

# 27. 风墙（未完成）

## 27.1 需要 mesh

# 28. 龙卷风（未完成）

## 28.1 需要 mesh

# 29. 山石升起

## 29.1 开始

1. 创建 Visual Effect Graph，vfxgraph_Earthbander，并拖入场景。
2. 编辑 vfxgraph_Earthbander，删除 Constant Spawn Rate，创建 Single Burst，Count -> 1。
3. 删除 Set Velocity Random。Set Lifetime -> Random -> Off，Lifetime -> 3。
4. 删除 Output Particle Quad，Update Particle 连接新模块 Output Particle Mesh，创建 Set Size，Size -> 1。创建 Set Scale。
5. 创建 Set Pivot，Y -> 1，让胶囊体在地面下方。
6. 创建 Set Velocity，创建 Sample Curve，Time 连接 Age Over Lifetime，Curve -> (0, 1)(0.2, 0)(0.8, 0)(1, -1)。并连接新节点 Multiply，Multiply -> B -> 11，Multiply 连接 Set Velocity -> Y。
7. Mesh -> Cube，Blend Mode -> Opaque。Set Scale -> Y -> 2。
8. Sample Curve -> Curve -> 添加 (0.45, 0) (0.6, 0) 关键帧，都为 Flat。

## 29.2 小石头

1. 将所有节点组为 CENTER_ROCK，复制为 SMALL_ROCKS_BASE。Output Particle Quad 也命名为 SMALL_ROCKS_BASE。
2. Count -> 15，Spawn system -> Delay Mode -> Before Loop。
3. Set Lifetime -> Random -> Uniform，AB -> (3.2, 3.6)。
4. 创建 Set Position Arc Circle，Arc Circle -> Circle -> Radius -> 0.8。Arc Circle -> Circle -> Transform -> Angles -> X -> -90。
5. 删除 Set Pivot，删除 Set Velocity，Mesh -> 换个石头，调整尺寸形成周围环绕的小石头群。
6. 创建 Set Angle，Random -> Uniform，AB -> (360, 360, 0)-(-360, -360, 0)。
7. 创建 Set Size over Life，Composition -> Multiply，Size -> (0, 0)(0.05, 1)(0.8, 1)(1, 0)。

## 29.3 飞溅

1. 复制 SMALL_ROCKS_BASE，命名为 FLYING_ROCKS，Output Particle Quad 也要改名。
2. Set Lifetime Random -> (0.4, 1.8)，删除 Set Position Circle，创建 Set Position Sphere，Radius -> 0.5，Position -> Y -> 0.4。
3. 创建 Set Velocity from Direction & Speed (Spherical)，创建 Random Number 节点，Min/Max -> (2, 14)，连接 Set Velocity from -> Speed。Set Size -> Random -> Uniform，AB -> (0.2, 0.8)。Set Scale -> Random -> Uniform。Multiply Size over Life -> Size -> 从大到小抛物线。
4. 创建 Gravity，创建 Random Number 节点，Min/Max -> (-7, -15)，连接 Gravity -> Force -> Y。
5. 创建 Collide with Plane，Bounce -> 0.5，Friction -> 0.5。

# 30. 毒液瀑布（未完成）

## 30.1 需要 mesh

# 31. 地面冲击崩裂（未完成）

## 31.1 需要 mesh

# 32. 拖尾原理

1. 在 VFX 中有一种 Particle Strip，它表示粒子的轨迹。粒子带意为粒子连接形成带。
2. 常见的模板会由二部分组成，左侧为标准化的粒子系统配置，然后在 Update Particle 中的 Trigger Event Rate 模块连接 GPU Event。再由 GPU Event 触发右侧的粒子带系统。通过这样一个配置可以实现由右侧的粒子带系统追踪左侧粒子系统的运动，来形成一个轨迹。
3. 可以通过在粒子带系统中使用 Set Color over Life 来改变粒子带的颜色，或者使用 Turbulence 增强轨迹的扭曲。
4. Trigger Event Rate 会生成每秒 60 次的事件，意味着每一帧都会发生一次事件，然后该事件由 GPU Event 接收，并为粒子带系统生成一个新的粒子。然后一般是通过 Inherit Source Position，继承当前父粒子的当前位置。
5. 可以通过创建 Simple Heads And Trails 快速创建。

# 33. 风格化的光柱特效

## 33.1 开始

1. 创建 Visual Effect Graph，名为 vfxgraph_VerticalBeam，并拖入场景。
2. 编辑 vfxgraph_VerticalBeam，删除 Constant Spawn Rate，创建 Single Burst，Count -> 1。
3. 删除 Set Velocity，Set Lifetime Random -> Random -> Off，Lifetime -> 3。
4. 删除 Output Particle Quad，Update Particle 连接新模块 Output Particle Mesh，Mesh -> 圆柱体。创建 Set Size，Size -> 1。创建 Set Scale -> (1, 1, 15)。
5. 创建 Set Angle，Angle -> X -> -90。
6. Output Particle Mesh 命名为 CYLINDER_CORE。Main Texture -> None。创建 Set Color -> (50, 17, 9)。
7. 创建 Multiply Scale over Life，XY 都是平线。Z -> (0, 0) - (0.15, 1) 直线。
8. 所有节点组为 CYLINDER_CORE，复制一个名为 CYLINDER_BACK，Output Particle Mesh 名称也要相应的修改。
9. Set Scale -> (4.9, 4.9, 15)，Set Color -> (0.075, 0, 0.22)。
10. 创建 Set Alpha，Alpha -> 2。
11. 调整 vfxgraph_VerticalBeam -> Inspector -> Output Render Order -> CYLINDER_BACK 先渲染。

## 33.2 溶解

1. 复制 CYLINDER_BACK，命名为 CYLINDER_FRESNEL。
2. Set Scale -> (5, 5, 15)，删除 Set Color。
3. 创建 Unlit Shader Graph，名为 FresnelShader。
4. 修改 FresnelShader，Allow Material Override -> On，Surface Type -> Transparent，Support VFX Graph -> On。
5. 创建 Fresnel Effect 节点，创建 Color 属性 FresnelColor，Mode -> HDR，白色，Alpha -> 100。创建 Float 属性 FresnelPower，X -> 1。FresnelPower 连接 Fresnel Effect -> Power。
6. FresnelColor 连接新节点 Multiply: A，Fresnel Effect 连接 Multiply -> B。Multiply 连接 Fragment -> BaseColor。
7. Multiply 连接新节点 Split，Split -> A 连接 Fragment -> Alpha。
8. 回到 vfxgraph_VerticalBeam，CYLINDER_FRESNEL -> Shader Graph -> FresnelShader。
9. 调整 vfxgraph_VerticalBeam -> Inspector -> Output Render Order -> CYLINDER_FRESNEL 到最后渲染。
10. FresnelColor -> (68, 7, 3)，FresnelPower -> 5.5。
11. 复制 CYLINDER_FRESNEL，名为 CYLINDER_VORONOI。
12. Set Scale -> (5.2, 5.2, 15)。
13. 创建 Unlit Shader Graph，名为 VoronoiShader。
14. 编辑 VoronoiShader，Allow Material Override -> On，Surface Type -> Transparent，Alpha Clipping -> On，Support VFX Graph -> On。
15. 创建 Color 属性 Color，Mode -> HDR，Color -> 白色，Alpha -> 100。
16. 创建 Voronoi 节点，Color 连接新节点 Multiply: A，Voronoi -> Out 连接 Multiply -> B。Multiply 连接 Fragment -> BaseColor。
17. Multiply 连接新节点 Split，Split -> A 连接 Fragment -> Alpha。
18. 创建 Float 属性 Clip，Mode -> Slider。连接 Fragment -> Alpha Clip Threshold。
19. 创建 Tiling And Offset 节点，连接 Voronoi -> UV。创建 Vector2 属性 VoronoiTiling，XY -> (1, 1)，连接 Tiling And Offset -> Tiling。
20. 创建 Time 节点，连接新节点 Multiply: A，创建 Vector2 属性 VoronoiSpeed，XY -> (0, 0)，连接 Multiply -> B。这个 Multiply 连接 Tiling And Offset -> Offset。
21. 回到 vfxgraph_VerticalBeam，CYLINDER_VORONOI -> Shader Graph -> VoronoiShader。Clip -> 0.5，VoronoiTiling -> (7.5, -0.52)，VoronoiSpeed -> (0, 1)。Color -> (11.3, 1.65, 0.77, 1.2)。

## 33.3 电流

1. 复制 CYLINDER_VORONOI，名为 CYLINDER_VORONOI_2。
2. Set Scale -> (5.4, 5.4, 15)。VoronoiTiling -> (7.5, -0.3)，VoronoiSpeed -> (0, 1.2)，Color -> (0.77, 1.65, 11.3, 1.2)，Clip -> 0.8。
3. 创建 AnimationCurve 属性 CylinderGrowXY，连接每个组的 Multiply Scale over Life -> X和Y。
4. 调整 CylinderGrowXY 曲线。

# 35. 视差地裂（未完成）

## 35.1 需要 mesh

# 36. 护盾屏障（未完成）

## 36.1 需要 mesh

# 37. Shader Graph 和 VFX Graph

1. Shader Graph 可以作为自定义 Shader 在 VFX 中使用，让 VFX 可以更精准的控制粒子的材质样式和外观。可以通过 Shader Graph / VFX Shader Graph 创建，也可以创建其他的 Shader Graph 并启用 Support VFX Graph。
2. Lit 类型的材质会接受光照和阴影的计算和影响，Unlit 类型则不参与光照和阴影计算。

# 38. SubGraph

1. SubGraph 可以认为是一种自定义的节点或模块。可以将多个节点组成一个 SubGraph，或多个模块组成一个 SubGraph Block。
2. 在 SubGraph 中也可以创建属性用来接收外部的参数，或者创建 Output 属性提供给外部使用。
3. SubGraph 创建完成后，在 Graph 中可以查找名字的方式和其他节点或模块一样使用。 

# 39. VFX Attribute Inspector

## 39.1 Set Attribute

1. Attribute: 要设置的属性。
2. Composition: 组合选项用来设置如何写入值。
   - Overwrite: 覆盖当前值。
   - Add: 加法，在当前值基础上做加法。
   - Multiply: 乘法，在当前值基础上做乘法。
   - Blend: 混合，通过 Blend (0 - 1)，控制将值的多少百分比混入当前值。
3. Source: 值从哪里获取。
   - Slot: 设置值。
   - Source: 通过继承（即切换为 Inherit Source Attribute）。
4. Random: 随机选项。
   - Off: 不随机。
   - PerComponent: XYZ 三个元素独立随机。
   - Uniform: 从值A和值B的线上随机选择一个值。

## 39.2 Attribute From Curve

1. Attribute: 要设置的属性。
2. Composition: 组合选项用来设置如何写入值。
3. Sample Mode: 对曲线的采样方式，即如果从曲线中获取值。
   - OverLife: 根据粒子的时间轴采样。
   - BySpeed: 基于粒子的速度采样。
   - Random: 每次采样随机位置。
   - RandomConstantPerParticle: 每个粒子会随机到一个固定位置，每次都返回相同的值。
   - Custom: 提供 Sample Time 可以自己控制采样的横坐标。

# 40. 电流贴图

## 40.1 需要 SubstanceDesigner

# 41. 物体相交效果

## 41.1 开始

1. Edit -> Project Settings -> Graphics -> Scriptable Render Pipeline Settings 找到指定的 Setting，选中该文件，Inspector -> Depth Texture -> On，Opaque Texture -> On。
2. 创建 Lit Shader Graph，命名为 Intersection_Lit。
3. 编辑 Intersection_Lit，Surface Type -> Transparent。
4. 创建 Screen Position，Mode -> Raw，连接新节点 Split，Split -> A 连接新节点 Subtract: A。
5. 创建 Float 属性 IntersectionDepth，X -> 0.5，连接 Subtract -> B。
6. 创建 Scene Depth 节点，Sampling -> Eye。连接新节点 Subtract: A，将 4.Subtract 连接到该 Subtract -> B。该 Subtract 连接到 Fragment -> Alpha。
7. 创建该 Shader 对应的材质，获得一个顶点相交部位会透明的效果。
8. 此时调节 IntersectionDepth，会发现值越大，效果越小。所以需要反转一下。将 IntersectionDepth 连接新节点 Remap: In，In Min Max -> (0, 1)，Out Min Max -> (1, 0)，Remap 连接 4.Subtract -> B 覆盖原有。
9. 将 6.Subtract 连接新节点 One Minus，OneMinus 连接新节点 Clamp: In，Clamp 连接 Fragment -> BaseColor 和 Alpha 覆盖原有。
10. 创建 Color 属性 IntersectionColor，Mode -> HDR，Color -> 白色，Alpha -> 100。它连接新节点 Multiply: A。Clamp 连接该 Multiply: B。
11. 将 10.Multiply 连接 Fragment -> Color 覆盖原有。再连接新节点 Split，Split -> A 连接 Fragment -> Alpha 覆盖原有。
12. Alow Material Override -> On，IntersectionMat -> Render Face -> Both。

# 42. 水波纹及相交物体的边缘效果

## 42.1 开始 

1. 创建 Lit Shader Graph，名为 CartoonWaterShader。
2. 编辑 CartoonWaterShader，Surface Type -> Transparent，Alpha Clipping -> On。
3. 创建 Color 属性 BaseColor，Mode -> HDR，Color -> 蓝色。连接 Fragment -> BaseColor。
4. 创建 Voronoi 节点，Voronoi -> Out 连接 Fragment -> Emission。
5. 创建 Time 节点，连接新节点 Multiply: A。
6. 创建 Vector2 属性 RipplesSpeed，XY -> (0.75, 0.75)，连接 Multiply -> B。Multiply 连接 Voronoi -> AngleOffset。
7. Voronoi -> Out 连接新节点 Multiply: A，创建 Color 属性 RipplesColor，Mode -> HDR，Color -> 淡蓝色。连接 Multiply -> B。该 Multiply 连接 Fragment -> Emission 覆盖原有。
8. 创建 Float 属性 RipplesScale，X -> 3，连接 Voronoi -> CellDensity。

## 42.2 细节

1. 创建 Radial Shear 节点，Strength -> (1, 1)，连接 Voronoi -> UV。
2. Voronoi -> Out 连接新节点 Power: A，Power 连接 42.1.7Multiply -> A，覆盖原有。
3. 创建 Float 属性 RipplesDissolve，X -> 5，连接 Power -> B。
4. 创建 Float 属性 Metallic 和 Gloss 连接 Fragment -> Metallic 和 Smoothness。
5. 将 42.1.7Multiply -> A 连接新节点 Normal From Height: In，创建 Float 属性 NormalStength，连接 Normal From Height -> Stength。Normal From Height 连接 Fragment -> Normal。
6. 创建 Simple Noise 节点，Scale -> 20，连接新节点 Normal From Height: In，NormalStength 连接 Normal From Height -> Stength，连接新节点 Normal Blend: A，42.2.5Normal From Height 连接 Normal Blend -> B。Normal Blend 连接 Fragment -> Normal 覆盖原有。
7. Time 连接新节点 Multiply: A，创建 Vector2 属性 NormalSpeed，X Y-> (0.1, -0.1)，连接 Multiply -> B。该 Multiply 连接新节点 Tiling And Offset: Offset。Tiling And Offset 连接 Simple Noise -> UV。
8. 使用 Gradient Noise 代替 Simple Noise。

## 42.3 远近

1. 创建 Position 节点，Space -> World。
2. 创建 Camera 节点，创建 Subtract 节点，Position 连接 A，Camera -> Position 连接 B。（表示离相机的距离）
3. Camera -> Direction 连接新节点 Dot Product: A，Subtract 连接 Dot Product -> B。Dot Product 连接 Fragment -> Alpha。
4. Dot Product 连接新节点 Remap: In，创建 Vector2 节点连接 Remap -> InMinMax，Camera -> FarPlane 连接 Vector2 -> Y。
5. 创建 Remap 节点，创建 Float 属性 FoamOffset，X -> 0.5。
6. Dot Product 连接新节点 Add: A，FoamOffset 连接 Add -> B。Add 连接 5.Remap -> In，Vector2 连接 5.Remap -> InMinMax。
7. 创建 Smoothstep 节点，两个 Remap 分别连接它的 Edge1 和 Edge2。创建 Scene Depth 节点连接 Smoothstep -> In。
8. Smoothstep 连接新节点 One Minus，One Minus 连接 Fragment -> Alpha 覆盖原有。
9. 将 42.1.7Multiply 连接新节点 Split，Split -> R 连接新节点 Remap: In，InMinMax -> (0, 1)，OutMinMax -> (0.2, 5)。
10. Remap 连接新节点 Multiply: A，创建 Float 属性 Transparency，连接 Multiply -> B。该 Multiply 连接 Add: A，OneMinus 连接 Add -> B。该 Add 连接 Fragment -> Alpha。

# 43. 水波纹与物体交互

## 43.1 开始

1. 创建 Lit Shader Graph，命名为 WaterShader，Alpha Clipping -> On，Surface Type -> Transparent。
2. 创建 Float 属性 WaterNoiseScale，WaterHeight，WaterSpeed，FoamDepth，X 都为 2。
3. 创建 Color 属性 FoamDepth，白色，Alpha -> 100。创建 Float 属性 FoamRippleSpeed，X -> 0.3。创建 Float 属性 FoamNoiseScale，X -> 10。创建 Float 属性 DepthFade，X -> 0.7。创建 Color 属性 ShallowColor，淡蓝色，Alpha -> 100。DeepColor，深蓝色，Alpha -> 100。
4. 创建 Position 节点，连接新节点 Split，Split -> R 连接 Vector2: A，Split -> B 连接 Vector2 -> Y。
5. Vector2 连接 Tiling And Offset: UV。创建 Time 节点，连接新节点 Multiply: A，Multiply 连接 Tiling And Offset -> Offset。WaterSpeed 连接 Multiply -> B。
6. Tiling And Offset 连接新节点 Gradient Noise: UV，WaterNoiseScale 连接 Gradient Noise -> Scale。GradientNoise 连接新节点 Multiply: A，WaterHeight 连接 Multiply -> B。
7. 将 6.Multiply 连接新节点 Vector3: Y，创建 Position 节点，Space -> Object，连接新节点 Split，Split -> R 连接 Vector3 -> X，Split -> B 连接 Vector3 -> Z。
8. 将 7.Position 连接新节点 DDY 和新节点 DDX。DDY 连接新节点 Cross Product: A，DDX 连接 Cross Product -> B。
9. Cross Product 连接新节点 Normailze，Normalize 连接新节点 Transform，Object 到 Tangent，Type -> Direction。
10. 将所有节点组为 Vert Displacement and Normals。
11. 将 Vector3 连接 Verte -> Position，Transform 连接 Fragment -> Normal。

## 43.2 颜色

1. 创建 Position 节点，连接新节点 Split，Split -> R 连接新节点 Vector2: X。
2. 创建 Time 节点，连接新节点 Multiply: A，FoamRippleSpeed 连接 Multiply -> B。
3. Vector2 连接新节点 Tiling And Offset: UV，2.Multiply 连接 Tiling And Offset -> Offset。
4. Tiling And Offset 连接新节点 Gradient Noise: UV，FoamNoiseScale 连接 Gradient Noise -> Scale。
5. 创建 Scene Depth 节点，Sampling -> Eye，连接新节点 Subtract: A。
6. 创建 Screen Position，Mode -> Raw，连接新节点 Split，Split -> A 连接 Subtract -> B。
7. Subtract 连接新节点 Add: A，Gradient Noise 连接 Add -> B。
8. Subtract 连接新节点 Divide: A，DepthFade 连接 Divide -> B。
9. Divide 连接新节点 Saturate，Saturate 连接新节点 Lerp: T，DeepColor 连接 Lerp -> A，ShallowColor 连接 Lerp -> B。
10. 将 7.Add 连接新节点 Divide: A，FoamDepth 连接 Divide -> B，Divide 连接新节点 Step: Edge，FoamColor 连接 Step -> In。
11. Step 连接新节点 Add: A，Lerp 连接 Add -> B。该 Add 连接 Fragment -> BaseColor。
12. 将以上这些节点组为 Color。
13. WaterMat:
    - WaterNoiseScale -> 0.16
    - WaterHeight -> 0.8
    - WaterSpeed -> 1.1
    - FoamDepth -> 1.2
    - FoamRippleSpeed -> 0.4
    - FoamNoiseScale -> 4.35
    - DepthFade -> 2.75
    - Enable GPU Instancing -> On

# 46. 能量护盾与物体相交

## 46.1 开始

1. 确认渲染管理设置中 Depth Texture -> On。
2. 创建 Lit Shader Graph，名为 ForceField。创建对应的材质 ForceFieldMat。
3. 编辑 ForceField，Alpha Clipping -> On，Surface Type -> Transparent。

## 46.2 Intersection 相交

1. 创建 Scene Depth 节点，Sampling -> Eye。创建 Screen Position 节点，Mode -> Raw。
2. Screen Position 连接新节点 Split。Scene Depth 连接新节点 Subtract: A。Split -> A 连接 Subtract -> B。Subtract 连接 Fragment -> Emission。
3. 选择使用 ForceFieldMat 的物体，Inspector -> Mesh Renderer -> Lighting -> Case Shadows -> Off。
4. 创建 Float 属性 Offset，X -> 0.6。Split -> A 连接新节点 Subtract: A，Offset 连接 Subtract -> B。该 Subtract 连接 2.Subtract -> B 覆盖原有。删除 2.Subtract 连接 Fragment -> Emission 的连线。
5. 将 2.Subtract 连接新节点 One Minus，One Minus 连接新节点 Smoothstep: In。Smoothstep 连接 Fragment -> Alpha。
6. 创建 Color 属性 Emission，Mode -> HDR，淡蓝色，2.5增强。连接 Fragment -> Emission。
7. Alpha Clip Threshold -> 0.2 左右才有效果。

## 46.3 护盾

1. 创建 Fresnel Effect 节点，创建 Float 属性 FresnelPower，X -> 5，连接 Fresnel Effect -> Power。
2. Fresnel Effect 连接新节点 Add: A，Smoothstep 连接 Add -> B，Add 连接 Fragment -> Alpha 覆盖原有。
3. 创建 Texture2D 属性 Pattern，连接新节点 Sample Texture 2D，它的 RGBA 连接新节点 Multiply: A，2.Add 连接 Multiply -> B。该 Multiply 连接 Fragment -> Alpha 覆盖原有。
4. Sample Texture2D -> UV 连接新节点 Tiling And Offset。
5. 创建 Time 节点，连接新节点 Multiply: A。创建 Float 属性 ScrollSpeed，X -> 0.05，连接 Multiply -> B。该 Multiply 连接 4.Tiling And Offset -> Offset。
6. 将 3.Multiply 连接新节点 Add: A，创建 Float 属性 Fill，X -> 0.01，连接 Add -> B。Add 连接 Fragment -> Alpha 覆盖原有。

# 47. 护盾交互（未完成）

## 47.1 不够详尽

# 48. 护盾（未完成）

## 48.1 需要 mesh

# 49. 风格化闪电（未完成）

## 49.1 需要 mesh

# 50. 冰晶材质

## 50.1 开始

1. 创建 Lit Shader Graph，命名为 IceShader。
2. 编辑 IceShader，创建 Float 属性 Metallic，创建 Float 属性 Smoothness，X -> 0.5，创建 Float 属性 Normals，Min -> -3，Max -> 3。Metallic 连接 Fragment -> Metallic，Smoothness 连接 Fragment -> Smoothness。
3. 创建 Color 属性 Color，Mode -> HDR。创建 Texture2D 属性 IceTexture。
4. 创建 Scene Color 节点，连接 Fragment -> BaseColor。
5. 确保 Pipeline 配置中打开了 Depth Texture 和 Opaque Texture。
6. 创建 Screen Position 节点，连接 Scene Color。
7. Graph Settrings -> Surface Type -> Transparent。
8. IceTexture 连接新节点 Sample Texture 2D，Sample Texture 2D -> R 连接新节点 Multiply: B，Screen Position 连接 Multiply -> A。将 Multiply 连接 Scene Color 覆盖原有。
9. IceMat 中也要设置一次 IceTexture。
10. Scene Position 连接新节点 Lerp: A，Multiply 连接 Lerp -> B，Lerp 连接 Scene Color 覆盖原有。
11. 创建 Float 属性 ReflectionAmount，Min -> -1，Max -> 1。ReflectionAmount 连接 Lerp -> T。
12. Multiply 连接新节点 Add: B，Scene Position 连接 Add -> A。Add 连接 Lerp -> B 覆盖原有。
13. 创建 Normal From Texture 节点，IceTexture 连接 Normal From Texture -> Texture，Normals 连接 Normal From Texture -> Strength。Normal From Texture 连接 Fragment -> Normal。
14. Scene Color 连接新节点 Add: A，Sample Texture 2D -> R 连接 Add -> B，Add 连接 Fragment -> BaseColor 覆盖原有。
15. Color 连接新节点 Multiply: A，14.Add 连接 Multiply -> B。Multiply 连接 Fragment -> BaseColor 覆盖原有。
16. 创建 Fresnel Effect 节点，创建 Float 属性 FresnelPower，X -> 1。连接 Fresnel Effect -> Power。
17. 创建 Color 属性 FresnelColor，Mode -> HDR，连接新节点 Multiply: A，Fresnel Effect 连接 Multiply -> B。该 Multiply 连接新节点 Add: A，15.Multiply 连接 Add -> B。该 Add 连接 Fragment -> BaseColor。
18. 创建 Tiling And Offset 节点，连接 Sample Texture 2D -> UV。
19. 创建 Vector2 属性 IceTiling，XY -> (1, 1)，连接 Tiling And Offset -> Tiling。
20. 如果阴影效果有问题，可以创建一个使用 Unlit 的 Mat，然后设置 Mesh Renderer -> Materials -> 2，并将 UnlitMat 设置给 Materials 第二项。

