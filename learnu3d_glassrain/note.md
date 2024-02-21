Unity 2021.3.33f1

# 1. 初始化

- 使用 3D(HDRP) 模板创建项目
- 创建一个 Material 名为 Rain
- Shader Graph -> HDRP -> Lit Shader Graph 名为 Rain_Shader 的着色器
- 在创建里创建一个 3D Object -> Quad ，它只会渲染一面。
- 调整到查看 Quad 正面的视角后，右击 Camera，选择 Align With View，让 Camera 自动调整为观察 Quad 的位置和角度。
- 需要给 Quad 设置一张材质贴图
- 将 Quad 的 Material 指定为 Rain，将 Shader 修改为 Rain_Shader

# 2. 修改设置

- Edit Rain_Shader
- Graph Inspector -> HDRP -> Surface Options
  - Surface Type -> Transparent (因为玻璃是有透明度的)
  - Receive fog -> Off
  - Depth Test -> Disabled
  - Receive Decals -> Off
  - Refraction Model -> Thin (适用于玻璃)
- Graph Inspector -> HDRP -> Advanced Options
  - Specular Occlusion Mode -> Off (关闭镜面反射)

# 3. 创建变量

| 变量名              | 类型      |
| ------------------- | --------- |
| Normalmap           | Texture2D |
| Diffuse             | Texture2D |
| Mask                | Texture2D |
| Opacity             | Float     |
| Refraction IOR      | Float     |
| Refraction_Distance | Float     |
| Metallic            | Float     |
| Normal_Strength     | Float     |
| Smoothness          | Float     |
| Rain_Speed          | Float     |
| Tiling              | Vector2   |

# 4. 节点

1. 创建一个 Sample Texture 2D 节点
   - Normalmap 变量指定到 Texture，Texture 的 RGBA(4) 连接到 Fragment 的 Normal
   - Type -> Normal
2. 创建一个 Normal Strangth 节点
   - Normal_Strength 变量指定到 Strength
   - 将 1. Sample Texture 2D 的 RGBA(4) 连到 In，Out 连到 Fragment 的 Normal
3. 创建一个 Flipbook 节点
   - width 和 height 都设置为8
   - Out 连到 1. Sample Texture 2D 的 UV
4. 创建一个 Tiling And Offset 节点
   - Out 连到 3. Flipbook 的 UV
   - Tiling 变量指定到 Tiling
5. （反光）创建一个 Sample Texture 2D 节点
   - Mask 变量指定到 Texture
6. 创建一个 Multiply 节点
   - Metallic 变量指定到 A
   - 然后 5. Sample Texture 2D 的 R 连到 B
   - Out 连到 Fragment 的 Metallic
7. 复制 6. Multiply 节点
   - 将 5. Sample Texture 2D 的 A 连到 A
   - Smoothness 变量指定到 B
   - Out 连到 Fragment 的 Smoothness
8. 返回 Scene 界面，选中 Quad，修改 Surface Options
   - Surface Type -> Transparent （玻璃需要透明）
   - Rendering Pass -> Before refraction （先处理玻璃，然后再处理环境的折射。这样如果玻璃后面有物体计算出来的折射才会比较真实）
   - Receive fog -> Off
   - Receive Decals -> Off
   - Transparency Inputs 下的 Refraction Model -> Thin
   - Shader properties 参数修改：
     - Opacity -> 0.02
     - Metallic -> 0.6
     - Normal_Strength -> 3
     - Smoothness -> 1
     - Tiling -> 0.5, 0.75
9. （表面）创建 Sample Texture 2D 节点
   - 变量 Diffuse 连到 Texture
   - RGBA 连到 Fragment 的 Base Color
10. 创建 Multiply 节点
    - 将 9. Sample Texture 2D 的 A 连到 A
    - 变量 Opacity 连到 B
    - Out 连到 Fragment 的 Alpha
11. （运动）创建 Time 节点
12. 创建 Multiply 节点
    - 将 11. Time 的 Time 连到 A
    - 变量 Rain_Speed 连到 B
13. 创建 Truncate 节点
    - 将 12. Multiply 的 Out 连到 In
    - Out 连到 3. Flipbook 的 Tile

# 5. 播放

Rain Material:
- Normalmap: Rain8X8_Normalized.tga Texture Type 设置为 Normal Map
- Mask: Glass_Dirt_mask.png

增加 Rain_Speed，并播放