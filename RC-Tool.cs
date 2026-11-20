using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Microsoft.Win32;

namespace RcTool
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try { SetProcessDPIAware(); } catch { }
            Trace.Init();
            Trace.W("==== RC 控制台启动 v3.7 ====");
            Trace.W("exe=" + Application.ExecutablePath);
            // 未处理异常写日志
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += delegate (object s, System.Threading.ThreadExceptionEventArgs e)
            {
                LogCrash(e.Exception);
            };
            AppDomain.CurrentDomain.UnhandledException += delegate (object s, UnhandledExceptionEventArgs e)
            {
                LogCrash(e.ExceptionObject as Exception);
            };
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception e)
            {
                LogCrash(e);
                throw;
            }
            Trace.W("==== 正常退出 ====");
        }

        static void LogCrash(Exception e)
        {
            string text = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " +
                          (e != null ? e.ToString() : "unknown") + Environment.NewLine + Environment.NewLine;
            try
            {
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rc_error.log"), text, Encoding.UTF8);
            }
            catch { }
            try
            {
                if (Directory.Exists(Trace.SharedDir))
                    File.AppendAllText(Path.Combine(Trace.SharedDir, "rc_error.log"), text, Encoding.UTF8);
            }
            catch { }
            Trace.W("[异常] " + (e != null ? e.ToString() : "unknown"));
        }

        [DllImport("user32.dll")]
        static extern bool SetProcessDPIAware();
    }

    // 调试日志（exe 目录 + C:\gs-capture）
    static class Trace
    {
        public const string SharedDir = "C:\\gs-capture";
        static readonly object lk = new object();
        static string path1, path2;

        public static void Init()
        {
            path1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rc_debug.log");
            try { if (Directory.Exists(SharedDir)) path2 = Path.Combine(SharedDir, "rc_debug.log"); } catch { }
            try { File.WriteAllText(path1, "", Encoding.UTF8); } catch { }
            if (path2 != null) { try { File.WriteAllText(path2, "", Encoding.UTF8); } catch { } }
        }

        public static void W(string s)
        {
            string line = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + s + Environment.NewLine;
            lock (lk)
            {
                try { File.AppendAllText(path1, line, Encoding.UTF8); } catch { }
                if (path2 != null) { try { File.AppendAllText(path2, line, Encoding.UTF8); } catch { } }
            }
        }
    }

    // ---------------------------- 配色 ----------------------------
    static class C
    {
        // 配色：近黑底 + GameSir 橙红
        public static readonly Color Bg = ColorTranslator.FromHtml("#0d0b0f");
        public static readonly Color Card = ColorTranslator.FromHtml("#171319");
        public static readonly Color Card2 = ColorTranslator.FromHtml("#0f0d11");
        public static readonly Color Border = ColorTranslator.FromHtml("#2a2430");
        public static readonly Color BorderHi = ColorTranslator.FromHtml("#463d45");
        public static readonly Color Text = ColorTranslator.FromHtml("#f3eff1");
        public static readonly Color Muted = ColorTranslator.FromHtml("#a49ea6");
        public static readonly Color Dim = ColorTranslator.FromHtml("#6a636c");
        public static readonly Color Accent = ColorTranslator.FromHtml("#ff5c39");
        public static readonly Color Accent2 = ColorTranslator.FromHtml("#e0502e");
        public static readonly Color Ok = ColorTranslator.FromHtml("#5ee08f");
        public static readonly Color OkBg = ColorTranslator.FromHtml("#0f2318");
        public static readonly Color OkBr = ColorTranslator.FromHtml("#1e5236");
        public static readonly Color Warn = ColorTranslator.FromHtml("#ffb454");
        public static readonly Color WarnBg = ColorTranslator.FromHtml("#231c0e");
        public static readonly Color WarnBr = ColorTranslator.FromHtml("#55401d");
        public static readonly Color Jit = ColorTranslator.FromHtml("#ff5c39");
        public static readonly Color Btn = ColorTranslator.FromHtml("#231e26");
        public static readonly Color BtnHi = ColorTranslator.FromHtml("#2d2630");
        public static readonly Color TabActive = ColorTranslator.FromHtml("#2b1d1a");
        public static readonly Color SmoothColor = ColorTranslator.FromHtml("#ffd166");
        public static readonly Color BgTop = ColorTranslator.FromHtml("#161017");
        public static readonly Color BgBottom = ColorTranslator.FromHtml("#08070a");

        public static Color BgAt(float t)
        {
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            return Color.FromArgb(
                (int)(BgTop.R + (BgBottom.R - BgTop.R) * t),
                (int)(BgTop.G + (BgBottom.G - BgTop.G) * t),
                (int)(BgTop.B + (BgBottom.B - BgTop.B) * t));
        }
    }

    // 内嵌标题字标（PNG）
    static class LogoBake
    {
        public const string B64 =
            "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";
        public static System.Drawing.Image Load()
        {
            try
            {
                byte[] data = Convert.FromBase64String(B64);
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(data))
                    return new Bitmap(System.Drawing.Image.FromStream(ms));
            }
            catch { return null; }
        }
    }

    sealed class LogoBox : Control
    {
        public System.Drawing.Image Img;
        public LogoBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Ui.Under(this));
            if (Img == null) return;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float sc = Math.Min((float)Width / Img.Width, (float)Height / Img.Height);
            int w = (int)(Img.Width * sc);
            int h = (int)(Img.Height * sc);
            g.DrawImage(Img, new Rectangle(0, (Height - h) / 2, w, h));
        }
    }

    // 轻量 i18n：L.En=true 时界面用英文
    static class L
    {
        public static bool En;
        public static string T(string zh, string en) { return En ? en : zh; }
    }

    static class Ui
    {
        // 控件所在位置的底色
        public static Color Under(Control c)
        {
            Control p = c.Parent;
            if (p == null) return C.Bg;
            if (p is Form)
                return C.BgAt(p.Height > 0 ? (float)(c.Top + c.Height / 2) / p.Height : 0);
            return p.BackColor;
        }

        public static GraphicsPath RoundRect(Rectangle r, int rad)
        {
            int d = rad * 2;
            GraphicsPath p = new GraphicsPath();
            if (d <= 0) { p.AddRectangle(r); return p; }
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        public static void ApplyRoundRegion(Control c, int rad)
        {
            if (c.Width <= 0 || c.Height <= 0) return;
            Rectangle r = new Rectangle(0, 0, c.Width, c.Height);
            using (GraphicsPath p = RoundRect(r, rad)) c.Region = new Region(p);
        }

        public static string FmtVal(int v, out bool jitter)
        {
            jitter = false;
            if (v == 0) return "0";
            if (v > 0) return L.T("防抖 ", "Smooth ") + v;
            jitter = true;
            return L.T("增抖 ", "Jitter ") + (-v);
        }

        public static string TrimNull(string s)
        {
            return s == null ? "" : s.Trim(new char[] { '\0', ' ', '\t', '\r', '\n' });
        }
    }

    // ---------------------------- 自绘控件 ----------------------------
    class RoundButton : Control
    {
        public bool Primary;
        bool hover, down;

        public RoundButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Cursor = Cursors.Hand;
        }
        protected override void OnTextChanged(EventArgs e) { Invalidate(); base.OnTextChanged(e); }
        protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }
        protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { hover = false; down = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { down = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { down = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Ui.Under(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color bg = Primary ? C.Accent2 : (hover ? C.BtnHi : C.Btn);
            if (Primary && down) bg = C.Accent;
            if (!Primary && down) bg = C.Border;
            if (!Enabled) bg = C.Card2;
            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath p = Ui.RoundRect(r, 10))
            {
                using (SolidBrush b = new SolidBrush(bg)) g.FillPath(b, p);
                using (Pen pen = new Pen(C.Border)) g.DrawPath(pen, p);
            }
            Color fg = Primary ? Color.White : C.Text;
            if (!Enabled) fg = C.Dim;
            TextRenderer.DrawText(g, Text, Font, r, fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    class Pill : Control
    {
        public bool WhiteText;
        string state = "";
        public Pill()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
        public void SetState(string text, string cls)
        {
            Text = text; state = cls; Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Ui.Under(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color fg = C.Muted, bg = C.Card, br = C.Border;
            if (state == "ok") { fg = C.Ok; bg = C.OkBg; br = C.OkBr; }
            if (state == "warn") { fg = C.Warn; bg = C.WarnBg; br = C.WarnBr; }
            if (WhiteText) fg = C.Text;
            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath p = Ui.RoundRect(r, Height / 2))
            {
                using (SolidBrush b = new SolidBrush(bg)) g.FillPath(b, p);
                using (Pen pen = new Pen(br)) g.DrawPath(pen, p);
            }
            TextRenderer.DrawText(g, Text, Font, r, fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    class TabButton : Control
    {
        public bool Active;
        public bool Current;
        public TabButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Cursor = Cursors.Hand;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Ui.Under(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath p = Ui.RoundRect(r, 11))
            {
                using (SolidBrush b = new SolidBrush(Active ? C.TabActive : C.Card2)) g.FillPath(b, p);
                using (Pen pen = new Pen(Active ? C.Accent : C.Border)) g.DrawPath(pen, p);
            }
            using (SolidBrush dot = new SolidBrush(Current ? C.Ok : C.BorderHi))
                g.FillEllipse(dot, 14, Height / 2 - 4, 8, 8);
            TextRenderer.DrawText(g, Text, Font,
                new Rectangle(30, 0, Width - 36, Height), Active ? Color.White : C.Muted,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }

    class SliderCtrl : Control
    {
        int min = -10, max = 10, val = 0;
        bool drag;
        public int? DisplayValue;
        public event EventHandler ValueChanged;

        public SliderCtrl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Cursor = Cursors.Hand;
        }
        public int Value
        {
            get { return val; }
            set
            {
                int v = Math.Max(min, Math.Min(max, value));
                if (v == val) return;
                val = v;
                Invalidate();
                if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
            }
        }
        public void SetValueSilent(int v)
        {
            val = Math.Max(min, Math.Min(max, v));
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e) { drag = true; SetFromMouse(e.X); base.OnMouseDown(e); }
        protected override void OnMouseMove(MouseEventArgs e) { if (drag) SetFromMouse(e.X); base.OnMouseMove(e); }
        protected override void OnMouseUp(MouseEventArgs e) { drag = false; base.OnMouseUp(e); }
        void SetFromMouse(int x)
        {
            int pad = 12;
            int w = Width - pad * 2;
            if (w <= 0) return;
            double f = (x - pad) / (double)w;
            f = Math.Max(0, Math.Min(1, f));
            Value = (int)Math.Round(min + f * (max - min));
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Ui.Under(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int pad = 12;
            int cy = Height - 21;
            Rectangle track = new Rectangle(pad, cy - 5, Width - pad * 2, 10);
            using (GraphicsPath p = Ui.RoundRect(track, 5))
            using (SolidBrush b = new SolidBrush(ColorTranslator.FromHtml("#2a2330")))
                g.FillPath(b, p);
            double f = (val - min) / (double)(max - min);
            int kx = pad + (int)Math.Round(f * (Width - pad * 2));
            using (SolidBrush b = new SolidBrush(ColorTranslator.FromHtml("#f3eff1")))
                g.FillEllipse(b, kx - 11, cy - 11, 22, 22);
            using (Pen pen = new Pen(C.Accent, 3))
                g.DrawEllipse(pen, kx - 11, cy - 11, 22, 22);
            // 数值文字：按实际宽度居中
            string vtxt = DisplayValue.HasValue ? DisplayValue.Value.ToString() : "0";
            Size ts = TextRenderer.MeasureText(g, vtxt, Font,
                new Size(1000, 24), TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
            int tx = kx - ts.Width / 2;
            if (tx < 0) tx = 0;
            if (tx + ts.Width > Width) tx = Width - ts.Width;
            TextRenderer.DrawText(g, vtxt, Font, new Rectangle(tx, 0, ts.Width, 24), Color.White,
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter);
        }
    }

    class CardPanel : Panel
    {
        public int Radius = 16;
        public CardPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = C.Card;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color under = C.Bg;
            if (Parent != null && Parent.Height > 0)
                under = C.BgAt((float)(Top + Height / 2) / Parent.Height);
            g.Clear(under);
            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath p = Ui.RoundRect(r, Radius))
            {
                using (SolidBrush b = new SolidBrush(BackColor)) g.FillPath(b, p);
                using (Pen pen = new Pen(C.Border)) g.DrawPath(pen, p);
            }
        }
    }

    // ---------------------------- GameSir Connect 管理（路径 / 补丁 / 启动） ----------------------------
    class GameSirApp
    {
        public string Dir;
        string CfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rc_config.json");

        static readonly JavaScriptSerializer Ser = new JavaScriptSerializer();

        public GameSirApp()
        {
            try
            {
                if (File.Exists(CfgPath))
                {
                    Dictionary<string, object> cfg = Ser.DeserializeObject(File.ReadAllText(CfgPath, Encoding.UTF8)) as Dictionary<string, object>;
                    if (cfg != null && cfg.ContainsKey("gamesir_dir")) Dir = Convert.ToString(cfg["gamesir_dir"]);
                }
            }
            catch { }
            if (!Valid(Dir)) Dir = null;
        }

        public static bool Valid(string d)
        {
            return !string.IsNullOrEmpty(d) && File.Exists(Path.Combine(d, "resources", "app.asar"));
        }

        public bool SetDir(string d)
        {
            if (!Valid(d)) return false;
            Dir = d;
            SaveCfg();
            return true;
        }

        void SaveCfg()
        {
            try
            {
                Dictionary<string, object> cfg = new Dictionary<string, object>();
                cfg["gamesir_dir"] = Dir;
                File.WriteAllText(CfgPath, Ser.Serialize(cfg), Encoding.UTF8);
            }
            catch { }
        }

        public string ExePath { get { return Dir == null ? null : Path.Combine(Dir, "GameSir Connect.exe"); } }
        public string AsarPath { get { return Dir == null ? null : Path.Combine(Dir, "resources", "app.asar"); } }
        public string BackupPath { get { return Dir == null ? null : Path.Combine(Dir, "resources", "app.asar.rctool.bak"); } }

        public string Locate(bool deep)
        {
            List<string> cands = new List<string>();
            if (Valid(Dir)) cands.Add(Dir);
            // 进程路径
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("powershell",
                    "-NoProfile -Command \"(Get-Process 'GameSir Connect' -ErrorAction SilentlyContinue | Select-Object -First 1).Path\"");
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.CreateNoWindow = true;
                Process p = Process.Start(psi);
                string outp = p.StandardOutput.ReadToEnd();
                p.WaitForExit(8000);
                outp = Ui.TrimNull(outp);
                if (outp.Length > 0 && File.Exists(outp)) cands.Add(Path.GetDirectoryName(outp));
            }
            catch { }
            // 注册表
            try
            {
                RegistryView[] views = { RegistryView.Registry64, RegistryView.Registry32 };
                RegistryHive[] hives = { RegistryHive.CurrentUser, RegistryHive.LocalMachine };
                foreach (RegistryHive hive in hives)
                {
                    foreach (RegistryView view in views)
                    {
                        try
                        {
                            using (RegistryKey baseKey = RegistryKey.OpenBaseKey(hive, view))
                            using (RegistryKey uninst = baseKey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall"))
                            {
                                if (uninst == null) continue;
                                foreach (string subName in uninst.GetSubKeyNames())
                                {
                                    try
                                    {
                                        using (RegistryKey sub = uninst.OpenSubKey(subName))
                                        {
                                            if (sub == null) continue;
                                            object dn = sub.GetValue("DisplayName");
                                            if (dn == null) continue;
                                            string name = Convert.ToString(dn);
                                            if (name.IndexOf("gamesir", StringComparison.OrdinalIgnoreCase) < 0) continue;
                                            string[] props = { "InstallLocation", "DisplayIcon", "UninstallString" };
                                            foreach (string prop in props)
                                            {
                                                object v = sub.GetValue(prop);
                                                if (v == null) continue;
                                                string s = Convert.ToString(v).Trim(new char[] { '"' });
                                                int comma = s.IndexOf(',');
                                                if (comma > 0) s = s.Substring(0, comma).Trim(new char[] { '"' });
                                                AddCandidate(cands, s);
                                                AddCandidate(cands, SafeDirName(s));
                                                string up = SafeDirName(s);
                                                if (up != null) AddCandidate(cands, SafeDirName(up));
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
            // 常见目录
            string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string pf86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            AddCandidate(cands, Path.Combine(local, "Programs", "GameSir Connect"));
            AddCandidate(cands, Path.Combine(local, "Programs", "GameSirConnect"));
            AddCandidate(cands, Path.Combine(pf, "GameSir Connect"));
            AddCandidate(cands, Path.Combine(pf86, "GameSir Connect"));

            foreach (string d in cands)
            {
                if (Valid(d)) { Dir = d; SaveCfg(); return d; }
            }

            if (deep)
            {
                string[] roots = { local, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), pf, pf86 };
                foreach (string root in roots)
                {
                    string hit = DeepSearch(root, 5);
                    if (hit != null) { Dir = hit; SaveCfg(); return hit; }
                }
            }
            return null;
        }

        static void AddCandidate(List<string> cands, string d)
        {
            if (!string.IsNullOrEmpty(d) && Directory.Exists(d) && Valid(d) && !cands.Contains(d))
                cands.Add(d);
        }

        static string SafeDirName(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) return null;
                return Path.GetDirectoryName(path);
            }
            catch { return null; }
        }

        static string DeepSearch(string root, int maxDepth)
        {
            if (string.IsNullOrEmpty(root) || !Directory.Exists(root)) return null;
            try
            {
                Stack<KeyValuePair<string, int>> stack = new Stack<KeyValuePair<string, int>>();
                stack.Push(new KeyValuePair<string, int>(root, 0));
                int visited = 0;
                while (stack.Count > 0)
                {
                    KeyValuePair<string, int> cur = stack.Pop();
                    visited++;
                    if (visited > 60000) return null;
                    try
                    {
                        string asar = Path.Combine(cur.Key, "resources", "app.asar");
                        if (File.Exists(asar))
                        {
                            string name = Path.GetFileName(cur.Key);
                            if (name != null && name.ToLowerInvariant().StartsWith("gamesir"))
                                return cur.Key;
                        }
                    }
                    catch { }
                    if (cur.Value >= maxDepth) continue;
                    try
                    {
                        foreach (string sub in Directory.GetDirectories(cur.Key))
                        {
                            string nm = Path.GetFileName(sub).ToLowerInvariant();
                            if (nm == "windows" || nm == "node_modules" || nm == "cache" ||
                                nm == "$recycle.bin" || nm == "system volume information") continue;
                            stack.Push(new KeyValuePair<string, int>(sub, cur.Value + 1));
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return null;
        }

        // ---- 补丁（去除 antiJitter 强制归零）----
        static readonly byte[] AnchorBytes = Encoding.ASCII.GetBytes("h.warn(\"[G7ProCE][MainInfo] temporary antiJitter reset\"");
        static readonly byte[] CondOrig = Encoding.ASCII.GetBytes("if(0!=a||0!=n){");
        static readonly byte[] CondPatched = Encoding.ASCII.GetBytes("if(0!=a&&0!=0){");

        static int IndexOf(byte[] hay, byte[] needle, int from)
        {
            for (int i = from; i <= hay.Length - needle.Length; i++)
            {
                bool ok = true;
                for (int j = 0; j < needle.Length; j++)
                    if (hay[i + j] != needle[j]) { ok = false; break; }
                if (ok) return i;
            }
            return -1;
        }

        static int LastIndexOf(byte[] hay, int start, int end, byte[] needle)
        {
            for (int i = end - needle.Length; i >= start; i--)
            {
                bool ok = true;
                for (int j = 0; j < needle.Length; j++)
                    if (hay[i + j] != needle[j]) { ok = false; break; }
                if (ok) return i;
            }
            return -1;
        }

        // ---- 补丁状态缓存 ----
        object patchLock = new object();
        object patchFileLock = new object();   // 扫描与写入互斥（读共享会挡住写）
        string patchCacheStatus = "";
        string patchCacheKey = "";
        volatile bool patchScanQueued;
        public Action PatchStatusChanged;
        public Action<string> PatchProgress;

        void Report(string msg)
        {
            Action<string> cb = PatchProgress;
            if (cb != null) { try { cb(msg); } catch { } }
        }

        string AsarKey()
        {
            try
            {
                FileInfo fi = new FileInfo(AsarPath);
                return fi.Length.ToString() + ":" + fi.LastWriteTimeUtc.Ticks.ToString();
            }
            catch { return null; }
        }

        // 只查缓存，过期后台扫描
        public string PatchStatusCached()
        {
            if (AsarPath == null || !File.Exists(AsarPath)) return "missing";
            string key = AsarKey();
            lock (patchLock)
            {
                if (key == null) return "unknown";
                if (key == patchCacheKey && patchCacheStatus.Length > 0) return patchCacheStatus;
            }
            RequestPatchScan();
            lock (patchLock)
                return patchCacheStatus.Length > 0 ? patchCacheStatus : "scanning";
        }

        public void RequestPatchScan()
        {
            if (patchScanQueued) return;
            patchScanQueued = true;
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    string st = PatchStatus();
                    string key = AsarKey();
                    lock (patchLock)
                    {
                        patchCacheStatus = st;
                        patchCacheKey = key != null ? key : "";
                    }
                    Action cb = PatchStatusChanged;
                    if (cb != null) { try { cb(); } catch { } }
                }
                catch { }
                finally { patchScanQueued = false; }
            });
        }

        void SetPatchCache(string st)
        {
            lock (patchLock)
            {
                patchCacheStatus = st;
                string key = AsarKey();
                patchCacheKey = key != null ? key : "";
            }
        }

        // 真实扫描（后台线程用）
        public string PatchStatus()
        {
            if (AsarPath == null || !File.Exists(AsarPath)) return "missing";
            try
            {
                lock (patchFileLock)
                {
                    byte[] data = File.ReadAllBytes(AsarPath);
                    int idx = IndexOf(data, AnchorBytes, 0);
                    if (idx < 0) return "unknown";
                    int start = Math.Max(0, idx - 800);
                    if (LastIndexOf(data, start, idx, CondPatched) >= 0) return "patched";
                    if (LastIndexOf(data, start, idx, CondOrig) >= 0) return "clean";
                    return "unknown";
                }
            }
            catch { return "unknown"; }
        }

        public static int GameSirProcessCount()
        {
            try { return Process.GetProcessesByName("GameSir Connect").Length; }
            catch { return 0; }
        }

        static string PatchBusyHint(Exception e)
        {
            int code = e != null ? (e.HResult & 0xFFFF) : 0;
            string extra = code > 0 ? (" (Win32 " + code + ")") : "";
            int n = GameSirProcessCount();
            if (n > 0)
                return "GameSir Connect 正在运行（" + n + " 个进程），请完全退出它（包括右下角托盘图标）后重试" + extra;
            return "app.asar 被其他程序占用" + extra + "，请关闭占用程序后重试";
        }

        // 原地写入字节
        static void WriteBytesAt(string path, int offset, byte[] bytes)
        {
            Exception last = null;
            for (int attempt = 0; attempt < 2; attempt++)
            {
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Flush(true);
                    }
                    return;
                }
                catch (Exception e)
                {
                    last = e;
                    if (attempt == 0) Thread.Sleep(300);
                }
            }
            throw new Exception(PatchBusyHint(last));
        }

        public string ApplyPatch()
        {
            if (AsarPath == null || !File.Exists(AsarPath)) throw new Exception("未选择 App 路径");
            lock (patchFileLock)
            {
                Report("读取 app.asar（约 119 MB）…");
                byte[] data = File.ReadAllBytes(AsarPath);
                Report("扫描补丁锚点…");
                int idx = IndexOf(data, AnchorBytes, 0);
                if (idx < 0) throw new Exception("找不到补丁锚点（App 版本可能已变化）");
                int start = Math.Max(0, idx - 800);
                if (LastIndexOf(data, start, idx, CondPatched) >= 0) { SetPatchCache("patched"); return "already"; }
                int cidx = LastIndexOf(data, start, idx, CondOrig);
                if (cidx < 0) throw new Exception("锚点附近找不到条件语句");
                // 首次备份
                if (!File.Exists(BackupPath))
                {
                    Report("备份 app.asar…");
                    try { File.Copy(AsarPath, BackupPath, false); }
                    catch (Exception e) { throw new Exception("备份失败：" + PatchBusyHint(e)); }
                }
                Report("写入补丁（原地改写 15 字节）…");
                WriteBytesAt(AsarPath, cidx, CondPatched);
                SetPatchCache("patched");
                return "ok";
            }
        }

        public string RevertPatch()
        {
            if (AsarPath == null || !File.Exists(AsarPath)) throw new Exception("未选择 App 路径");
            lock (patchFileLock)
            {
                Report("检查当前补丁状态…");
                string st = PatchStatus();
                if (st != "patched") { SetPatchCache(st); return "clean"; }
                Report("读取 app.asar（约 119 MB）…");
                byte[] data = File.ReadAllBytes(AsarPath);
                int idx = IndexOf(data, CondPatched, 0);
                if (idx < 0) throw new Exception("找不到已打补丁的位置");
                Report("恢复原始字节…");
                WriteBytesAt(AsarPath, idx, CondOrig);
                SetPatchCache("clean");
                return "restored";
            }
        }

        public string Restart()
        {
            int n = GameSirProcessCount();
            if (n > 0)
            {
                try
                {
                    foreach (Process p in Process.GetProcessesByName("GameSir Connect"))
                    {
                        try { p.Kill(); } catch { }
                    }
                    // 等进程完全退出（app.asar 解锁）
                    for (int i = 0; i < 40 && GameSirProcessCount() > 0; i++) Thread.Sleep(250);
                }
                catch { }
            }
            if (ExePath == null || !File.Exists(ExePath))
                throw new Exception("未找到 GameSir Connect.exe（请先选择安装路径）");
            Process.Start(ExePath);
            return n > 0 ? "restarted" : "started";
        }

        public bool LaunchPlain()
        {
            if (ExePath == null || !File.Exists(ExePath))
                throw new Exception("未找到 GameSir Connect.exe（请先选择安装路径）");
            Process.Start(ExePath);
            return true;
        }
    }

    // ---------------------------- 直连 HID ----------------------------
    class HidDirect
    {
        const int DIGCF_PRESENT = 0x2;
        const int DIGCF_DEVICEINTERFACE = 0x10;
        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint OPEN_EXISTING = 3;
        const uint FILE_FLAG_OVERLAPPED = 0x40000000;

        public const int CHUNK = 58;
        public const int PROFILE_LEN = 1070;   // G7ProCE (G7 Pro 8K) 档总长，App 侧 EveryPacketLength
        public const int OFF_LS = 49;     // FunData[17]  ls_AntiJitter
        public const int OFF_RS = 50;     // FunData[18]  rs_AntiJitter

        [StructLayout(LayoutKind.Sequential)]
        struct GUID { public uint Data1; public ushort Data2; public ushort Data3; [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public byte[] Data4; }

        [StructLayout(LayoutKind.Sequential)]
        struct SP_DEVICE_INTERFACE_DATA { public uint cbSize; public GUID InterfaceClassGuid; public uint Flags; public IntPtr Reserved; }

        [StructLayout(LayoutKind.Sequential)]
        struct HIDD_ATTRIBUTES { public uint Size; public ushort VendorID; public ushort ProductID; public ushort VersionNumber; }

        [StructLayout(LayoutKind.Sequential)]
        struct HIDP_CAPS
        {
            public ushort Usage; public ushort UsagePage;
            public ushort InputReportByteLength; public ushort OutputReportByteLength;
            public ushort FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)] public ushort[] Reserved;
            public ushort NumberLinkCollectionNodes;
            public ushort NumberInputButtonCaps; public ushort NumberInputValueCaps; public ushort NumberInputDataIndices;
            public ushort NumberOutputButtonCaps; public ushort NumberOutputValueCaps; public ushort NumberOutputDataIndices;
            public ushort NumberFeatureButtonCaps; public ushort NumberFeatureValueCaps; public ushort NumberFeatureDataIndices;
        }

        [DllImport("hid.dll")] static extern void HidD_GetHidGuid(out GUID guid);
        [DllImport("hid.dll")] static extern bool HidD_GetAttributes(IntPtr h, ref HIDD_ATTRIBUTES attr);
        [DllImport("hid.dll")] static extern bool HidD_GetPreparsedData(IntPtr h, out IntPtr pp);
        [DllImport("hid.dll")] static extern bool HidD_FreePreparsedData(IntPtr pp);
        [DllImport("hid.dll")] static extern int HidP_GetCaps(IntPtr pp, ref HIDP_CAPS caps);
        [DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr SetupDiGetClassDevs(ref GUID guid, IntPtr Enumerator, IntPtr hwnd, int flags);
        [DllImport("setupapi.dll")]
        static extern bool SetupDiEnumDeviceInterfaces(IntPtr h, IntPtr devInfo, ref GUID guid, int index, ref SP_DEVICE_INTERFACE_DATA data);
        [DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
        static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr h, ref SP_DEVICE_INTERFACE_DATA data, IntPtr detail, int size, ref int needed, IntPtr devInfo);
        [DllImport("setupapi.dll")]
        static extern bool SetupDiDestroyDeviceInfoList(IntPtr h);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr CreateFile(string name, uint access, uint share, IntPtr sec, uint dispo, uint flags, IntPtr templ);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadFile(IntPtr h, byte[] buf, int n, out int read, IntPtr ov);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteFile(IntPtr h, byte[] buf, int n, out int written, IntPtr ov);
        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr h);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateEvent(IntPtr sec, bool manual, bool initial, IntPtr name);
        [DllImport("kernel32.dll")] static extern uint WaitForSingleObject(IntPtr h, uint ms);
        [DllImport("kernel32.dll")] static extern bool ResetEvent(IntPtr h);
        [DllImport("kernel32.dll")]
        static extern bool GetOverlappedResult(IntPtr hFile, IntPtr lpOverlapped, out int transferred, bool wait);
        [DllImport("kernel32.dll")]
        static extern bool CancelIoEx(IntPtr hFile, IntPtr lpOverlapped);

        [DllImport("hid.dll", CharSet = CharSet.Unicode)]
        static extern bool HidD_GetProductString(IntPtr h, StringBuilder buf, int len);
        [DllImport("hid.dll", CharSet = CharSet.Unicode)]
        static extern bool HidD_GetManufacturerString(IntPtr h, StringBuilder buf, int len);
        [DllImport("hid.dll", CharSet = CharSet.Unicode)]
        static extern bool HidD_GetSerialNumberString(IntPtr h, StringBuilder buf, int len);

        public class Device
        {
            public string Path;
            public ushort Vid, Pid, Version, UsagePage, Usage, InLen, OutLen;
            public string Product = "";
            public string Manufacturer = "";
            public string Serial = "";
        }

        public List<Device> EnumDevices()
        {
            return EnumerateAll(true);
        }

        List<Device> EnumerateAll(bool onlyGameSir)
        {
            GUID hg = new GUID();
            HidD_GetHidGuid(out hg);
            IntPtr devs = SetupDiGetClassDevs(ref hg, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
            if (devs == (IntPtr)(-1)) throw new Exception("SetupDiGetClassDevs 失败");
            List<Device> list = new List<Device>();
            SP_DEVICE_INTERFACE_DATA did = new SP_DEVICE_INTERFACE_DATA();
            did.cbSize = (uint)Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA));
            int idx = 0;
            while (SetupDiEnumDeviceInterfaces(devs, IntPtr.Zero, ref hg, idx, ref did))
            {
                idx++;
                int need = 0;
                SetupDiGetDeviceInterfaceDetail(devs, ref did, IntPtr.Zero, 0, ref need, IntPtr.Zero);
                if (need <= 0) continue;
                IntPtr buf = Marshal.AllocHGlobal(need);
                try
                {
                    Marshal.WriteInt32(buf, IntPtr.Size == 8 ? 8 : 6);
                    if (!SetupDiGetDeviceInterfaceDetail(devs, ref did, buf, need, ref need, IntPtr.Zero))
                        continue;
                    string path = Marshal.PtrToStringUni(new IntPtr(buf.ToInt64() + 4));
                    Device d = ProbeDevice(path, onlyGameSir);
                    if (d != null) list.Add(d);
                }
                finally { Marshal.FreeHGlobal(buf); }
            }
            SetupDiDestroyDeviceInfoList(devs);
            return list;
        }

        Device ProbeDevice(string path, bool requireGameSirVid)
        {
            IntPtr h = CreateFile(path, 0, 3, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (h == (IntPtr)(-1)) return null;
            try
            {
                HIDD_ATTRIBUTES attr = new HIDD_ATTRIBUTES();
                attr.Size = (uint)Marshal.SizeOf(typeof(HIDD_ATTRIBUTES));
                if (!HidD_GetAttributes(h, ref attr)) return null;
                if (requireGameSirVid && attr.VendorID != 0x3537) return null;
                Device d = new Device();
                d.Path = path;
                d.Vid = attr.VendorID; d.Pid = attr.ProductID; d.Version = attr.VersionNumber;
                d.InLen = 64; d.OutLen = 64;
                IntPtr pp;
                if (HidD_GetPreparsedData(h, out pp))
                {
                    try
                    {
                        HIDP_CAPS caps = new HIDP_CAPS();
                        caps.Reserved = new ushort[17];
                        if (HidP_GetCaps(pp, ref caps) >= 0)
                        {
                            d.UsagePage = caps.UsagePage; d.Usage = caps.Usage;
                            d.InLen = caps.InputReportByteLength;
                            d.OutLen = caps.OutputReportByteLength;
                        }
                    }
                    finally { HidD_FreePreparsedData(pp); }
                }
                try
                {
                    StringBuilder sb = new StringBuilder(260);
                    if (HidD_GetProductString(h, sb, 520)) d.Product = sb.ToString();
                    sb = new StringBuilder(260);
                    if (HidD_GetManufacturerString(h, sb, 520)) d.Manufacturer = sb.ToString();
                    sb = new StringBuilder(260);
                    if (HidD_GetSerialNumberString(h, sb, 520)) d.Serial = sb.ToString();
                }
                catch { }
                return d;
            }
            finally { CloseHandle(h); }
        }

        public Device PickConfig(List<Device> devs)
        {
            foreach (Device d in devs) if (d.UsagePage == 0xFFF0) return d;
            return devs.Count > 0 ? devs[0] : null;
        }

        // ---------- 会话（长连接 + 心跳 + 全超时 I/O） ----------
        public class Session : IDisposable
        {
            IntPtr h, evRead, evWrite, ovRead, ovWrite;
            public int InLen, OutLen;
            public bool ConfigEntered;
            readonly List<byte[]> packets = new List<byte[]>();
            readonly object writeLock = new object();
            volatile bool stop;
            volatile bool ioDead;
            Thread reader, beater;
            DateTime lastRx = DateTime.UtcNow;

            static int OvSize { get { return IntPtr.Size == 8 ? 32 : 20; } }
            static int OvEventOffset { get { return IntPtr.Size == 8 ? 24 : 16; } }

            static void ZeroOv(IntPtr ov, IntPtr ev)
            {
                for (int i = 0; i < OvSize; i++) Marshal.WriteByte(ov, i, 0);
                Marshal.WriteIntPtr(ov, OvEventOffset, ev);
            }

            public Session(Device dev)
            {
                h = CreateFile(dev.Path, GENERIC_READ | GENERIC_WRITE, 3, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, IntPtr.Zero);
                if (h == (IntPtr)(-1))
                {
                    int err = Marshal.GetLastWin32Error();
                    string hint;
                    if (err == 5) hint = "系统拒绝访问（接口被驱动或其他程序独占）";
                    else if (err == 32) hint = "被其他程序独占——请完全退出 GameSir Connect（含托盘）和其他手柄工具后重试";
                    else if (err == 1167) hint = "设备不可用（可能已被拔出）";
                    else if (err == 2) hint = "设备路径已失效（重新插拔手柄后再试）";
                    else hint = "未知错误";
                    throw new Exception("打开手柄失败 (Win32 " + err + "): " + hint);
                }
                InLen = dev.InLen > 0 ? dev.InLen : 64;
                OutLen = dev.OutLen > 0 ? dev.OutLen : 64;
                evRead = CreateEvent(IntPtr.Zero, true, false, IntPtr.Zero);
                evWrite = CreateEvent(IntPtr.Zero, true, false, IntPtr.Zero);
                ovRead = Marshal.AllocHGlobal(OvSize);
                ovWrite = Marshal.AllocHGlobal(OvSize);
                reader = new Thread(ReadLoop);
                reader.IsBackground = true;
                reader.Start();
                beater = new Thread(HeartbeatLoop);
                beater.IsBackground = true;
                beater.Start();
                Thread.Sleep(40);
            }

            // 心跳 500ms：[15,242,0]
            void HeartbeatLoop()
            {
                while (!stop)
                {
                    try { Send(Cmd(0x0F, 0xF2, 0x00)); } catch { }
                    Thread.Sleep(500);
                }
            }

            void ReadLoop()
            {
                byte[] buf = new byte[InLen];
                GCHandle pin = GCHandle.Alloc(buf, GCHandleType.Pinned);
                try
                {
                    while (!stop)
                    {
                        ZeroOv(ovRead, evRead);
                        int read;
                        bool ok = ReadFile(h, buf, InLen, out read, ovRead);
                        if (!ok)
                        {
                            int err = Marshal.GetLastWin32Error();
                            if (err != 997) { ioDead = true; break; }
                            // 100ms 步进等待，检查退出
                            while (!stop)
                            {
                                if (WaitForSingleObject(evRead, 100) == 0) break;
                            }
                            if (stop)
                            {
                                try { CancelIoEx(h, ovRead); } catch { }
                                int trc;
                                try { GetOverlappedResult(h, ovRead, out trc, true); } catch { }
                                break;
                            }
                            int tr;
                            if (!GetOverlappedResult(h, ovRead, out tr, false)) { ioDead = true; break; }
                            read = tr;
                        }
                        ResetEvent(evRead);
                        if (read > 0)
                        {
                            lastRx = DateTime.UtcNow;
                            if (buf[0] == 0x10)   // 只收配置应答（0x12 是手柄状态流，跳过）
                            {
                                byte[] copy = new byte[read];
                                Array.Copy(buf, copy, read);
                                lock (packets) packets.Add(copy);
                            }
                        }
                    }
                }
                finally { pin.Free(); }
            }

            // 存活判定：6 秒无报文视为掉线
            public bool Alive
            {
                get { return !stop && !ioDead && (DateTime.UtcNow - lastRx).TotalSeconds < 6; }
            }

            // 发送：Overlapped 写 + 300ms 超时
            public bool Send(byte[] cmd)
            {
                if (stop || ioDead) return false;
                byte[] outb = new byte[OutLen];
                Array.Copy(cmd, 0, outb, 0, Math.Min(cmd.Length, OutLen));
                lock (writeLock)
                {
                    GCHandle pin = GCHandle.Alloc(outb, GCHandleType.Pinned);
                    try
                    {
                        ZeroOv(ovWrite, evWrite);
                        int written;
                        bool ok = WriteFile(h, outb, OutLen, out written, ovWrite);
                        if (!ok)
                        {
                            int err = Marshal.GetLastWin32Error();
                            if (err != 997) return false;
                            if (WaitForSingleObject(evWrite, 300) != 0)
                            {
                                try { CancelIoEx(h, ovWrite); } catch { }
                                int tr0;
                                try { GetOverlappedResult(h, ovWrite, out tr0, true); } catch { }
                                return false;
                            }
                            int tr;
                            if (!GetOverlappedResult(h, ovWrite, out tr, false)) return false;
                        }
                        ResetEvent(evWrite);
                        return true;
                    }
                    finally { pin.Free(); }
                }
            }

            public void Drain(int ms)
            {
                Thread.Sleep(ms > 0 ? ms : 0);
                lock (packets) packets.Clear();
            }

            public byte[] WaitFor(Predicate<byte[]> pred, int timeoutMs)
            {
                DateTime t0 = DateTime.Now;
                while ((DateTime.Now - t0).TotalMilliseconds < timeoutMs)
                {
                    lock (packets)
                    {
                        for (int i = 0; i < packets.Count; i++)
                        {
                            if (pred(packets[i]))
                            {
                                byte[] hit = packets[i];
                                packets.RemoveAt(i);
                                return hit;
                            }
                        }
                    }
                    Thread.Sleep(5);
                }
                return null;
            }

            public void Dispose()
            {
                if (stop) return;
                stop = true;
                try { CancelIoEx(h, IntPtr.Zero); } catch { }
                try { CloseHandle(h); } catch { }
                try { if (reader != null) reader.Join(500); } catch { }
                try { if (beater != null) beater.Join(500); } catch { }
                // 线程退出后释放重叠结构
                bool readerGone = reader == null || !reader.IsAlive;
                bool beaterGone = beater == null || !beater.IsAlive;
                if (readerGone && beaterGone)
                {
                    try { if (ovRead != IntPtr.Zero) Marshal.FreeHGlobal(ovRead); } catch { }
                    try { if (ovWrite != IntPtr.Zero) Marshal.FreeHGlobal(ovWrite); } catch { }
                    try { CloseHandle(evRead); } catch { }
                    try { CloseHandle(evWrite); } catch { }
                }
            }
        }

        public Session OpenSession(Device dev)
        {
            return new Session(dev);
        }

        static byte[] Cmd(params int[] vals)
        {
            byte[] b = new byte[vals.Length];
            for (int i = 0; i < vals.Length; i++) b[i] = (byte)vals[i];
            return b;
        }

        // 进入配置模式 [15,1]（非致命）
        bool EnterConfig(Session s)
        {
            if (s.ConfigEntered) return true;
            s.Send(Cmd(0x0F, 0x01));
            byte[] ack = s.WaitFor(delegate (byte[] q) { return q.Length >= 2 && q[0] == 0x10 && q[1] == 0x06; }, 1200);
            s.ConfigEntered = true;
            return ack != null;
        }

        // ---------- 读：当前档 / 固件版本 / 档数据（应答报文首字节 = 0x10） ----------
        public int? ReadCurrent(Session s)
        {
            EnterConfig(s);
            for (int attempt = 0; attempt < 2; attempt++)
            {
                s.Send(Cmd(0x0F, 0x0B));
                byte[] p = s.WaitFor(delegate (byte[] q)
                {
                    return q.Length >= 3 && q[0] == 0x10 && q[1] == 0x0C;   // ReadCurrentProfileAck
                }, 1200);
                if (p != null)
                {
                    int idx = p[2];
                    if (idx == 0) idx = 1;   // 与 App 相同兜底
                    if (idx > 4) idx = 1;
                    return idx;
                }
            }
            return null;
        }

        public string ReadFw(Session s)
        {
            EnterConfig(s);
            for (int attempt = 0; attempt < 2; attempt++)
            {
                s.Send(Cmd(0x0F, 0x09));
                byte[] p = s.WaitFor(delegate (byte[] q)
                {
                    return q.Length >= 10 && q[0] == 0x10 && q[1] == 0x0A;   // ReadFirmwareVersionAck
                }, 1500);
                if (p != null)
                {
                    // 版本串 UTF-16LE，自第 2 字节起
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 2; i + 1 < Math.Min(p.Length, 14); i += 2)
                        {
                            char c = (char)(p[i] | (p[i + 1] << 8));
                            if (c == '\0') break;
                            sb.Append(c);
                        }
                        return Ui.TrimNull(sb.ToString());
                    }
                    catch { return ""; }
                }
            }
            return null;
        }

        // 读整个档：分块 [15,4,档,Hi,Lo,Len]，回包 10-05-档-Hi-Lo-Len-data
        public byte[] ReadProfile(Session s, int prof)
        {
            EnterConfig(s);
            byte[] buf = new byte[PROFILE_LEN];
            int maxEnd = 0;
            for (int off = 0; off < PROFILE_LEN; off += CHUNK)
            {
                int n = Math.Min(CHUNK, PROFILE_LEN - off);
                int hi = off >> 8, lo = off & 0xFF;
                byte[] resp = null;
                Exception last = null;
                for (int attempt = 0; attempt < 3 && resp == null; attempt++)
                {
                    if (attempt > 0) Thread.Sleep(200);
                    s.Send(Cmd(0x0F, 0x04, prof, hi, lo, n));
                    resp = s.WaitFor(delegate (byte[] q)
                    {
                        return q.Length >= 6 && q[0] == 0x10 &&
                               (q[1] == 0x05 || q[1] == 0x11) &&   // ReadProfileAck / RefreshProfileAck
                               q[2] == prof &&
                               q[3] == hi && q[4] == lo && q[5] > 0 && q[5] <= CHUNK &&
                               q.Length >= 6 + q[5];
                    }, 1200);
                    if (resp == null) last = new Exception("读档超时（档 " + prof + " 偏移 " + off + "）");
                }
                if (resp == null) throw last;
                int addr = (resp[3] << 8) | resp[4];
                int len = resp[5];
                if (addr < 0 || addr + len > PROFILE_LEN) throw new Exception("读档回包地址异常");
                Array.Copy(resp, 6, buf, addr, len);
                if (addr + len > maxEnd) maxEnd = addr + len;
                s.Send(Cmd(0x0F, 0xF2, 0x00));   // 保活心跳
            }
            if (maxEnd == 0) return null;
            byte[] outb = new byte[maxEnd];
            Array.Copy(buf, outb, maxEnd);
            return outb;
        }

        // ---------- 写：只写变化的块 ----------
        public bool WriteBytes(Session s, int prof, int offset, byte[] data)
        {
            if (data == null || data.Length == 0) return false;
            EnterConfig(s);
            int pos = 0;
            while (pos < data.Length)
            {
                int n = Math.Min(CHUNK, data.Length - pos);
                int off = offset + pos;
                int hi = off >> 8, lo = off & 0xFF;
                byte[] cmd = new byte[6 + n];
                cmd[0] = 0x0F; cmd[1] = 0x03; cmd[2] = (byte)prof;
                cmd[3] = (byte)hi; cmd[4] = (byte)lo; cmd[5] = (byte)n;
                Array.Copy(data, pos, cmd, 6, n);
                s.Send(cmd);
                s.WaitFor(delegate (byte[] q) { return q.Length >= 2 && q[0] == 0x10 && (q[1] == 0x06 || q[1] == 0x03); }, 1500);
                s.Send(Cmd(0x0F, 0xF2, 0x00));
                pos += n;
            }
            Thread.Sleep(60);
            return true;
        }

        public bool Switch(Session s, int prof)
        {
            EnterConfig(s);
            s.Send(Cmd(0x0F, 0x07, prof));
            s.WaitFor(delegate (byte[] q) { return q.Length >= 2 && q[0] == 0x10 && (q[1] == 0x06 || q[1] == 0x07); }, 1500);
            Thread.Sleep(150);
            return true;
        }

        // ---------- 语义换算 / 档解析 ----------
        public static int DecodeAnti(byte b)
        {
            int v = (b == 0) ? 0 : (b - 11);
            if (v < -10) v = -10;
            if (v > 10) v = 10;
            return v;
        }

        public static byte EncodeAnti(int v)
        {
            int b = v + 11;
            if (b < 1) b = 1;
            if (b > 21) b = 21;
            return (byte)b;
        }

        public static string ParseName(byte[] raw)
        {
            if (raw == null || raw.Length < 2) return "";
            int n = Math.Min(32, raw.Length);
            try { return Ui.TrimNull(Encoding.UTF8.GetString(raw, 0, n)); }
            catch { return ""; }
        }
    }

    // ---------------------------- 主窗体 ----------------------------
    class MainForm : Form
    {
        const int W = 940;
        const int H = 466;

        class ProfInfo
        {
            public string Name = "";
            public int? L = null;
            public int? R = null;
            public bool Loaded = false;
        }

        GameSirApp app = new GameSirApp();
        HidDirect hid = new HidDirect();
        object hidSync = new object();

        volatile bool shutdown;
        Thread worker;
        int actionBusy;

        bool connected;
        string fwText = "";
        int currentIdx = -1;
        ProfInfo[] prof = new ProfInfo[5];
        int activeTab = 1;
        DateTime touchL = DateTime.MinValue, touchR = DateTime.MinValue;

        Pill pDev, pFw, pPatch, pApp, langPill;
        RoundButton btnPatch, btnUnpatch, btnLaunch, btnPickDir;
        TabButton[] tabs = new TabButton[4];
        SliderCtrl slL, slR;

        RoundButton apL, apR;
        Label[] cardTitles = new Label[2];
        Label[] jitLabels = new Label[2];
        Label[] smoothLabels = new Label[2];
        Panel toast;
        Label toastLabel;
        System.Windows.Forms.Timer toastTimer;

        public MainForm()
        {
            for (int i = 0; i < 5; i++) prof[i] = new ProfInfo();
            try
            {
                string lp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lang.txt");
                if (File.Exists(lp)) L.En = File.ReadAllText(lp, Encoding.UTF8).Trim().ToLower() == "en";
            }
            catch { }
            Text = "RC-Tool";
            try { Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            BackColor = C.Bg;
            ForeColor = C.Text;
            ClientSize = new Size(W, H);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            Font = NewFont(9F, FontStyle.Regular);
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            BuildUi();
            ApplyLanguage();
            FormClosing += delegate
            {
                shutdown = true;
                if (toastTimer != null) toastTimer.Stop();
                KillLive();
            };

            AppendLog("RC-Tool v3.50（直连版）已启动。");
            try { Trace.W("启动环境: app=" + (app.Dir ?? "null") + " shared=" + Directory.Exists(Trace.SharedDir)); } catch { }
            AppendLog("直连模式：不依赖 GameSir Connect 读取配置；App 路径仅用于打补丁/启动。");
            app.PatchStatusChanged = delegate
            {
                try { BeginInvoke(new Action(delegate { PaintAppPills(); })); } catch { }
            };
            app.PatchProgress = delegate (string s)
            {
                try { BeginInvoke(new Action(delegate { AppendLog("  " + s); })); } catch { }
            };
            if (app.Dir != null) app.RequestPatchScan();
            if (app.Dir != null) AppendLog("GameSir Connect 路径: " + app.Dir);
            else
            {
                AppendLog("未定位到 GameSir Connect（打补丁前请手动选择安装路径）。");
                ThreadPool.QueueUserWorkItem(delegate
                {
                    string d = null;
                    try { d = app.Locate(false); } catch { }
                    try
                    {
                        BeginInvoke(new Action(delegate
                        {
                            if (d != null) { AppendLog("已定位 App: " + d); app.RequestPatchScan(); }
                            else AppendLog("未找到 App，可点\"选择安装路径…\"手动指定。");
                            PaintAppPills();
                        }));
                    }
                    catch { }
                });
            }

            worker = new Thread(WorkerLoop);
            worker.IsBackground = true;
            worker.Start();
        }

        static FontFamily cnFamily, enFamily;
        static bool cnReady, enReady;

        static FontFamily FindFamily(string[] candidates)
        {
            try
            {
                InstalledFontCollection col = new InstalledFontCollection();
                foreach (string c in candidates)
                    foreach (FontFamily f in col.Families)
                        if (f.Name.IndexOf(c, StringComparison.OrdinalIgnoreCase) >= 0)
                            return f;
            }
            catch { }
            return null;
        }

        // 中文：微软雅黑 UI
        Font NewFont(float size, FontStyle style)
        {
            if (!cnReady)
            {
                cnFamily = FindFamily(new string[] { "Microsoft YaHei UI", "Microsoft YaHei", "DengXian", "SimHei" });
                cnReady = true;
            }
            if (cnFamily != null) return new Font(cnFamily, size, style);
            return new Font("Microsoft YaHei UI", size, style);
        }

        // 英文/数字：Bahnschrift
        Font NewFontEn(float size, FontStyle style)
        {
            if (!enReady)
            {
                enFamily = FindFamily(new string[] { "Bahnschrift" });
                enReady = true;
            }
            if (enFamily != null) return new Font(enFamily, size, style);
            return new Font("Segoe UI", size, style);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Rectangle r = ClientRectangle;
            using (LinearGradientBrush br = new LinearGradientBrush(r, C.BgTop, C.BgBottom, LinearGradientMode.Vertical))
                e.Graphics.FillRectangle(br, r);
        }

        // ---------- UI ----------
        void BuildUi()
        {
            LogoBox logo = new LogoBox();
            logo.Img = LogoBake.Load();
            logo.SetBounds(20, 12, 302, 56);
            Controls.Add(logo);
            langPill = new Pill();
            langPill.Font = NewFont(8.5F, FontStyle.Regular);
            langPill.WhiteText = true;
            langPill.SetBounds(330, 22, 64, 36);   // 与右侧四个指示器同高、间距 6 贴靠
            langPill.Cursor = Cursors.Hand;
            langPill.Click += delegate { ToggleLang(); };
            Controls.Add(langPill);

            pDev = MakePill(400, 22, 148, 36);
            pFw = MakePill(554, 22, 104, 36);
            pApp = MakePill(664, 22, 120, 36);
            pPatch = MakePill(790, 22, 126, 36);
            pDev.SetState("手柄: 探测中", "");
            pFw.SetState("固件: -", "");
            pApp.SetState("App: 探测中", "");
            pPatch.SetState("补丁: ?", "");

            btnPatch = MakeButton(24, 84, 214, 34, "去除RC强制归零", false, delegate { ActPatch(); });
            btnUnpatch = MakeButton(250, 84, 214, 34, "恢复官方原版", false, delegate { ActUnpatch(); });
            btnLaunch = MakeButton(476, 84, 214, 34, "重启 GameSir Connect", false, delegate { ActLaunch(); });
            btnPickDir = MakeButton(702, 84, 214, 34, "选择安装路径…", false, delegate { ActPickDir(); });

            for (int i = 0; i < 4; i++)
            {
                int k = i;
                TabButton tb = new TabButton();
                tb.Font = NewFont(9F, FontStyle.Regular);
                tb.Text = "配置档 " + (i + 1);
                tb.SetBounds(24 + i * 226, 142, 214, 38);
                tb.Click += delegate { SetTab(k + 1); };
                tabs[i] = tb;
                Controls.Add(tb);
            }

            int cw = (W - 72) / 2;
            for (int i = 0; i < 2; i++)
            {
                string side = i == 0 ? "L" : "R";
                int x = 24 + i * (cw + 24);
                CardPanel card = new CardPanel();
                card.SetBounds(x, 204, cw, 238);
                card.BackColor = C.Card;
                Controls.Add(card);

                Label title = MakeLabel(side == "L" ? "左摇杆 · RC 滤波" : "右摇杆 · RC 滤波",
                    24, 16, cw - 48, 32, NewFont(14.5F, FontStyle.Bold), C.Text);
                title.TextAlign = ContentAlignment.MiddleCenter;
                cardTitles[i] = title;
                card.Controls.Add(title);

                // 左右两侧标签
                Label lbJit = MakeLabel("增抖", 4, 76, 68, 24, NewFont(10F, FontStyle.Regular), C.Jit);
                lbJit.TextAlign = ContentAlignment.MiddleCenter;
                Label lbJitNum = MakeLabel("-10", 4, 104, 68, 22, NewFontEn(8.5F, FontStyle.Regular), C.Jit);
                lbJitNum.TextAlign = ContentAlignment.MiddleCenter;
                Label lbSmooth = MakeLabel("防抖", cw - 72, 76, 68, 24, NewFont(10F, FontStyle.Regular), C.SmoothColor);
                lbSmooth.TextAlign = ContentAlignment.MiddleCenter;
                Label lbSmoothNum = MakeLabel("10", cw - 72, 104, 68, 22, NewFontEn(8.5F, FontStyle.Regular), C.SmoothColor);
                lbSmoothNum.TextAlign = ContentAlignment.MiddleCenter;
                jitLabels[i] = lbJit;
                smoothLabels[i] = lbSmooth;
                card.Controls.Add(lbJit);
                card.Controls.Add(lbJitNum);
                card.Controls.Add(lbSmooth);
                card.Controls.Add(lbSmoothNum);

                SliderCtrl sl = new SliderCtrl();
                sl.Font = NewFontEn(12F, FontStyle.Bold);
                sl.SetBounds(74, 74, cw - 148, 62);
                SliderCtrl cap = sl;
                string capSide = side;
                sl.ValueChanged += delegate
                {
                    if (capSide == "L") touchL = DateTime.Now.AddSeconds(3.5);
                    else touchR = DateTime.Now.AddSeconds(3.5);
                    PaintValue(capSide);
                };
                card.Controls.Add(sl);
                if (i == 0) slL = sl; else slR = sl;

                RoundButton ap = new RoundButton();
                ap.Primary = true;
                ap.Font = NewFont(10.5F, FontStyle.Regular);
                ap.Text = side == "L" ? "应用到左摇杆" : "应用到右摇杆";
                ap.SetBounds(22, 162, cw - 44, 40);
                string aps = side;
                ap.Click += delegate { ApplySide(aps); };
                card.Controls.Add(ap);
                if (i == 0) apL = ap; else apR = ap;
            }

            toast = new Panel();
            toast.BackColor = ColorTranslator.FromHtml("#3a2018");
            toast.Visible = false;
            toastLabel = new Label();
            toastLabel.Dock = DockStyle.Fill;
            toastLabel.TextAlign = ContentAlignment.MiddleCenter;
            toastLabel.ForeColor = ColorTranslator.FromHtml("#ffe9e2");
            toastLabel.Font = NewFont(9F, FontStyle.Regular);
            toast.Controls.Add(toastLabel);
            Controls.Add(toast);
            toast.BringToFront();
            toastTimer = new System.Windows.Forms.Timer();
            toastTimer.Interval = 2000;
            toastTimer.Tick += delegate { toast.Visible = false; toastTimer.Stop(); };

            PaintAppPills();
        }

        Label MakeLabel(string text, int x, int y, int w, int h, Font f, Color fore)
        {
            Label lb = new Label();
            lb.Text = text;
            lb.SetBounds(x, y, w, h);
            lb.Font = f;
            lb.ForeColor = fore;
            lb.BackColor = Color.Transparent;
            lb.AutoEllipsis = true;
            return lb;
        }

        Pill MakePill(int x, int y, int w, int h)
        {
            Pill p = new Pill();
            p.Font = NewFont(8.5F, FontStyle.Regular);
            p.SetBounds(x, y, w, h);
            Controls.Add(p);
            return p;
        }

        RoundButton MakeButton(int x, int y, int w, int h, string text, bool primary, EventHandler onClick)
        {
            RoundButton b = new RoundButton();
            b.Primary = primary;
            b.Font = NewFont(9F, FontStyle.Regular);
            b.Text = text;
            b.SetBounds(x, y, w, h);
            b.Click += onClick;
            Controls.Add(b);
            return b;
        }

        // ---------- 语言切换 ----------
        void ToggleLang()
        {
            L.En = !L.En;
            try
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lang.txt"),
                    L.En ? "en" : "zh", Encoding.UTF8);
            }
            catch { }
            ApplyLanguage();
        }

        void ApplyLanguage()
        {
            Text = "RC-Tool";
            btnPatch.Text = L.T("去除RC强制归零", "Remove RC Force-Reset");
            btnUnpatch.Text = L.T("恢复官方原版", "Restore Official App");
            btnLaunch.Text = L.T("重启 GameSir Connect", "Restart GameSir Connect");
            btnPickDir.Text = L.T("选择安装路径…", "Select Install Folder...");
            for (int i = 0; i < 2; i++)
            {
                if (cardTitles[i] != null)
                    cardTitles[i].Text = i == 0
                        ? L.T("左摇杆 · RC 滤波", "Left Stick · RC Filter")
                        : L.T("右摇杆 · RC 滤波", "Right Stick · RC Filter");
                if (jitLabels[i] != null) jitLabels[i].Text = L.T("增抖", "Jitter");
                if (smoothLabels[i] != null) smoothLabels[i].Text = L.T("防抖", "Smooth");
            }
            if (apL != null) apL.Text = L.T("应用到左摇杆", "Apply to Left Stick");
            if (apR != null) apR.Text = L.T("应用到右摇杆", "Apply to Right Stick");
            if (langPill != null) langPill.SetState(L.En ? "中文" : "EN", "");
            PaintAll();
        }

        // ---------- 日志 / Toast ----------
        void AppendLog(string s)
        {
            Trace.W(s);
        }

        void ShowToast(string msg)
        {
            toastLabel.Text = msg;
            int tw = 30 + msg.Length * 14;
            if (tw > W - 60) tw = W - 60;
            toast.SetBounds(W / 2 - tw / 2, H - 56, tw, 34);
            Ui.ApplyRoundRegion(toast, 12);
            toast.Visible = true;
            toast.BringToFront();
            toastTimer.Stop();
            toastTimer.Start();
        }

        // ---------- 状态轮询（直连） ----------
        void WorkerLoop()
        {
            while (!shutdown)
            {
                Thread.Sleep(3000);
                if (shutdown) break;
                PollTick();
            }
        }

        int pollGate;

        class Snapshot
        {
            public bool Connected;
            public string Fw;
            public int Current;
            public string Err;
            public int Tab;
            public ProfInfo Info;
            public string Dev;
        }

        void ApplySnapshot(Snapshot snap)
        {
            bool wasConnected = connected;
            connected = snap.Connected;
            if (snap.Fw != null) fwText = snap.Fw;
            bool curChanged = snap.Current >= 0 && snap.Current != currentIdx;
            if (snap.Current >= 0) currentIdx = snap.Current;
            if (snap.Info != null)
            {
                prof[snap.Tab] = snap.Info;
            }
            bool justDisconnected = wasConnected && !connected;
            if (wasConnected != connected)
                AppendLog(connected ? "手柄已连接（直连）" : "手柄未连接：" + (snap.Err ?? ""));
            // 断联：档名/数值/固件显示归位默认
            if (justDisconnected) ResetToDefault();
            // 连接/换档时自动读取
            if (connected && currentIdx >= 1 && (wasConnected != connected || curChanged))
            {
                if (activeTab != currentIdx)
                {
                    activeTab = currentIdx;
                    AppendLog("自动切换到当前配置档 " + currentIdx);
                }
                AutoReadAll();
            }
            PaintAll();
        }

        // 手柄断联：所有档位数据与数值归位默认
        void ResetToDefault()
        {
            for (int i = 0; i < 5; i++) prof[i] = new ProfInfo();
            currentIdx = -1;
            fwText = "";
            try
            {
                slL.SetValueSilent(0);
                slL.DisplayValue = null;
                slL.Invalidate();
                slR.SetValueSilent(0);
                slR.DisplayValue = null;
                slR.Invalidate();
            }
            catch { }
        }

        void PaintAll()
        {
            PaintAppPills();
            pDev.SetState(connected
                    ? (L.T("手柄: ", "Pad: ") + "G7P8K")
                    : L.T("手柄: 未连接", "Pad: Not connected"),
                connected ? "ok" : "warn");
            pFw.SetState(L.T("固件: ", "FW: ") + (fwText.Length > 0 ? fwText : "-"), fwText.Length > 0 ? "ok" : "");
            PaintTabs();
            PaintValues();
        }

        void PaintAppPills()
        {
            pApp.SetState(L.T("App: ", "App: ") + (app.Dir != null ? L.T("已定位", "Found") : L.T("未选择", "Not set")),
                app.Dir != null ? "ok" : "warn");
            string patch = "unknown";
            try { patch = app.PatchStatusCached(); } catch { }
            string t = L.T("补丁: ?", "Patch: ?"), cls = "warn";
            if (patch == "clean") { t = L.T("补丁: 官方原版", "Patch: Official"); cls = ""; }
            else if (patch == "patched") { t = L.T("补丁: 已去除归零", "Patch: RC-unlock"); cls = "ok"; }
            else if (patch == "missing") t = L.T("补丁: 未选择路径", "Patch: No path");
            else if (patch == "scanning") t = L.T("补丁: 扫描中…", "Patch: Scanning...");
            pPatch.SetState(t, cls);
            btnPatch.Enabled = app.Dir != null;
            btnUnpatch.Enabled = app.Dir != null;
            btnLaunch.Enabled = app.Dir != null && File.Exists(app.ExePath ?? "");
        }

        void PaintTabs()
        {
            for (int i = 0; i < 4; i++)
            {
                int idx = i + 1;
                string name = (prof[idx] != null && prof[idx].Name.Length > 0)
                    ? prof[idx].Name : (L.T("配置档 ", "Profile ") + idx);
                tabs[i].Text = name;
                tabs[i].Active = (idx == activeTab);
                tabs[i].Current = (idx == currentIdx);
                tabs[i].Invalidate();
            }
        }

        void PaintValues()
        {
            ProfInfo p = prof[activeTab];
            DateTime now = DateTime.Now;
            if (now > touchL)
            {
                slL.SetValueSilent(p != null && p.L.HasValue ? p.L.Value : 0);
                slL.DisplayValue = p != null && p.L.HasValue ? p.L : (int?)null;
                slL.Invalidate();
            }
            if (now > touchR)
            {
                slR.SetValueSilent(p != null && p.R.HasValue ? p.R.Value : 0);
                slR.DisplayValue = p != null && p.R.HasValue ? p.R : (int?)null;
                slR.Invalidate();
            }
        }

        void PaintValue(string side)
        {
            SliderCtrl sl = side == "L" ? slL : slR;
            sl.DisplayValue = sl.Value;
            sl.Invalidate();
        }

        void SetTab(int k)
        {
            activeTab = k;
            PaintTabs();
            PaintValues();
            RequestReadTab(k);
            // 点 Tab 即切换手柄当前档
            if (k != currentIdx && connected)
                SwitchToProfile(k);
        }

        void SwitchToProfile(int k)
        {
            RunBg(delegate
            {
                HidDirect.Session s = EnsureSession();
                int cur = -1;
                try
                {
                    hid.Switch(s, k);
                    int? c = hid.ReadCurrent(s);
                    if (c != null) cur = c.Value;
                }
                catch { KillLive(); throw; }
                return cur;
            },
            delegate (object r)
            {
                currentIdx = (int)r;
                Trace.W("tab-switch: target=" + k + " current=" + currentIdx);
                PaintTabs();
                PaintValues();
            });
        }

        // ---------- 后台操作封装 ----------
        void RunBg(Func<object> work, Action<object> done)
        {
            RunBg(work, done, true);
        }

        void RunBg(Func<object> work, Action<object> done, bool useHid)
        {
            Interlocked.Increment(ref actionBusy);
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    object r;
                    if (useHid)
                    {
                        // 等锁最多 8 秒
                        if (!Monitor.TryEnter(hidSync, 8000))
                            throw new Exception("手柄通道忙（上一次操作还没结束），请稍后重试");
                        try { r = work(); }
                        finally { Monitor.Exit(hidSync); }
                    }
                    else r = work();
                    try { BeginInvoke(new Action(delegate { done(r); })); } catch { }
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    try { BeginInvoke(new Action(delegate { AppendLog("[错误] " + msg); ShowToast("失败: " + msg); })); } catch { }
                }
                finally { Interlocked.Decrement(ref actionBusy); }
            });
        }

        // ---------- 直连动作 ----------
        void RequestReadTab(int tab)
        {
            RunBg(delegate
            {
                HidDirect.Session s = EnsureSession();
                byte[] raw;
                try { raw = hid.ReadProfile(s, tab); }
                catch { KillLive(); throw; }
                if (raw == null || raw.Length <= HidDirect.OFF_RS) throw new Exception("读取配置档失败");
                ProfInfo info = new ProfInfo();
                info.Name = HidDirect.ParseName(raw);
                info.L = HidDirect.DecodeAnti(raw[HidDirect.OFF_LS]);
                info.R = HidDirect.DecodeAnti(raw[HidDirect.OFF_RS]);
                info.Loaded = true;
                return new object[] { tab, info };
            },
            delegate (object r)
            {
                object[] arr = (object[])r;
                int pt = (int)arr[0];
                prof[pt] = (ProfInfo)arr[1];
                PaintTabs();
                if (pt == activeTab) PaintValues();
            });
        }

        void ApplySide(string side)
        {
            if (!connected) { ShowToast("手柄未连接"); return; }
            int p = activeTab;
            int v = side == "L" ? slL.Value : slR.Value;
            int off = side == "L" ? HidDirect.OFF_LS : HidDirect.OFF_RS;
            byte val = HidDirect.EncodeAnti(v);
            bool jit;
            string label = Ui.FmtVal(v, out jit);
            RunBg(delegate
            {
                HidDirect.Session s = EnsureSession();
                try
                {
                    hid.WriteBytes(s, p, off, new byte[] { val });
                    byte[] raw = hid.ReadProfile(s, p);
                    if (raw == null || raw.Length <= off) throw new Exception("写入后校验失败（读档为空）");
                    if (raw[off] != val)
                        throw new Exception("写入后校验不一致：期望 0x" + val.ToString("X2") + " 实际 0x" + raw[off].ToString("X2"));
                    return raw;
                }
                catch { KillLive(); throw; }
            },
            delegate (object r)
            {
                byte[] raw = (byte[])r;
                ProfInfo info = new ProfInfo();
                info.Name = HidDirect.ParseName(raw);
                info.L = HidDirect.DecodeAnti(raw[HidDirect.OFF_LS]);
                info.R = HidDirect.DecodeAnti(raw[HidDirect.OFF_RS]);
                info.Loaded = true;
                prof[p] = info;
                AppendLog("配置档" + p + " " + side + " → " + label + "（写入并校验通过）");
                ShowToast(label + " 已写入配置档 " + p);
                PaintAll();
            });
        }

        int autoReadFlag;

        // 自动读取全部档位
        void AutoReadAll()
        {
            if (Interlocked.CompareExchange(ref autoReadFlag, 1, 0) != 0) return;
            RunBg(delegate
            {
                try
                {
                    HidDirect.Session s = EnsureSession();
                    ProfInfo[] outInfo = new ProfInfo[5];
                    for (int p = 1; p <= 4; p++)
                    {
                        byte[] raw = hid.ReadProfile(s, p);
                        if (raw != null && raw.Length > HidDirect.OFF_RS)
                        {
                            ProfInfo info = new ProfInfo();
                            info.Name = HidDirect.ParseName(raw);
                            info.L = HidDirect.DecodeAnti(raw[HidDirect.OFF_LS]);
                            info.R = HidDirect.DecodeAnti(raw[HidDirect.OFF_RS]);
                            info.Loaded = true;
                            outInfo[p] = info;
                        }
                    }
                    return outInfo;
                }
                catch { KillLive(); throw; }
                finally { Interlocked.Exchange(ref autoReadFlag, 0); }
            },
            delegate (object r)
            {
                ProfInfo[] outInfo = (ProfInfo[])r;
                int n = 0;
                for (int p = 1; p <= 4; p++)
                    if (outInfo[p] != null) { prof[p] = outInfo[p]; n++; }
                Trace.W("auto-read: " + n + "/4 profiles");
                PaintTabs();
                PaintValues();
            });
        }

        Snapshot GrabSnapshot(int tab)
        {
            Snapshot snap = new Snapshot();
            snap.Tab = tab;
            snap.Current = -1;
            if (!Monitor.TryEnter(hidSync, 500)) return null;   // 有用户操作在进行，下一轮再轮询
            try
            {
                try
                {
                    HidDirect.Session s = EnsureSession();
                    snap.Connected = true;
                    snap.Dev = liveDesc;
                    int? cur = hid.ReadCurrent(s);
                    if (cur != null) snap.Current = cur.Value;
                    if (string.IsNullOrEmpty(fwText)) snap.Fw = hid.ReadFw(s);
                    // 档数据：每 3 次轮询刷新
                    pollCount++;
                    bool needProfile = prof[tab] == null || !prof[tab].Loaded || pollCount % 3 == 0;
                    if (needProfile)
                    {
                        byte[] raw = hid.ReadProfile(s, tab);
                        if (raw != null && raw.Length > HidDirect.OFF_RS)
                        {
                            ProfInfo info = new ProfInfo();
                            info.Name = HidDirect.ParseName(raw);
                            info.L = HidDirect.DecodeAnti(raw[HidDirect.OFF_LS]);
                            info.R = HidDirect.DecodeAnti(raw[HidDirect.OFF_RS]);
                            info.Loaded = true;
                            snap.Info = info;
                        }
                    }
                }
                catch (Exception e)
                {
                    KillLive();
                    snap.Connected = false;
                    snap.Err = e.Message;
                }
            }
            finally { Monitor.Exit(hidSync); }
            Trace.W("poll: connected=" + snap.Connected + " cur=" + snap.Current +
                    (snap.Info != null ? (" L=" + snap.Info.L + " R=" + snap.Info.R) : " profile=-") +
                    (snap.Err != null ? (" err=" + snap.Err) : ""));
            return snap;
        }

        int pollCount;

        // ---------- 持久会话 ----------
        HidDirect.Session live;
        string liveDesc = "";

        // 调用方须持有 hidSync
        HidDirect.Session EnsureSession()
        {
            if (live != null)
            {
                if (live.Alive) return live;
                KillLive();
            }
            List<HidDirect.Device> devs = hid.EnumDevices();
            HidDirect.Device dev = hid.PickConfig(devs);
            if (dev == null) throw new Exception("未发现手柄");
            live = hid.OpenSession(dev);
            liveDesc = "VID_" + dev.Vid.ToString("X4") + "  PID_" + dev.Pid.ToString("X4") +
                       "  usagePage=0x" + dev.UsagePage.ToString("X4");
            return live;
        }

        void KillLive()
        {
            if (live != null)
            {
                try { live.Dispose(); } catch { }
                live = null;
            }
        }

        // ---------- App 相关动作（路径 / 补丁 / 启动） ----------
        int patchBusy;

        void SetPatchButtons(bool enabled)
        {
            bool ok = enabled && app.Dir != null;
            btnPatch.Enabled = ok;
            btnUnpatch.Enabled = ok;
        }

        void ActPatch()
        {
            if (app.Dir == null) { ShowToast("请先选择安装路径"); return; }
            if (Interlocked.CompareExchange(ref patchBusy, 1, 0) != 0) { ShowToast("补丁操作正在进行，请稍候…"); return; }
            SetPatchButtons(false);
            int n = GameSirApp.GameSirProcessCount();
            if (n > 0)
                AppendLog("提示：检测到 GameSir Connect 正在运行（" + n + " 个进程），若补丁失败请完全退出它（含托盘）再试。");
            AppendLog("执行补丁：去除强制归零…");
            RunBg(delegate
            {
                try { return "OK:" + app.ApplyPatch(); }
                catch (Exception e) { return "ERR:" + e.Message; }
            },
            delegate (object r)
            {
                string s = (string)r;
                Interlocked.Exchange(ref patchBusy, 0);
                SetPatchButtons(true);
                if (s.StartsWith("ERR:"))
                {
                    AppendLog("[补丁失败] " + s.Substring(4));
                    ShowToast("补丁失败: " + s.Substring(4));
                }
                else
                {
                    string detail = s == "OK:already" ? "（此前已打过，无需重复）" : "";
                    AppendLog("补丁完成" + detail + "，重启 GameSir Connect 后生效");
                    ShowToast("补丁完成，重启 App 生效");
                }
                PaintAll();
            }, false);
        }

        void ActUnpatch()
        {
            if (app.Dir == null) { ShowToast("请先选择安装路径"); return; }
            if (Interlocked.CompareExchange(ref patchBusy, 1, 0) != 0) { ShowToast("补丁操作正在进行，请稍候…"); return; }
            SetPatchButtons(false);
            AppendLog("恢复官方原版…");
            RunBg(delegate
            {
                try { return "OK:" + app.RevertPatch(); }
                catch (Exception e) { return "ERR:" + e.Message; }
            },
            delegate (object r)
            {
                string s = (string)r;
                Interlocked.Exchange(ref patchBusy, 0);
                SetPatchButtons(true);
                if (s.StartsWith("ERR:"))
                {
                    AppendLog("[恢复失败] " + s.Substring(4));
                    ShowToast("恢复失败: " + s.Substring(4));
                }
                else
                {
                    string detail = s == "OK:clean" ? "（当前本来就是官方原版）" : "";
                    AppendLog("已恢复官方原版" + detail);
                    ShowToast("已恢复官方原版");
                }
                PaintAll();
            }, false);
        }

        void ActLaunch()
        {
            RunBg(delegate { return app.Restart(); },
                delegate (object r)
                {
                    string s = (string)r;
                    AppendLog(s == "restarted" ? "已重启 GameSir Connect" : "GameSir Connect 未在运行，已启动");
                    ShowToast(s == "restarted" ? "已重启 GameSir Connect" : "已启动 GameSir Connect");
                }, false);
        }

        void ActPickDir()
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "选择 GameSir Connect 安装目录（包含 resources 文件夹）";
                if (app.Dir != null) dlg.SelectedPath = app.Dir;
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                string d = dlg.SelectedPath;
                if (!GameSirApp.Valid(d))
                {
                    // 允许选中游戏目录本身或上层
                    string alt = Path.Combine(d, "GameSir Connect");
                    if (GameSirApp.Valid(alt)) d = alt;
                    else
                    {
                        MessageBox.Show(this, "该目录下没有 resources\\app.asar，请选择 GameSir Connect 的安装目录。",
                            "路径无效", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                app.SetDir(d);
                AppendLog("已设置 App 路径: " + d);
                app.RequestPatchScan();
                PaintAll();
            }
        }

        // 后台轮询
        void PollTick()
        {
            if (shutdown) return;
            if (Interlocked.CompareExchange(ref actionBusy, 0, 0) != 0) return;
            if (Interlocked.CompareExchange(ref pollGate, 1, 0) != 0) return;
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    Snapshot snap = GrabSnapshot(activeTab);
                    if (snap != null)
                    {
                        try { BeginInvoke(new Action(delegate { ApplySnapshot(snap); })); } catch { }
                    }
                }
                catch { }
                finally { Interlocked.Exchange(ref pollGate, 0); }
            });
        }
    }
}
