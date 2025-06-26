# OllamaIPScanner



## 项目简介

OllamaIPScanner 是一款基于 C# WinForms 的 Ollama 服务批量扫描工具，支持 CIDR 段和单个 IP 扫描，具备多线程高效扫描、断点续扫、进度显示、日志输出等功能。适用于教育、合规测试等场景。

---

## 主要功能
- 支持加载包含 CIDR 段和单个 IP 的 ip.txt 文件
- 多线程高效端口扫描（默认 50 并发，可自定义）
- 实时日志窗口，显示扫描进度和发现的服务
- 进度条显示整体进度
- 支持断点保存与加载，断点续扫
- 扫描结果自动保存为 JSON 文件
- 一键发布：支持框架依赖版和自包含版

---

## 使用方法

### 1. 准备 IP 文件（ip.txt）
- 每行一个目标，可以是 CIDR 段（如 1.2.3.0/24）或单个 IP（如 113.81.20.245）
- 支持混合写入，支持注释（# 开头）

示例：
```
# 单个IP
113.81.20.245
120.126.23.26
# CIDR段
1.2.3.0/24
```

### 2. 运行程序
- 双击 `OllamaIPScanner.exe` 启动
- 点击"选择IP文件"加载 ip.txt
- 点击"开始扫描"
- 可随时点击"暂停"/"继续"
- 可点击"保存进度"/"加载进度"实现断点续扫
- 扫描结果自动保存为 `scan_result_日期时间.json`

### 3. 日志与进度
- 日志窗口实时显示正在扫描和已发现的服务
- 进度条显示整体进度

---

## 编译与发布

### 1. 依赖环境
- Windows 10/11
- .NET 6 SDK（开发/编译时需要）

### 2. 一键发布
- 运行 `build_publish.bat`，自动生成：
  - `publish/framework_dependent`（框架依赖版，需目标机有.NET 6运行时）
  - `publish/self_contained`（自包含版，无需任何环境，直接运行）
- 发布目录已自动清理，仅保留运行所需文件

### 3. 手动编译
```
dotnet build OllamaIPScanner/OllamaIPScanner.csproj
```

---

## 运行要求

- 框架依赖版：需目标机器已安装 .NET 6 运行时
- 自包含版：无需任何环境，直接运行 exe 即可

---

## 常见问题

- **Q: ip.txt 支持哪些格式？**
  - A: 支持单个IP（如 1.2.3.4）、CIDR段（如 1.2.3.0/24），可混合，支持注释。
- **Q: 进度条/日志无变化？**
  - A: 请确保未被杀毒软件拦截，或尝试自包含版。
- **Q: 发布目录哪些文件可以删除？**
  - A: 框架依赖版必须保留 exe/dll/json，详见上文说明。自包含版只需 exe。
- **Q: 如何自定义线程数？**
  - A: 可在 Scanner.cs 里修改 `maxThreads` 变量。

---

## 联系方式

- 作者：johnzhao
- 邮箱：a@mallerp.com
- 仅供学习与合规测试，禁止非法用途！

---

## 文件详细说明
## 项目目录结构

