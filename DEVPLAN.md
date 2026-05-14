# WarriorLegendsRemake 开发计划

> 最后更新：2026-05-15

---

## 项目现状速览

- **引擎**：Unity URP，2D 横版 RPG / 平台动作
- **场景**：MainMenu → TutorialMap → Map1
- **玩家角色**：珂莱塔（Kamola），近战 + 射击
- **已实现系统**：玩家控制（移动/跳跃/闪避/攻击/射击）、敌人 AI（状态机）、任务系统、对话系统、背包/物品、存档（JSON槽位）、跨场景持久化、Timeline 过场动画、Cinemachine 摄像机、URP Shader（闪白/Dissolve/Bloom）

---

## 已完成的代码修复（2026-05-15）

| 文件 | 修复内容 |
|------|---------|
| `PlayerActionManager.cs` | 重构为**命名锁系统**，彻底解决多系统交叉调用导致玩家卡死的问题 |
| `BackpackPanel.cs` | 改用 `LockActions/UnlockActions`，删除手动保存状态的代码 |
| `TaskPanel.cs` | 同上 |
| `TaskNPC.cs` | 修复 `DisableAll()` 被调用两次但 `EnableAll()` 只调用一次的 bug |
| `DialogueManager.cs` | 修复协程泄漏：用引用追踪确保新对话开始前 Stop 旧协程 |
| `Character.cs` | 修复 FlashEffect 协程叠加；删除所有 `Debug.Log` |
| `Enemy.cs` | **修复 `lostTimeCounter` 未初始化 bug**（敌人第一次追击会立刻返回巡逻）；删除 `FoundPlayer()` 中的 Debug.Log |
| `BoarChaseState.cs` | `OnEnter()` 中重置 `lostTimeCounter`，确保每次追击时间完整 |
| `TaskManager.cs` | 继承 `SingletonMono<T>` |
| `NotificationManager.cs` | 继承 `SingletonMono<T>` |
| `InventoryManager.cs` | 继承 `SingletonMono<T>`；删除测试物品代码 |
| `SingletonMono.cs` *(新建)* | 泛型单例基类 |
| `ISaveable.cs` *(新建)* | 存档接口，为未来模块化存档奠基 |

---

## 阶段一：稳固战斗体验

> 改动量小，体验提升明显，优先完成

- [ ] **击退效果（Knockback）**  
  受击时对玩家/敌人施加横向冲力，被打有位移感  
  实现位置：`Character.TakeDamage()` + `Control.GetInjured()`

- [ ] **Variable Jump Height**  
  跳跃途中松开空格键立即增大下落重力，跳跃更精准  
  实现位置：`Control.cs`，监听 `Jump.canceled` 事件，叠加 `rb.AddForce(Vector2.down * fallMultiplier)`

- [ ] **连击窗口系统**  
  Attack1 → Attack2 → Attack3 的时间窗口，超时重置到 Attack1  
  实现位置：`PlayerAttackManager.cs` 增加 combo index + 窗口计时器

- [ ] **Small Bee AI**  
  复用现有状态机框架，增加垂直跟随移动（Rigidbody2D 直接设 velocity.y）  
  新建文件：`SmallBee.cs`、`BeePatrolState.cs`、`BeeChaseState.cs`

- [ ] **Snail AI**  
  低速 + 受击缩回壳（短暂无敌帧），复用 Boar 框架  
  新建文件：`Snail.cs`、`SnailPatrolState.cs`、`SnailChaseState.cs`

---

## 阶段二：游戏完整性

- [ ] **Checkpoint 系统**  
  关卡内存档点，死亡后在最近 Checkpoint 复活而非重载存档  
  新建：`Checkpoint.cs`（实现 `IInteractable`），在 `PlayerManager` 记录当前激活 Checkpoint 位置

- [ ] **死亡 / Game Over 界面**  
  死亡后显示面板，提供「重试（从 Checkpoint）」和「读档」选项  
  实现位置：监听 `Character.Dead` 事件，在 UI 层显示面板

- [ ] **任务目标指引**  
  接受任务后在场景内显示目标方向箭头（屏幕边缘 UI）或区域高亮  
  新建：`QuestMarker.cs`，注册到 `TaskManager.AddTask()` 回调

