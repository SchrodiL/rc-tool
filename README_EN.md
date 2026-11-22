# RC-Tool (stick RC-filter tuner for G7 Pro 8K / Uranus 8K)

A standalone USB-HID companion tool for the GameSir **G7 Pro 8K (a.k.a. Uranus 8K / G7ProCE)**
controller: tune the left/right stick RC filter (antiJitter: jitter / smoothing), manage the
"RC force-reset" patch for GameSir Connect, and flash firmware through the official upgrade
channel. Pure native C# WinForms single-file exe - no install, no Python, no browser, and it
creates zero files at runtime.

## Features

- **4 profiles = 4 tabs**: all profiles are read automatically on connect, real profile names
  are shown, the active profile is highlighted; clicking a tab switches the controller to it.
- **Independent left/right adjustment**: sliders -10..+10 (negative = jitter, 0 = off,
  positive = smoothing), value read-back verified after applying.
  Profile bytes 49 (left) / 50 (right); byte value = clamp(1..21, value + 11).
- **Fully automatic connection**: auto-detect every 3 s, reconnect on replug, UI reset on
  disconnect - no buttons needed.
- **Firmware flashing** (same channel as the official app, no app files modified):
  - Built-in official V2.10 / V2.38 images for one-click recovery;
  - Choosing "Select firmware file..." in the dropdown **opens the file picker immediately**;
    after picking, the file name is shown in the selection bar; flashing starts only when
    "Flash Firmware..." is clicked (any .ufw supported).
- **Firmware indicator**: the version reported by the controller is shown in the header -
  0938 (custom RC build) or 0210 (official V2.10, RC natively works) = green;
  other official versions (0219 / 0238 etc.) = orange (no RC); grey when disconnected.
- **Patch**: removes GameSir Connect's forced antiJitter reset (equal-length in-place edit of
  app.asar, one-click restore, auto backup as app.asar.rctool.bak); plus "Restart GameSir
  Connect" and "Select install folder...".
- **Chinese / English toggle** via the pill next to the model name.
- Styled after the official GameSir Connect app (near-black gradient + GameSir orange-red);
  the title is an embedded ROG-font wordmark.

## Files

| File | Description |
|---|---|
| `RC-Tool.exe` | Main program (release asset / build output), double-click to run |
| `RC-Tool.cs` | Source (C# 5, compiles with the system csc.exe) |
| `build.cmd` | One-click build & launch |
| `app.ico` | Program icon |

Optional files (not distributed in this repo): `fw_v210.ufw` / `fw_v238.ufw` (built-in
official firmware) and `jl_firmware_upgrade_x86.dll` (official upgrade engine). Place all
three next to the source before building to enable built-in firmware flashing.

## Usage

1. Connect the controller over USB (wired is fine; quit GameSir Connect's device page first);
2. Run `RC-Tool.exe`, wait for "Pad: G7P8K" in the header;
3. Pick a tab, drag the sliders, press "Apply to Left/Right stick";
4. Patch: fully quit GameSir Connect (including tray) -> "Remove RC Force-Reset" -> restart
   the app for it to take effect;
5. Flashing: select a firmware in the dropdown (or "Select firmware file..." for any .ufw),
   then click "Flash Firmware...". Takes ~30-90 s; the controller reboots automatically.
   Failures can simply be retried.

## How flashing works (identical to the official app)

1. Send `[15,23,85,136]` on the HID config interface - the controller enumerates as a U-disk
   upgrade device;
2. Call the official `jl_firmware_upgrade_x86.dll`:
   `JL_loadUfwData -> JL_getUfwPidVid -> JL_queryDevicePidVid -> JL_setDualBackupBurning(1) -> JL_upgradeDevice`,
   writing both banks with per-block verification; the controller reboots when done;
3. The upgrade DLL is embedded in the exe (extracted to `%TEMP%\RCTool\dfu\` at runtime), so
   flashing does not require GameSir Connect; the "install folder" setting is only used for
   the antiJitter patch.

## Direct HID protocol (calibrated on hardware)

- Output report ID `0x0F`, 64 bytes; device replies with report ID `0x10`
  (gamepad state stream `0x12` is ignored).
- Enter config mode `[15,1]` -> `10-06` ack (once per session).
- Read profile block `[15,4,profile,Hi,Lo,Len]` -> `10-05-profile-Hi-Lo-Len-data`;
  write `[15,3,profile,Hi,Lo,Len,data]`; switch `[15,7,profile]`;
  current profile `[15,11]` -> `10-0C-profile`; firmware version `[15,9]` -> `10-0A` (UTF-16LE).
- Profile length 1070 bytes; name = 0..31; RC value = byte 49 (left) / 50 (right).
- Transport uses **overlapped I/O with full timeouts and a 500 ms heartbeat**, so a misbehaving
  device never freezes the UI.

## Build

On Windows run `build.cmd` (requires .NET Framework 4.x, which ships csc.exe).
The tool is a **32-bit program** (the official upgrade DLL is x86). Without the optional
resource files, everything except built-in firmware still works (read/write profiles,
flash-from-file, patch).

## Changelog

### v1.1
- New firmware flashing row: built-in official V2.10 / V2.38 for instant recovery;
- "Select firmware file..." now shows the chosen file name right away and flashing starts
  only on the button press;
- Firmware version indicator (green = RC effective, orange = not); dropdown now closes when
  clicked again;
- Zero runtime files; fully overlapped HID I/O so device quirks never freeze the UI.

### v1.0
- First public release: 4-profile RC tuning, auto-connect, antiJitter force-reset patch,
  bilingual UI.

## Disclaimer

- The ROG font used in the title wordmark is copyright ASUS and is used for appearance only -
  do not redistribute this tool commercially.
- The jitter direction is banned in some esports leagues; check the rules before competing.
- Flashing firmware carries risk (an interruption may brick the device). Keep the connection
  stable and do not unplug during flashing; the built-in official firmware can be reflashed
  at any time.
- This project is not affiliated with GameSir.
