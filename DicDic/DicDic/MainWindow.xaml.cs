using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace DicDic
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            BuildDicData();
            Left = SystemParameters.WorkArea.Right - Width;
            Top = SystemParameters.WorkArea.Bottom - Height;
            Activated += (s, e) => KeyWordTextBox.Focus();
            Deactivated += (s, e) => HideWindow(null, null);
            Closing += (s, e) => e.Cancel = true;
            DataContext = this;
        }

        #region 用来注册和注销全局快捷键

        /// <summary>
        /// Win32API 用来注册一个全局快捷键
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(
            IntPtr hWnd, int id, int fsModifiers, int vk);

        /// <summary>
        /// Win32API 用来注销一个全局快捷键
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// 当窗口的Source初始化完成时注册快捷键
        /// 快捷键是Shift+Alt+D
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            var setting = Properties.Settings.Default;
            base.OnSourceInitialized(e);
            var hWnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hWnd).AddHook(WndProc);
            if (RegisterHotKey(hWnd, 0,setting.KeyModifier , setting.KeyCode))
                Application.Current.Exit += (s, args) => UnregisterHotKey(hWnd, 0);
            else
                Debug.WriteLine("注册按键失败");
        }

        /// <summary>
        /// 用来响应快捷键按下事件的方法
        /// </summary>
        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wideParam, IntPtr longParam, ref bool handled)
        {
            if (msg == 0x312 && wideParam.ToInt32() == 0)
            {
                Show();
                Activate();
                handled = true;
            }
            return IntPtr.Zero;
        }

        #endregion 用来注册和注销全局快捷键

        private string _keyWord = "";
        private bool _showResult = true;
        private Dictionary<string, int> _dicIndex = new Dictionary<string, int>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 是否时显示结果的状态，
        /// </summary>
        public bool ShowResult
        {
            get => _showResult; set
            {
                if (value == _showResult) return;
                _showResult = value;
                Height = ShowResult ? 500 : 50;
                Left = SystemParameters.WorkArea.Right - Width;
                Top = SystemParameters.WorkArea.Bottom - Height;
            }
        }

        /// <summary>
        /// 当前的搜索关键词
        /// </summary>
        public string KeyWord
        {
            get => _keyWord; set
            {
                _keyWord = value;
                ShowResult = value != "";
                if (_dicIndex.TryGetValue(KeyWord, out var index))
                    ResultList.SelectedItem = DicContent[index];
                else
                {
                    var key = _dicIndex.Keys.FirstOrDefault(x => x.StartsWith(KeyWord));
                    if (key != null) ResultList.SelectedItem = DicContent[_dicIndex[key]];
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("KeyWord"));
            }
        }

        /// <summary>
        /// 词典的内容
        /// </summary>
        public List<Result> DicContent { get; } = new List<Result>();

        /// <summary>
        /// 根据资源中的数据构建内存中的字典
        /// </summary>
        /// <returns></returns>
        public void BuildDicData()
        {
            foreach (var item in Properties.Resources.en_zh.Split('\n')
                .Concat(Properties.Resources.zh_en.Split('\n')))
            {
                var pos = item.IndexOf(',');
                if (pos < 0) continue;
                var key = item.Substring(0, pos);
                var value = item.Substring(pos + 1, item.Length - pos - 1).Replace("\r", "");
                if (_dicIndex.ContainsKey(key)) DicContent[_dicIndex[key]].Content += "|" + value;
                else
                {
                    _dicIndex.Add(key, DicContent.Count);
                    DicContent.Add(new Result() { Content = value, Title = key });
                }
            }
        }

        /// <summary>
        /// 在线搜索
        /// </summary>
        public void SearchWeb(object sender, EventArgs e)
        {
            Process.Start(Properties.Settings.Default.SearchUrl.Replace("{KEYWORD}", KeyWord));
        }

        /// <summary>
        /// 隐藏界面
        /// </summary>
        public void HideWindow(object sender, EventArgs e)
        {
            Hide();
            KeyWord = "";
        }

        /// <summary>
        /// 在线查词
        /// </summary>
        private void OpenDetail(object sender, EventArgs e)
        {
            Process.Start(Properties.Settings.Default.DictUrl.Replace("{KEYWORD}", KeyWord));
        }

        /// <summary>
        /// 上一个词
        /// </summary>
        private void PrevItem(object sender, EventArgs e)
        {
            if (ResultList.SelectedIndex > 0 && ResultList.SelectedIndex < DicContent.Count)
                ResultList.SelectedIndex--;
            AutoFill(null, null);
        }

        /// <summary>
        /// 下一个词
        /// </summary>
        private void NextItem(object sender, EventArgs e)
        {
            if (ResultList.SelectedIndex >= 0 && ResultList.SelectedIndex < DicContent.Count - 1)
                ResultList.SelectedIndex++;
            AutoFill(null, null);
        }

        /// <summary>
        /// 自动补全
        /// </summary>
        private void AutoFill(object sender, EventArgs e)
        {
            if (ResultList.SelectedItem is Result r)
                KeyWord = r.Title;
        }

        /// <summary>
        /// 结果列表选择变化
        /// </summary>
        private void ResultList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResultList.SelectedItem is Result r)
                ResultList.ScrollIntoView(r);
        }

        /// <summary>
        /// 用来显示结果的VM
        /// </summary>
        public class Result
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// 正文
            /// </summary>
            public string Content { get; set; }
        }
    }
}