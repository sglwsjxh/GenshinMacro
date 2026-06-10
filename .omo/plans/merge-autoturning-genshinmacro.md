# 合并 AutoTurning 与 genshinmacro

## TL;DR
> **Summary**: 把 `genshinmacro` 和 `AutoTurning` 合成一个 Tkinter GUI 工具，统一由一个启动/停止界面管理两个鼠标宏：`x` 负责按住旋转，`x2` 负责双击右键类操作。保留管理员提权、后台线程和轮询监听，但把输入冲突收口成一个线程安全的共享控制层。
> **Deliverables**:
> - `main.py`：统一 GUI + 生命周期控制
> - `macros/`：拆出旋转宏和双击宏
> - `requirements.txt`：合并依赖
> - `README.md`：更新安装与使用说明
> - `.omo/evidence/`：保存 smoke/launch 证据
> **Effort**: Short
> **Parallel**: YES - 3 waves
> **Critical Path**: 共享基础层 → 两个宏模块并行 → GUI 统一接入 → 文档与 smoke 验证

## Context
### Original Request
用户要把 GitHub 上的 `sglwsjxh/AutoTurning` 和当前本地 `genshinmacro` 合并，做成**同一个 GUI** 管理**两个鼠标宏**：
- `x`（鼠标侧键下键）= 按住持续旋转
- `x2`（鼠标侧键上键）= 双击右键类操作

### Interview Summary
- GUI 风格参考 AutoTurning，启动/停止两界面。
- 鼠标按键监听尽量保持当前项目的轮询风格。
- 旋转参数固定，不做可调面板。
- GUI 标题采用「通用宏工具」。

### Metis Review (gaps addressed)
- 修正了按键命名歧义：实现里必须用 `keyboard.mouse.X` / `keyboard.mouse.X2`，不要写成不存在的 `x1`。
- 双宏同时运行会争抢鼠标输入，必须加共享锁。
- `pyautogui` 的 fail-safe 和异常吞掉问题需要显式处理。
- `x2` 的行为要保留“按住期间重复触发”的原始语义，而不是偷偷改成单次触发。
- 旋转默认参数要写死成常量，不留“以后再说”的空口 TODO。

## Work Objectives
### Core Objective
把两个现有脚本合并成一个可直接运行的、线程安全的 GUI 宏工具。

### Deliverables
- 一个可启动的 Tkinter GUI。
- 一个旋转宏模块：监听 `x`，按住时持续右移鼠标。
- 一个双击宏模块：监听 `x2`，按住时按当前节奏重复执行双击右键序列。
- 一个共享的启动/停止控制层，负责线程、锁、退出清理，并允许 worker 注入可测试的按钮读取/动作函数。
- 一份能直接照着执行的 README。

### Definition of Done (verifiable conditions with commands)
- `python -m py_compile main.py macros\base.py macros\rotation.py macros\double_click.py` 通过。
- `python -c "import main; import macros.base; import macros.rotation; import macros.double_click"` 通过，不会自动启动 GUI。
- `python main.py` 能打开标题为「通用宏工具」的窗口。
- 点击启动后，`x`/`x2` 两个宏都能独立工作，互不阻塞。
- 关闭窗口或按退出逻辑后，线程能干净退出，不留僵尸进程。
- `README.md` 的安装与运行步骤和实际行为一致。

### Must Have
- 保留管理员提权。
- 保留后台线程 + `threading.Event` 停止机制。
- 保留轮询式鼠标侧键监听。
- 保留当前双击右键序列的时间节奏。
- 用 `keyboard.mouse.X` / `keyboard.mouse.X2` 作为实际按键名。

### Must NOT Have (guardrails, AI slop patterns, scope boundaries)
- 不新增第三个宏。
- 不加配置面板、不做参数滑条。
- 不把 GUI 线程卡死在轮询里。
- 不把 `x1` 当成有效鼠标键名。
- 不默认吞异常后装作没事。
- 不把旋转触发偷偷改回 `R` 键。

## Verification Strategy
> ZERO HUMAN INTERVENTION - all verification is agent-executed.
- Test decision: tests-after（无新测试框架）；只做 `py_compile`、import sanity、GUI smoke、关闭清理检查。
- QA policy: 每个任务都带 agent 可执行场景，最后再做一轮完整 smoke。
- Evidence: `.omo/evidence/task-{N}-{slug}.txt`

