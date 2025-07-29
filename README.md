# Inno-2D 编辑器开发流程文档（Unity 风格 2D Scene Editor）

Inno —— Inspired Notions, No Obedience.

## 🎯 项目目标

构建一个功能完整、体验接近 Unity 的 2D 场景编辑器，服务于 Inno-2D 游戏引擎，支持实体场景构建、实时预览、组件编辑、资源管理等功能。
（此文档由AI归总，不是最终README）

---

## 🧱 编辑器核心模块

### 1. 编辑器框架（Editor Shell）
- 主编辑器窗口（ImGui 驱动）
- Docking 面板支持（如 Scene、Inspector、Hierarchy、Project）
- MenuBar 菜单项与快捷键系统
- 多文档支持（Multi-scene Tab）

### 2. 场景视图（Scene View）
- 正交相机视图（支持平移、缩放）
- 2D 网格背景绘制（可缩放）
- 实体渲染与高亮
- 拖拽选中、框选、旋转、缩放 Gizmo
- 编辑器坐标系绘制（Axis Gizmo）
- 实时 Gizmo 绘制（参考 Unity Handles）
- 鼠标悬停拾取与点击选择（Raycast Picking）
- 多选支持，Shift/Ctrl 操作

### 3. Inspector 检查器面板
- 动态展示选中对象的所有组件
- 每个组件独立折叠、可编辑属性
- 支持自定义组件渲染（如颜色选择、枚举下拉等）
- 支持组件的添加 / 删除 / 顺序调整
- 属性变更后立刻反映到场景

### 4. Hierarchy 层级面板
- 展示当前 Scene 中所有实体（GameObject）
- 支持嵌套（Transform 层级）
- 拖动调整父子关系
- 显示激活状态（eye icon）
- 支持右键菜单（删除、复制、重命名）

### 5. Project 面板
- 所有资源的浏览器视图（纹理、Sprite、Prefab、场景等）
- 按文件夹组织，支持拖拽到场景中
- 支持导入、删除、重命名资源
- 资源预览图支持（缩略图）

### 6. 资源系统（Asset System）
- 支持所有可用资源（Texture、Sprite、Prefab、Audio、Shader 等）
- 提供唯一 ID 和路径引用
- 支持资源加载、卸载、热更新
- 支持资源 Meta 信息（如导入设置）

### 7. 组件系统编辑支持
- 内部 ECS 可插拔系统支持
- 所有组件必须支持序列化
- 属性需支持 ImGui 渲染
- 自定义组件 Inspector 绘制器（类似 Unity [CustomEditor]）

### 8. Gizmo 与 Handles 工具
- 拖拽移动、旋转、缩放 Gizmo
- 支持坐标系（Local / World）切换
- 吸附到网格（Snap to Grid）
- Gizmo 高亮、选中反馈
- 编程式绘制线、框、箭头（Handles.DrawLine 等）

### 9. Prefab 系统
- 支持 GameObject 预制体保存 / 加载
- 支持断开链接 / 应用改动
- 嵌套 Prefab 支持
- Prefab 实例与模板关联标记

### 10. 序列化与保存
- JSON / Binary 序列化系统
- 场景保存、Prefab 保存、资源引用保存
- 支持 Undo / Redo（Command Pattern）

---

## 🏗️ 开发阶段流程（✅：已完成；⚠️：进行中；❓：未开始）

### ✅ 阶段一：核心架构搭建
- 构建主窗口与 Dock 面板（ImGui）
- Editor Shell 初始化流程
- 加入空白 SceneView、Inspector、Hierarchy 等 Panel

### ✅ 阶段二：场景渲染系统
- Editor Camera（支持缩放、平移）
- 渲染 GameObject + SpriteRenderer
- Editor Gizmo 基础功能（坐标轴绘制）
- 鼠标选中逻辑实现（HitTest / Picking）

### ✅ 阶段三：组件系统编辑支持
- ECS 中组件可序列化
- Inspector 动态显示所有组件
- 支持基础类型字段的 ImGui 编辑（float、int、bool、Vector2 等）

