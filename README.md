<img src="https://novashang.github.io/DicDic/img/logo-dark.svg" width="128">


# DicDic

极其轻巧又足够高效的桌面词典软件

- 单文件，仅5MB大小
- 全局快捷键一键唤出
- 内置15万词的离线词库
- 一键在线查词或在线搜索

# 开发与构建

- Clone该源后使用VS 2017打开DicDic.sln
- 使用默认设置生成项目

# 安装与运行说明

1. 使用上方的下载按钮下载最新版本或在[这里](https://github.com/NovaShang/DicDic/releases)查看所有的发布版本。
2. 运行下载的 `DicDic.exe`。

如需开机自启动，将该程序或该程序的快捷方式放入 `C:\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp` 位置即可。

# 使用说明

1. 点击系统托盘中的DicDic图标，或按下快捷键 `shift + Alt + D` 打开输入框。
2. 输入要查询的单词或搜索的关键字。
3. 按 `Tab` 键自动补全单词，按 `↑` 建或 `↓` 键在本地词库列表中滚动。
4. 按 `Enter` 键或点击查词按钮在线查词，按 `Ctrl + Enter` 键或点击搜索按钮在线搜索当前输入的关键词。 
5. 右键点击系统托盘图标，在菜单中可以退出本程序。

# 配置说明

- 在系统目录 `C:\Users\[你的用户名]\AppData\Local\Nova\DicDic_[一些随机字符]\[版本号]` 目录下可以找到 `user.config` 文件，使用文本编辑器修改该文件即可变更程序的设置。
- `searchUrl` 字段为程序使用的搜索引擎的地址，`"{KeyWord}"` 将被替换为输入的关键词。默认为Baidu,如需更换成Google，则将该字段改为 `https://www.google.co.jp/search?q={KeyWord}` 。
- `dicUrl` 字段为程序使用的在线词典的地址，`"{KeyWord}"` 将被替换为输入的关键词。
- `keyCode` 和 `keyModifier` 字段用来设置全局唤出快捷键，具体的值参见[该文档](https://msdn.microsoft.com/zh-cn/library/system.windows.forms.keys(v=vs.110).aspx)。
- 如需恢复默认设置，删除该文件即可。