## Execution Strategy
### Parallel Execution Waves
> Target: 5-8 tasks per wave. <3 per wave（except final）= under-splitting. 这个项目很小，所以用 3 波收敛，不强行灌水。

Wave 1: 基础骨架与共享控制层（1 task）

Wave 2: 两个宏模块并行迁移（2 tasks）

Wave 3: GUI 统一接入 + 文档/烟雾验证（2 tasks）

### Dependency Matrix (full, all tasks)
- Task 1 → blocks Task 2, Task 3, Task 4, Task 5
- Task 2 and Task 3 → can run in parallel after Task 1
- Task 4 → blocked by Task 2 and Task 3
- Task 5 → blocked by Task 4

### Agent Dispatch Summary (wave → task count → categories)
- Wave 1 → 1 task → `quick`
- Wave 2 → 2 tasks → `quick`, `quick`
- Wave 3 → 2 tasks → `unspecified-high`, `quick`

## TODOs
> Implementation + Test = ONE task. Never separate.
> EVERY task MUST have: Agent Profile + Parallelization + QA Scenarios.

- [x] 1. `main.py + macros/base.py`: 建立合并后的包骨架和共享宏控制层 - expect 可复用且线程安全的基础控制器

  **What to do**: 把当前单文件脚本重构成可扩展结构，创建 `macros/` 包和共享基础层；把线程生命周期、`Event` 停止信号、输入互斥锁抽出来，供两个宏复用；同步更新 `requirements.txt` 到合并后的依赖集合。
  **Must NOT do**: 不实现具体宏业务；不引入配置面板；不要让 `main.py` 在 import 时自动启动 GUI。

  **Recommended Agent Profile**:
  - Category: `quick` - Reason: 主要是骨架重组、抽公共类和依赖整理。
  - Skills: [] - 无需额外技能。
  - Omitted: `unspecified-high` - 这里不需要做复杂架构推导。

  **Parallelization**: Can Parallel: NO | Wave 1 | Blocks: Task 2, Task 3, Task 4, Task 5 | Blocked By: none

  **References** (executor has NO interview context - be exhaustive):
  - Pattern: `C:\Users\mark3\Desktop\code\projects\genshinmacro\main.py:1-28` - 当前单文件轮询脚本和退出方式。
  - Pattern: `https://github.com/sglwsjxh/AutoTurning/blob/main/main.py#L14-L52` - `is_admin()`、`elevate()`、`RotationManager` 的线程/停止信号模式。
  - Pattern: `https://github.com/sglwsjxh/AutoTurning/blob/main/main.py#L56-L112` - Tkinter GUI 生命周期、启动/停止界面和关闭清理。
  - Pattern: `https://github.com/sglwsjxh/AutoTurning/blob/main/requirements.txt` - 合并后要保留的依赖集合。

  **Acceptance Criteria** (agent-executable only):
  - [ ] `python -m py_compile main.py macros\base.py` 通过。
  - [ ] `python -c "import main; import macros.base"` 通过且不启动窗口。
  - [ ] `pip install -r requirements.txt` 在已激活 venv 中成功。

  **QA Scenarios** (MANDATORY - task incomplete without these):
  ```
  Scenario: Import sanity for merged skeleton
    Tool: Bash
    Steps: Activate venv, run `python -c "import main; import macros.base; print('ok')"`.
    Expected: Process exits 0 and prints `ok`; no GUI window appears.
    Evidence: .omo/evidence/task-1-skeleton-import.txt

  Scenario: Dependency alignment after requirements merge
    Tool: Bash
    Steps: Activate venv, run `pip install -r requirements.txt`.
    Expected: Install completes without unresolved package errors.
    Evidence: .omo/evidence/task-1-requirements-install.txt
  ```

  **Commit**: YES | Message: `refactor(core): add shared macro skeleton` | Files: `main.py`, `macros/base.py`, `macros/__init__.py`, `requirements.txt`