```
Ip端口扫描工具/
├── README.md                    # 项目说明文档
├── build_publish.bat           # 一键发布脚本
├── OllamaIPScanner/            # 主项目源码目录
│   ├── OllamaIPScanner.csproj  # 项目配置文件
│   ├── Program.cs              # 程序入口点
│   ├── MainForm.cs             # 主窗体界面代码
│   ├── Scanner.cs              # 核心扫描逻辑
│   ├── ProgressState.cs        # 进度状态管理
│   ├── bin/                    # 编译输出目录
│   │   └── Debug/
│   │       └── net6.0-windows/ # Debug版本输出
│   │           ├── OllamaIPScanner.exe
│   │           ├── OllamaIPScanner.dll
│   │           ├── OllamaIPScanner.deps.json
│   │           ├── OllamaIPScanner.runtimeconfig.json
│   │           ├── OllamaIPScanner.pdb
│   │           ├── runtimes/   # 运行时依赖
│   │           │   └── browser/
│   │           │       └── lib/
│   │           │           └── net6.0/
│   │           │               └── System.Text.Encodings.Web.dll
│   │           ├── System.Text.Encodings.Web.dll
│   │           └── System.Text.Json.dll
│   └── obj/                    # 编译中间文件目录
│       ├── Debug/
│       │   └── net6.0-windows/ # Debug编译中间文件
│       │       ├── apphost.exe
│       │       ├── OllamaIPScanner.AssemblyInfo.cs
│       │       ├── OllamaIPScanner.assets.cache
│       │       ├── OllamaIPScanner.csproj.AssemblyReference.cache
│       │       ├── OllamaIPScanner.csproj.CoreCompileInputs.cache
│       │       ├── OllamaIPScanner.csproj.FileListAbsolute.txt
│       │       ├── OllamaIPScanner.dll
│       │       ├── OllamaIPScanner.GeneratedMSBuildEditorConfig.editorconfig
│       │       ├── OllamaIPScanner.genruntimeconfig.cache
│       │       ├── OllamaIPScanner.pdb
│       │       ├── ref/        # 引用程序集
│       │       │   └── OllamaIPScanner.dll
│       │       └── refint/     # 内部引用程序集
│       │           └── OllamaIPScanner.dll
│       ├── OllamaIPScanner.csproj.nuget.dgspec.json
│       ├── OllamaIPScanner.csproj.nuget.g.props
│       ├── OllamaIPScanner.csproj.nuget.g.targets
│       ├── project.assets.json
│       ├── project.nuget.cache
│       └── Release/            # Release版本编译输出
│           └── net6.0-windows/
│               ├── apphost.exe
│               ├── OllamaIPScanner.AssemblyInfo.cs
│               ├── OllamaIPScanner.assets.cache
│               ├── OllamaIPScanner.csproj.AssemblyReference.cache
│               ├── OllamaIPScanner.csproj.CoreCompileInputs.cache
│               ├── OllamaIPScanner.csproj.FileListAbsolute.txt
│               ├── OllamaIPScanner.dll
│               ├── OllamaIPScanner.GeneratedMSBuildEditorConfig.editorconfig
│               ├── OllamaIPScanner.genruntimeconfig.cache
│               ├── OllamaIPScanner.pdb
│               ├── PublishOutputs.*.txt
│               ├── ref/
│               │   └── OllamaIPScanner.dll
│               ├── refint/
│               │   └── OllamaIPScanner.dll
│               └── win-x64/    # x64平台发布文件
│                   ├── OllamaIPScanner.AssemblyInfo.cs
│                   ├── OllamaIPScanner.assets.cache
│                   ├── OllamaIPScanner.csproj.AssemblyReference.cache
│                   ├── OllamaIPScanner.csproj.CoreCompileInputs.cache
│                   ├── OllamaIPScanner.csproj.FileListAbsolute.txt
│                   ├── OllamaIPScanner.deps.json
│                   ├── OllamaIPScanner.dll
│                   ├── OllamaIPScanner.GeneratedMSBuildEditorConfig.editorconfig
│                   ├── OllamaIPScanner.genruntimeconfig.cache
│                   ├── OllamaIPScanner.pdb
│                   ├── PublishOutputs.*.txt
│                   ├── ref/
│                   │   └── OllamaIPScanner.dll
│                   ├── refint/
│                   │   └── OllamaIPScanner.dll
│                   └── singlefilehost.exe
├── publish/                    # 发布版本目录
│   ├── framework_dependent/    # 框架依赖版（需.NET 6运行时）
│   │   ├── OllamaIPScanner.exe
│   │   ├── OllamaIPScanner.dll
│   │   ├── OllamaIPScanner.deps.json
│   │   ├── OllamaIPScanner.runtimeconfig.json
│   │   ├── runtimes/          # 运行时依赖
│   │   │   └── browser/
│   │   │       └── lib/
│   │   │           └── net6.0/
│   │   │               └── System.Text.Encodings.Web.dll
│   │   ├── System.Text.Encodings.Web.dll
│   │   └── System.Text.Json.dll
│   └── self_contained/        # 自包含版（无需额外环境）
│       └── OllamaIPScanner.exe
└── ip.txt                     # IP地址列表文件（用户自建）
```

### 目录说明

#### 核心源码文件
- **Program.cs**: 应用程序入口点，负责启动主窗体
- **MainForm.cs**: 主界面窗体，包含UI控件和事件处理
- **Scanner.cs**: 核心扫描引擎，实现多线程IP端口扫描逻辑
- **ProgressState.cs**: 进度状态管理类，处理扫描进度保存和恢复

