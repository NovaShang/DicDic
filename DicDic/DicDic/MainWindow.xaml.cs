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
        #region 用来注册和注销全局快捷键

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(
            IntPtr hWnd, int id, int fsModifiers, F.Keys vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd, int id);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hWnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hWnd).AddHook(WndProc);
            if (RegisterHotKey(hWnd, 0, 3, F.Keys.D))
                Application.Current.Exit += (s, args) => UnregisterHotKey(hWnd, 0);
            else
                Debug.WriteLine("注册按键失败");
        }

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

        private bool _showResult = false;
        private string _keyWord = "";


        public MainWindow()
        {
            InitializeComponent();
            ResetWindowBound();
            DataContext = this;
        }

        public bool ShowResult
        {
            get => _showResult; set
            {
                if (value == _showResult) return;
                _showResult = value;
                ResetWindowBound();
            }
        }

        public string KeyWord
        {
            get => _keyWord; set
            {
                _keyWord = value;
                ShowResult = value != "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("KeyWord"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void ResetWindowBound()
        {
            Height = ShowResult ? 500 : 50;
            Left = SystemParameters.WorkArea.Right - Width;
            Top = SystemParameters.WorkArea.Bottom - Height;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Hide();
            KeyWord = "";
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            KeyWordTextBox.Focus();
        }
    }
}