- [x] 2. `macros/rotation.py`: 把 AutoTurning 的旋转逻辑迁到共享基础层并改成 `x` 触发 - expect 侧键下键按住时持续旋转

  **What to do**: 从 AutoTurning 的旋转 loop 提取出一个独立 worker；把触发键从 `R` 改成 `keyboard.mouse.X`（也就是 `x`）；保持按住期间持续执行、松开即停的语义；通过共享锁包住鼠标移动，避免与其他宏抢输入；固定旋转速度/幅度常量，不做 GUI 配置。
  **Must NOT do**: 不保留 `R` 触发；不要把 `x` 写成 `x1`；不要把轮询线程放进 GUI 主线程；不要暴露可调参数。

  **Recommended Agent Profile**:
  - Category: `quick` - Reason: 单模块迁移，重点是把现有 loop 封装好。
  - Skills: [] - 无需额外技能。
  - Omitted: `unspecified-high` - 这里主要是机械迁移，不是大设计题。

  **Parallelization**: Can Parallel: YES | Wave 2 | Blocks: none | Blocked By: Task 1

  **References** (executor has NO interview context - be exhaustive):
  - Pattern: `https://github.com/sglwsjxh/AutoTurning/blob/main/main.py#L24-L31` - 旋转 loop 的原始节奏和 `pydirectinput.move(...)` 用法。
  - Pattern: `https://github.com/sglwsjxh/AutoTurning/blob/main/main.py#L33-L52` - 线程/停止事件封装方式。
  - Pattern: `C:\Users\mark3\Desktop\code\projects\genshinmacro\main.py:6-28` - 当前轮询式写法，方便保持风格一致。

  **Acceptance Criteria** (agent-executable only):
  - [ ] `python -m py_compile macros\rotation.py` 通过。
  - [ ] `python -c "from macros.rotation import RotationWorker; print('ok')"` 通过。
  - [ ] 代码里只使用 `keyboard.mouse.X`（`x`）作为旋转触发键，不再引用 `R`。

  **QA Scenarios** (MANDATORY - task incomplete without these):
  ```
  Scenario: Deterministic loop smoke with injected detector
    Tool: Bash
    Steps: Run an inline Python harness that injects a fake `x` detector and a fake movement callback into the rotation worker, then sets the stop event after one short cycle.
    Expected: Worker starts, invokes the injected movement callback at least once, and exits cleanly after the stop signal.
    Evidence: .omo/evidence/task-2-rotation-harness.txt

  Scenario: Stop signal preempts the rotation loop
    Tool: Bash
    Steps: Start the worker with the stop event already set.
    Expected: Thread returns immediately and the movement callback is never called.
    Evidence: .omo/evidence/task-2-rotation-stop.txt
  ```

  **Commit**: YES | Message: `feat(rotation): add x hold-to-rotate worker` | Files: `macros/rotation.py`

- [x] 3. `macros/double_click.py`: 把当前 X2 双击右键序列迁到共享基础层并保留重复触发语义 - expect 侧键上键按住时继续执行原宏节奏

  **What to do**: 提取当前 `genshinmacro` 的双击右键节奏，封装成独立 worker；监听 `x2`；保留“按住期间按当前循环继续重复触发”的旧语义；同样使用共享锁保护鼠标动作；把异常处理和 fail-safe 处理补上，不要靠静默吞掉错误。
  **Must NOT do**: 不改成单次触发；不改变原有 sleep 节奏；不新增配置开关；不要让 `x2` 和 `x` 竞争时互相打乱鼠标动作。

  **Recommended Agent Profile**:
  - Category: `quick` - Reason: 基本是把现有 28 行逻辑模块化。
  - Skills: [] - 无需额外技能。
  - Omitted: `unspecified-high` - 这不是复杂推导型任务。

  **Parallelization**: Can Parallel: YES | Wave 2 | Blocks: none | Blocked By: Task 1

  **References** (executor has NO interview context - be exhaustive):
  - Pattern: `C:\Users\mark3\Desktop\code\projects\genshinmacro\main.py:10-25` - 当前双击右键的原始动作序列和重复节奏。
  - Pattern: `C:\Users\mark3\Desktop\code\projects\genshinmacro\main.py:6-28` - 当前轮询和 `escape` 退出模式。
  - Pattern: `https://github.com/sglwsjxh/AutoTurning/blob/main/main.py#L62-L66` - GUI 关闭时的收尾模式，可复用为异常安全退出。

  **Acceptance Criteria** (agent-executable only):
  - [ ] `python -m py_compile macros\double_click.py` 通过。
  - [ ] `python -c "from macros.double_click import DoubleClickWorker; print('ok')"` 通过。
  - [ ] 代码里只使用 `x2` 作为双击触发键，并保留按住期间重复触发的语义。

  **QA Scenarios** (MANDATORY - task incomplete without these):
  ```
  Scenario: Deterministic burst smoke with injected detector
    Tool: Bash
    Steps: Run an inline Python harness that injects a fake `x2` detector and fake click callbacks into the double-click worker, then sets the stop event after one short cycle.
    Expected: Worker starts, invokes the injected click callbacks in the recorded burst order, and exits cleanly after the stop signal.
    Evidence: .omo/evidence/task-3-doubleclick-harness.txt

  Scenario: Stop signal halts repeat loop
    Tool: Bash
    Steps: Start the worker with the stop event already set.
    Expected: Thread returns immediately and the click callbacks are never called.
    Evidence: .omo/evidence/task-3-doubleclick-stop.txt
  ```

  **Commit**: YES | Message: `feat(macro): add x2 double-click worker` | Files: `macros/double_click.py`

