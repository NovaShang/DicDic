using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using F = System.Windows.Forms;
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
            IntPtr hWnd, int id, int fsModifiers, F.Keys vk);

        /// <summary>
        /// Win32API 用来注销一个全局快捷键
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// 当窗口的Source初始化完成时注册快捷键
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hWnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hWnd).AddHook(WndProc);
            if (RegisterHotKey(hWnd, 0, 5, F.Keys.D))
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
        #endregion

        private string _keyWord = "";
        private bool _showResult = true;
        private Dictionary<string, int> _dicIndex=new Dictionary<string, int>();
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("KeyWord"));
            }
        }

        /// <summary>
        /// 词典的内容
        /// </summary>
        public List<Result> DicContent { get;} = new List<Result>();

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
                var value = item.Substring(pos + 1, item.Length - pos - 1).Replace("\r","");
                if (_dicIndex.ContainsKey(key)) DicContent[_dicIndex[key]].Content += "|" + value;
                else {
                    _dicIndex.Add(key, DicContent.Count);
                    DicContent.Add(new Result() {  Content= value,Title= key});
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

        public class Result
        {
            public string Title { get; set; }

            public string Content { get; set; }
        }
    }
}
