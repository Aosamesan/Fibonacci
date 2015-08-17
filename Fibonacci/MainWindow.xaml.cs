using System;
using System.Collections.Generic;
using System.Globalization;
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
using MahApps.Metro.Controls;

namespace Fibonacci
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        GradeSystem selectedSystem;

        public MainWindow()
        {
            InitializeComponent();
            selectedSystem = Resources["SelectedGradeSystemKey"] as GradeSystem;
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 1;
        }

        private void ContentsButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 0;
        }

        private void GradeAddButton_Click(object sender, RoutedEventArgs e)
        {
            selectedSystem.Add("Hello", 4.5);
            selectedSystem.Add("33", 3.2);
            selectedSystem.Add("Apple", 3.1);
        }

        private void GradeDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = GradeSystemListView.SelectedItem as GradeTuple;

            if(item != null)
                selectedSystem.Remove(item.GradeName);
        }
    }
}