- [x] 4. `main.py`: 把两个 worker 接进 AutoTurning 风格的 Tkinter GUI，并保留管理员提权与关闭清理 - expect 单窗口控制双宏

  **What to do**: 以 AutoTurning 的启动/停止双界面为基础，重写 `main.py` 为统一入口；显示标题「通用宏工具」；启动时同时开启两个 worker；停止时同时关闭两个 worker；保留管理员提权；给窗口关闭和 `Esc` 补一个一致的退出路径；如果需要图标，拷贝 `logo.ico` 并挂到窗口。
  **Must NOT do**: 不加新设置页；不把 GUI 线程阻塞住；不改掉两个宏的触发映射；不要漏掉线程 join；不要在关闭时留下僵尸线程。

  **Recommended Agent Profile**:
  - Category: `unspecified-high` - Reason: 这是把骨架、线程、GUI 和退出逻辑整合到一起的核心任务。
  - Skills: [] - 不需要额外插件。
  - Omitted: `quick` - 这一步不是纯机械改名。

  **Parallelization**: Can Parallel: NO | Wave 3 | Blocks: Task 5 | Blocked By: Task 2, Task 3

  **References** (executor has NO interview context - be exhaustive):
  - Pattern: `https://github.com/sglwsjxh/AutoTurning/blob/main/main.py#L14-L22` - 管理员提权。
  - Pattern: `https://github.com/sglwsjxh/AutoTurning/blob/main/main.py#L56-L112` - Tkinter GUI 结构、启动/停止界面、关闭回调。
  - Pattern: `https://github.com/sglwsjxh/AutoTurning/blob/main/README.md#L1-L22` - 视觉/文案参考和启动路径。
  - Pattern: `C:\Users\mark3\Desktop\code\projects\genshinmacro\main.py:1-28` - 原始退出方式与轮询风格。

  **Acceptance Criteria** (agent-executable only):
  - [ ] `python main.py` 打开 GUI，窗口标题是「通用宏工具」。
  - [ ] `python -c "import main; print('ok')"` 通过且不会自动启动 GUI。
  - [ ] 点击启动后两个 worker 同时进入运行态。
  - [ ] 点击停止或关闭窗口后，两个线程都能干净退出。
  - [ ] GUI 主线程不被鼠标轮询卡死。

  **QA Scenarios** (MANDATORY - task incomplete without these):
  ```
  Scenario: GUI launch and start/stop
    Tool: Bash
    Steps: Run `python main.py`, click Start, observe both macros become active, then click Stop.
    Expected: UI 状态切换正常，两个 worker 都被停止且窗口保持响应。
    Evidence: .omo/evidence/task-4-gui-start-stop.txt

  Scenario: Close path cleanup
    Tool: Bash
    Steps: With the app running, close the window (or press Esc if bound as emergency exit).
    Expected: App exits cleanly, no zombie python process remains.
    Evidence: .omo/evidence/task-4-gui-close-cleanup.txt
  ```

  **Commit**: YES | Message: `feat(gui): unify two macros in one tkinter app` | Files: `main.py`, `logo.ico` (if copied)

