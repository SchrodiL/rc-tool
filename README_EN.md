# RC-Tool

Stick RC-filter tuner for GameSir Uranus 8K (G7 Pro 8K), talking to the controller over direct USB HID.

[中文说明](README.md) | English

A single-file Windows utility written in plain C# WinForms. No installer, no third-party runtime needed
(compiles with the csc.exe that ships with .NET Framework 4.x).

## Features

- Direct USB HID access, works without GameSir Connect running
- Four profile tabs: profile names and values load automatically; clicking a tab switches the
  controller's active profile
- Independent left/right stick adjustment with a −10…+10 slider (negative = jitter,
  0 = off, positive = smoothing), verified by read-back after every write
- Automatic hot-plug detection and reconnect
- One-click removal of GameSir Connect's forced RC reset (equal-length patch to app.asar,
  with automatic backup and one-click restore)
- Chinese / English UI toggle; dark theme matching the official app

## Supported models

- **GameSir Uranus 8K / G7 Pro 8K** (G7ProCE, including all colorways and collab editions)
- Other models use different profile layouts or have no public firmware support yet

## Build

Double-click `build.cmd`. It compiles with the system `csc.exe` (.NET Framework 4.x) and produces
a ~120 KB single-file `RC-Tool.exe`, which is launched automatically on success.

## Usage

1. Connect the controller via USB (fully exit GameSir Connect first is recommended)
2. Run `RC-Tool.exe` and wait for "Pad: G7P8K" in the top-right corner
3. Pick a tab, drag the slider, click "Apply to Left/Right Stick"
4. Patch: fully exit GameSir Connect (including the tray icon), click "Remove RC Force-Reset",
   then restart the app

## HID protocol notes (verified on hardware)

| Item | Details |
|---|---|
| Output report | ID `0x0F`, 64 bytes |
| Input report | ID `0x10` for replies; `0x12` is the gamepad state stream (ignored) |
| Enter config mode | `[15,1]` → `10-06` Ack |
| Read profile chunk | `[15,4,profile,Hi,Lo,Len]` → `10-05-profile-Hi-Lo-Len-data` |
| Write profile chunk | `[15,3,profile,Hi,Lo,Len,data]`, Len ≤ 58 |
| Switch profile | `[15,7,profile]` |
| Current profile | `[15,11]` → `10-0C-profile` |
| Firmware version | `[15,9]` → `10-0A` (UTF-16LE) |
| Profile layout | 1070 bytes; name at 0..31; RC value at byte 49 (left) / 50 (right), `byte = clamp(1..21, value+11)` |

The transport uses fully-timed-over-out Overlapped I/O plus a 500 ms heartbeat, so a misbehaving
device can never freeze the UI.

## Disclaimer

- Unofficial tool. Not affiliated with or endorsed by GameSir or ASUS.
- The title wordmark imitates the ASUS ROG brand typeface (ROG is a trademark of ASUS).
  It is decorative only and not covered by the open-source license.
- The patch is an equal-length local file edit with automatic backup and one-click restore;
  use at your own risk.

## License

MIT (see LICENSE). Third-party trademarks and font assets are not included in the license.