- [ ] **任务面板显示进度**  
  击杀类任务显示「野猪 3/5」，现在 `TaskDetailPopup` 已有框架只需对接数据

- [ ] **背包格数上限 + 物品丢弃**  
  `InventoryManager` 增加 `maxSlots` 字段；BackpackPanel 增加「丢弃」按钮

- [ ] **背包存档**  
  `InventoryManager` 实现 `ISaveable` 接口，将物品列表纳入 `SaveSystemJSON`

---

## 阶段三：角色深度

- [ ] **技能系统**  
  主动技能槽（2-4 个），冷却 + MP 消耗  
  新建：`SkillDataSO.cs`（ScriptableObject 定义技能），`SkillManager.cs`，UI 技能栏  
  与 `PlayerActionManager` 对接（技能释放时锁定其他输入）

- [ ] **等级 / 经验系统**  
  击败敌人获得经验，升级提升 `CharacterStats`  
  修改：`Character.cs` 增加 `level`、`exp`；`Enemy.EnemyDead()` 发放经验

- [ ] **装备系统**  
  完善 `ItemType.Equipment` 逻辑，装备后修改 `CharacterStats`（攻击力、防御力等）  
  修改：`InventoryManager.UseItem()`；新建 `EquipmentManager.cs` 管理当前装备槽

- [ ] **商店 NPC**  
  继承 `BaseNPC`，弹出商店 UI，与 `InventoryManager` 对接购买/出售  
  新建：`ShopNPC.cs`、`ShopPanel.cs`

---

## 阶段四：内容扩展（长期）

- [ ] **Map2 场景**  
  利用现有 `SceneTransition` 触发器，设计新关卡  
  建议风格：室内/地下城，区别于 Map1 的森林背景

- [ ] **Boss 战**  
  独立 Boss 状态机（多阶段），专属 Boss 血条 UI  
  新建：`Boss.cs`（继承 `Enemy`），`BossHPBar.cs`

- [ ] **更多敌人类型**  
  - 远程型（投射物攻击，保持距离）  
  - 精英型（高血量 + 范围攻击）  
  - 飞行巡逻型（Small Bee 强化版）

- [ ] **存档系统模块化**  
  各 Manager 实现 `ISaveable` 接口（`TaskManager`、`InventoryManager`、`PlayerManager`、`Character`）  
  `SaveSystemJSON` 改为通过注册表收集所有 `ISaveable` 的数据，新增可存档系统无需改 SaveSystem

---

## 技术债（不阻塞开发，有空再做）

- [ ] `TaskManager.UpdateTaskProgress()` 中的 `FindObjectOfType<TaskPanel>()` 改为事件驱动（UnityEvent 或 ScriptableObject Event），避免每次更新任务都全场景搜索
- [ ] `InventoryManager.UseItem()` 中的 `FindObjectOfType<BackpackPanel>()` 同上
- [ ] `DialogueLoader` 的对话数据改用 `ScriptableObject` 而非裸字符串 ID，获得编辑器校验
- [ ] `PlayerManager` 继承 `SingletonMono<T>` 并提取 `DontDestroyOnLoad` 逻辑
- [ ] 给 `Control.cs` 中的 `DetectInteractable()` 替换 `FindObjectsOfType<InteractionIndicator>()` 为缓存列表，避免每帧全场景搜索

---

## 快速参考：关键脚本位置

| 系统 | 主要脚本 |
|------|---------|
| 玩家控制 | `Scripts/Player/Control.cs` |
| 角色属性/伤害 | `Scripts/General/Character.cs` |
| 操作权限 | `Scripts/Manager/PlayerActionManager.cs` |
| 敌人 AI 基类 | `Scripts/Enemy/Enemy.cs` + `BaseState.cs` |
| 任务系统 | `Scripts/Manager/TaskManager.cs` |
| 对话系统 | `Scripts/Dialogue/DialogueManager.cs` |
| 背包 | `Scripts/Manager/InventoryManager.cs` |
| 存档 | `Scripts/Utilities/SaveSystem.cs` |
| 跨场景管理 | `Scripts/Utilities/CrossSceneManager.cs` |
| 单例基类 | `Scripts/Utilities/SingletonMono.cs` |
| 存档接口 | `Scripts/Utilities/ISaveable.cs` |
