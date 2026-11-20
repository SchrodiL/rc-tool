# RC-Tool

GameSir 天王星8K / G7 Pro 8K 摇杆 RC 滤波调节工具。
*Stick RC-filter tuner for GameSir Uranus 8K (G7 Pro 8K), via direct USB HID.*

单文件 Windows 小工具，纯 C# WinForms，无需安装、无需第三方运行时
（.NET Framework 4.x 系统自带即可构建）。

## 功能

- 直连 USB HID，不依赖 GameSir Connect 运行
- 4 个配置档四个 Tab：自动读取档名与数值，点 Tab 即切换手柄当前档
- 左右摇杆独立调节，滑块 −10 ~ +10（负=增抖，0=关闭，正=防抖），写入后自动回读校验
- 自动检测插拔与重连，断线恢复后自动重读
- 一键去除 GameSir Connect 对 RC 值的强制归零（等长改写 app.asar，可一键恢复并自动备份）
- 中英文界面切换、官方同款深色配色

## 支持型号

- **GameSir 天王星8K / G7 Pro 8K**（G7ProCE，含全部配色与联名版）
- 其他型号的摇杆配置结构不同或未公开固件，暂不支持

## 构建

双击 `build.cmd`。使用系统自带的 `csc.exe`（.NET Framework 4.x）编译，
产物为约 120 KB 的单文件 `RC-Tool.exe`，编译成功后自动启动。

## 使用

1. USB 连接手柄（建议先完全退出 GameSir Connect）
2. 运行 `RC-Tool.exe`，等待右上角显示"手柄: G7P8K"
3. 选 Tab → 拖动滑块 → "应用到左/右摇杆"
4. 补丁：完全退出 GameSir Connect（含托盘）→ "去除RC强制归零" → 重启 App 生效

## 直连协议要点（真机实测）

| 项目 | 内容 |
|---|---|
| 发送报告 | ID `0x0F`，64 字节 |
| 应答报告 | ID `0x10`；`0x12` 为手柄状态流（忽略） |
| 进入配置 | `[15,1]` → `10-06` Ack |
| 读档块 | `[15,4,档,Hi,Lo,Len]` → `10-05-档-Hi-Lo-Len-data` |
| 写档块 | `[15,3,档,Hi,Lo,Len,data]`，Len ≤ 58 |
| 切档 | `[15,7,档]` |
| 当前档 | `[15,11]` → `10-0C-档号` |
| 固件版本 | `[15,9]` → `10-0A`（UTF-16LE） |
| 档结构 | 总长 1070；档名 0..31；RC 值 = 字节 49(左) / 50(右)，`字节 = clamp(1..21, 值+11)` |

传输层采用 Overlapped I/O 全超时 + 500ms 心跳保活，设备异常不会卡住界面。

## 免责声明

- 非官方工具，与 GameSir / ASUS 无关。
- 标题字标沿用 ASUS ROG 品牌字体风格（ROG 为 ASUS 商标），仅作界面装饰，不在开源许可范围内。
- 补丁为本地文件的等长改写并自动备份，可一键恢复；请自行承担使用风险。

## License

MIT（见 LICENSE），不含第三方商标与字体素材。