#### 编译输出目录
- **bin/Debug/net6.0-windows/**: 开发调试版本输出目录
- **obj/**: 编译过程中生成的中间文件，可安全删除

#### 发布版本目录
- **publish/framework_dependent/**: 框架依赖版本，需要目标机器安装.NET 6运行时
- **publish/self_contained/**: 自包含版本，包含所有依赖，可直接运行

#### 配置文件
- **OllamaIPScanner.csproj**: .NET项目配置文件，定义目标框架、依赖包等
- **build_publish.bat**: 自动化发布脚本，一键生成两种发布版本

### 技术架构

#### 核心技术栈
- **.NET 6**: 跨平台开发框架
- **WinForms**: 桌面应用程序UI框架
- **System.Text.Json**: JSON序列化/反序列化
- **System.Net.Http**: HTTP客户端通信
- **System.Net.Sockets**: TCP端口扫描

#### 核心模块说明

**1. 扫描引擎 (Scanner.cs)**
- 多线程并行扫描，默认50个并发线程
- 支持CIDR段解析和单个IP扫描
- TCP连接测试，超时控制（默认5秒）
- HTTP API调用获取Ollama模型列表
- 实时进度更新和日志输出

**2. 进度管理 (ProgressState.cs)**
- JSON格式保存/加载扫描进度
- 支持断点续扫功能
- 扫描结果持久化存储

**3. 用户界面 (MainForm.cs)**
- 文件选择对话框
- 实时日志显示
- 进度条和状态标签
- 控制按钮（开始/暂停/保存/加载）

#### 依赖包
- `System.Text.Json` (7.0.2): JSON处理
- `System.Net.Http` (4.3.4): HTTP客户端

### 开发环境搭建

#### 必需工具
- Visual Studio 2022 或 Visual Studio Code
- .NET 6 SDK
- Windows 10/11 操作系统

#### 开发步骤
1. 克隆项目到本地
2. 使用Visual Studio打开 `OllamaIPScanner.csproj`
3. 恢复NuGet包依赖
4. 按F5开始调试，或Ctrl+F5运行

#### 自定义配置
- 修改 `Scanner.cs` 中的 `maxThreads` 变量调整并发数
- 修改 `targetPort` 变量更改目标端口（默认11434）
- 修改 `timeout` 变量调整连接超时时间（默认5秒）

---
### 核心文件

| 文件名 | 类型 | 说明 | 重要性 |
|--------|------|------|--------|
| `Program.cs` | 源代码 | 应用程序入口点，启动主窗体 | ⭐⭐⭐⭐⭐ |
| `MainForm.cs` | 源代码 | 主界面窗体，UI控件和事件处理 | ⭐⭐⭐⭐⭐ |
| `Scanner.cs` | 源代码 | 核心扫描引擎，多线程扫描逻辑 | ⭐⭐⭐⭐⭐ |
| `ProgressState.cs` | 源代码 | 进度状态管理，断点续扫功能 | ⭐⭐⭐⭐ |
| `OllamaIPScanner.csproj` | 项目配置 | .NET项目配置文件，定义依赖和框架 | ⭐⭐⭐⭐ |

### 构建和发布文件

| 文件名 | 类型 | 说明 | 重要性 |
|--------|------|------|--------|
| `build_publish.bat` | 脚本 | 一键发布脚本，生成两种发布版本 | ⭐⭐⭐⭐ |
| `bin/Debug/net6.0-windows/OllamaIPScanner.exe` | 可执行文件 | Debug版本主程序 | ⭐⭐⭐ |
| `bin/Debug/net6.0-windows/OllamaIPScanner.dll` | 程序集 | Debug版本主程序集 | ⭐⭐⭐ |
| `obj/` | 目录 | 编译中间文件，可安全删除 | ⭐ |

### 发布版本文件

| 文件名 | 类型 | 说明 | 重要性 |
|--------|------|------|--------|
| `publish/framework_dependent/OllamaIPScanner.exe` | 可执行文件 | 框架依赖版主程序 | ⭐⭐⭐⭐ |
| `publish/self_contained/OllamaIPScanner.exe` | 可执行文件 | 自包含版主程序（推荐） | ⭐⭐⭐⭐⭐ |

### 用户文件

| 文件名 | 类型 | 说明 | 重要性 |
|--------|------|------|--------|
| `ip.txt` | 配置文件 | IP地址列表文件（用户自建） | ⭐⭐⭐⭐⭐ |
| `scan_result_*.json` | 输出文件 | 扫描结果文件（程序自动生成） | ⭐⭐⭐⭐ |
| `Progress/scan_progress_*.json` | 进度文件 | 断点续扫进度文件（程序自动生成） | ⭐⭐⭐ |

### 依赖文件

| 文件名 | 类型 | 说明 | 重要性 |
|--------|------|------|--------|
| `System.Text.Json.dll` | 依赖库 | JSON序列化/反序列化库 | ⭐⭐⭐ |
| `System.Text.Encodings.Web.dll` | 依赖库 | Web编码处理库 | ⭐⭐ |
| `OllamaIPScanner.deps.json` | 依赖配置 | .NET依赖配置文件 | ⭐⭐ |
| `OllamaIPScanner.runtimeconfig.json` | 运行时配置 | .NET运行时配置文件 | ⭐⭐ |

### 开发相关文件

| 文件名 | 类型 | 说明 | 重要性 |
|--------|------|------|--------|
| `*.pdb` | 调试文件 | 程序调试符号文件 | ⭐ |
| `*.cache` | 缓存文件 | MSBuild编译缓存文件 | ⭐ |
| `*.g.props` | 属性文件 | NuGet包属性文件 | ⭐ |
| `*.g.targets` | 目标文件 | NuGet包目标文件 | ⭐ |
| `project.assets.json` | 资产文件 | NuGet包资产清单 | ⭐ |

**重要性说明：**
- ⭐⭐⭐⭐⭐：核心文件，程序运行必需
- ⭐⭐⭐⭐：重要文件，影响主要功能
- ⭐⭐⭐：常用文件，影响特定功能
- ⭐⭐：辅助文件，影响细节功能
- ⭐：次要文件，可删除或忽略

---

## 捐助支持

如果这个项目对您有帮助，欢迎通过以下方式支持作者：

### 支付宝收款码
![支付宝收款码](images/alipay_qr.jpg)

### 微信收款码
![微信收款码](images/wechat_qr.jpg)

### 捐助说明
- 您的捐助将用于项目的持续开发和维护
- 捐助金额不限，感谢您的支持！
- 如有问题或建议，欢迎通过邮箱联系：a@mallerp.com

---

**感谢所有支持此项目的用户！** 🎉 