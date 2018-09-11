using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using F = System.Windows.Forms;
namespace DicDic
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 程序启动
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            MainWindow = new MainWindow();
            MainWindow.Show();
            MainWindow.Hide();
            CreateNotifyIcon();
        }

        /// <summary>
        /// 创建系统托盘图标和相关的菜单
        /// </summary>
        private void CreateNotifyIcon()
        {
            var ni = new F.NotifyIcon()
            {
                Text = "DicDic",
                Visible = true,
                Icon = DicDic.Properties.Resources.NotifyIcon,
                ContextMenu = new F.ContextMenu(new[] {
                    new F.MenuItem("帮助",(s,args)=>{
                        Process.Start("https://github.com/NovaShang/DicDic");
                    }),
                    new F.MenuItem("关于",(s,args)=>{
                        Process.Start("http://novashang.com");
                    }),
                    new F.MenuItem("-"),
                    new F.MenuItem("退出",(s,args)=>{
                        Shutdown();
                    })
                })
            };

            ni.Click += (s, args) =>
            {
                MainWindow.Show();
                MainWindow.Activate();
            };
        }
    }
}
