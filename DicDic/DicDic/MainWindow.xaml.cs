using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DicDic
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
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
