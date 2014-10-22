using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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
using System.Windows.Shell;

namespace Domopomodoro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            cp.ActivityFgBrush = new SolidColorBrush(Color.FromRgb(106, 18, 38));
            cp.ActivityBgBrush = new SolidColorBrush(Color.FromRgb(195, 58, 58));
            cp.ShortBreakBgBrush = new SolidColorBrush(Color.FromRgb(79, 185, 159));
            cp.ShortBreakFgBrush = new SolidColorBrush(Color.FromRgb(6, 133, 135));
            cp.LongBreakBgBrush = new SolidColorBrush(Color.FromRgb(121, 189, 168));
            cp.LongBreakFgBrush = new SolidColorBrush(Color.FromRgb(59, 134, 134));
            cp.HoleBrush = this.Background;

            cp.TimeChanged += cp_TimeChanged;
            cp.ModeChanged += cp_ModeChanged;
        }

        void cp_ModeChanged(object sender, PomodoroTimer.ModeChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.NewMode == PomodoroTimer.PomodoroMode.Activity)
                    this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Error;
                else
                    this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            });
        }

        void cp_TimeChanged(object sender, PomodoroTimer.TimeChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                this.TaskbarItemInfo.ProgressValue = (e.PercentComplete / 100.0);
            });
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.TaskbarItemInfo = new TaskbarItemInfo();
            this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Error;
        }
    }
}