### ⚠️ 阶段四：层级管理器（Hierarchy）
- 展示场景中所有 GameObject
- 选中同步、拖动父子关系
- 增删物体支持（+按钮 / Delete）

### ⚠️ 阶段五：资源系统 & Project 面板
- 构建资源加载系统（统一入口）
- 支持纹理、Prefab、音效等的浏览和加载
- Project 面板展示资源 + 拖拽功能

### ❓ 阶段六：Gizmo 与 Handles
- 实现拖动、旋转、缩放 Gizmo
- 鼠标吸附、局部/全局坐标系统切换
- 多选支持
- 支持组件独立控制 Gizmo 显示（如 Camera Gizmo、Collider Gizmo）

### ❓ 阶段七：Prefab 系统
- GameObject 可保存为 Prefab
- 场景中实例为引用对象
- 修改时支持 Apply / Revert 操作
- 嵌套 Prefab 编辑器预览支持

### ❓ 阶段八：序列化系统
- 自定义序列化器（用于组件、GameObject）
- 保存场景到 .scene / .json 文件
- 加载还原完整场景结构
- 支持拖拽 prefab 放入 scene 自动实例化

### ❓ 阶段九：编辑器工具完善
- Undo / Redo 实现
- 快捷键支持（Ctrl+Z / D / Delete）
- Inspector 属性变化动画与高亮反馈
- 支持编辑模式与运行模式切换

---

## 🧩 可拓展模块（中后期）

| 模块名 | 描述 |
|--------|------|
| Animation 面板 | 可视化编辑 Sprite 动画、帧间时间 |
| TileMap 编辑器 | 自定义地图格子编辑器，支持图块画笔 |
| Path 编辑器 | 可视化路径编辑（用于移动轨迹等） |
| Timeline 编辑器 | 用于剧情控制、音效、动画同步 |
| Script 编辑支持 | 脚本组件绑定（动态脚本或反射） |
| 状态机编辑器 | 编辑 AI / 交互 / 动作状态机 |
| Shader 可视化编辑器 | 支持 Shader Graph / 文本 Shader 编辑器 |
| 场景运行模拟 | 运行场景，不跳转游戏模式（参考 Unity PlayMode） |

---

## 💡 注意事项

- 所有模块均需支持无缝与 ECS 数据交互（GameObject/Component 结构）
- 避免编辑器逻辑依赖运行时（Runtime）模块
- 所有编辑器行为需通过接口或桥接层与核心交互
- 禁止在编辑器中直接使用 MonoGame 类型（需经由 InnoEngine 的封装）

---

## 🧠 技术建议

- 使用 `EditorManager` 静态类进行管理
- 使用 `EditorPanel` 抽象类管理面板生命周期
- 使用 `IEditorCommand` 实现可撤销操作（Undo/Redo）
- 所有渲染行为通过 `EditorRenderPass` 区分运行时渲染
- 所有组件字段通过 `IComponentDrawer` 显示，支持拓展
- 使用 GUID 作为资源的唯一标识，避免路径丢失

---

## 📁 示例结构（Editor 项目结构建议）
```
Editor/
├── Core/
│ ├── EditorApp.cs
│ ├── EditorPanel.cs
│ ├── EditorContext.cs
│ ├── EditorManager.cs
│ ├── EditorMode.cs
│ └── IEditorCommand.cs
├── Panels/
│ ├── SceneViewPanel.cs
│ ├── InspectorPanel.cs
│ ├── HierarchyPanel.cs
│ ├── ProjectPanel.cs
│ └── ConsolePanel.cs
├── Gizmos/
│ ├── TransformGizmo.cs
│ └── GizmoRenderer.cs
├── Resources/
│ └── EditorAssets.meta
├── Utility/
│ ├── EditorCamera2D.cs
│ ├── EditorSelection.cs
│ └── DragAndDrop.cs
└── Serialization/
  ├── SceneSerializer.cs
  └── ComponentSerializer.cs
```
