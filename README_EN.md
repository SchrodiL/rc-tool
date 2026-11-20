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

### Connecting

Plug the controller in via USB (wired mode or 2.4G mode both work). GameSir Connect does not
need to be running. The tool scans every 3 seconds: plug in to connect, unplug to disconnect,
re-plug to reconnect automatically.

### Patch (remove the forced RC reset)

GameSir Connect zeroes the RC values every time it opens. This tool removes that logic:

1. **Fully exit GameSir Connect** (including the tray icon);
2. Click **Remove RC Force-Reset** and wait for the progress lines
   (read → scan → write, a few seconds);
3. Start GameSir Connect again — it will no longer overwrite your RC values;
4. Click **Restore Official App** to undo at any time (an automatic backup is made before
   the first patch).

If the app folder is not found, click **Select Install Folder...** and point it to the
GameSir Connect installation directory.

### Tuning RC

1. Wait until the top indicator reads **Pad: G7P8K**;
2. Click a tab to pick the profile you want to edit (clicking a tab also switches the
   controller's active profile);
3. Drag the slider: the left side is the jitter direction (−10), the right side is smoothing
   (+10); the current value is shown right above the slider;
4. Click **Apply to Left/Right Stick** — the value is written to the controller and verified
   by reading it back. Success/failure is reported immediately;
5. Try it in game and fine-tune as needed.

Value range: `-10..-1` adds jitter; `0` = off; `1..10` = smoothing (higher = smoother).
Left and right sticks are configured independently. Start with smoothing 2~3 and go up if
needed. Note that the jitter direction is banned in some esports rulesets.

### Misc

- Language: click the small pill next to the model name to toggle Chinese / English;
- Settings are stored inside the controller's profiles and survive power-off, so they carry
  over to any PC;
- To reset a profile, drag the slider back to 0 and apply.

## Disclaimer

- Unofficial tool. Not affiliated with or endorsed by GameSir or ASUS.
- The title wordmark imitates the ASUS ROG brand typeface (ROG is a trademark of ASUS).
  It is decorative only and not covered by the open-source license.
- The patch is an equal-length local file edit with automatic backup and one-click restore;
  use at your own risk.

## License

MIT (see LICENSE). Third-party trademarks and font assets are not included in the license.