- [x] 5. `README.md + .omo/evidence/`: 更新安装/运行说明并补齐 smoke 证据 - expect 新人照着文档就能跑起来

  **What to do**: 把 README 改成合并后的项目说明：安装依赖、创建/激活 venv、启动 GUI、`x/x2` 映射、管理员权限提示、常见问题；同时在 `.omo/evidence/` 留下 import / launch / cleanup 的证据文件，方便回看。
  **Must NOT do**: 不写空话宣传；不写与实际行为不一致的步骤；不要把测试结果只留在聊天记录里。

  **Recommended Agent Profile**:
  - Category: `quick` - Reason: 文档与验证整理，改动面小但要求准确。
  - Skills: [] - 无需额外技能。
  - Omitted: `unspecified-high` - 不是大范围设计推导。

  **Parallelization**: Can Parallel: YES | Wave 3 | Blocks: none | Blocked By: Task 4

  **References** (executor has NO interview context - be exhaustive):
  - Pattern: `https://github.com/sglwsjxh/AutoTurning/blob/main/README.md#L1-L22` - 安装与运行文档结构。
  - Pattern: `https://github.com/sglwsjxh/AutoTurning/blob/main/logo.ico` - 图标资产（如需要说明）。
  - Pattern: `C:\Users\mark3\Desktop\code\projects\genshinmacro\requirements.txt:1-2` - 当前依赖基线。

  **Acceptance Criteria** (agent-executable only):
  - [ ] README 中的命令和实际路径一致。
  - [ ] `.omo/evidence/` 下存在本次 smoke 验证的文本证据。
  - [ ] 文档明确写出 `x` / `x2` 映射、管理员权限和退出方式。

  **QA Scenarios** (MANDATORY - task incomplete without these):
  ```
  Scenario: Fresh clone runbook
    Tool: Bash
    Steps: Create/activate venv, install requirements, run `python main.py`.
    Expected: Tool launches without missing dependency errors; README steps match reality.
    Evidence: .omo/evidence/task-5-fresh-clone-runbook.txt

  Scenario: Documentation truthfulness check
    Tool: Bash
    Steps: Compare README claims against actual GUI labels and button mappings.
    Expected: No mismatch between docs and behavior.
    Evidence: .omo/evidence/task-5-doc-truthfulness.txt
  ```

  **Commit**: YES | Message: `docs: update merged macro usage guide` | Files: `README.md`, `.omo/evidence/*`

## Final Verification Wave (MANDATORY — after ALL implementation tasks)
> 4 review agents run in PARALLEL. ALL must APPROVE. Present consolidated results to user and get explicit "okay" before completing.
> **Do NOT auto-proceed after verification. Wait for user's explicit approval before marking work complete.**
> **Never mark F1-F4 as checked before getting user's okay.** Rejection or user feedback -> fix -> re-run -> present again -> wait for okay.
- [x] F1. Plan Compliance Audit — oracle
- [x] F2. Code Quality Review — unspecified-high
- [x] F3. Real Manual QA — unspecified-high (+ desktop-control / local smoke if needed)
- [x] F4. Scope Fidelity Check — deep

## Commit Strategy
- 单个合并提交也可以，但更稳妥的是按任务提交：Task 1 打基础，Task 2/3 各自落宏，Task 4 做 GUI，Task 5 做文档与证据。
- 如果实现中发现某一任务改动太大，优先拆成两个相邻提交，不要把 GUI 和宏逻辑混在一个不可回滚的大补丁里。

## Success Criteria
- 一个入口 `main.py` 能稳定启动。
- `x` 与 `x2` 映射正确，且彼此不抢输入。
- GUI 能启动、停止、关闭，线程无残留。
- 依赖安装和 README 步骤真实可用。
- `.omo/evidence/` 里有本次 smoke 的文本记录。
- 没有新增配置面板、没有第三个宏、没有把 `x/x2` 写错成别的按键名。
