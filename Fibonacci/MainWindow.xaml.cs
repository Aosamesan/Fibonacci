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
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;
using Microsoft.Win32;
using System.Windows.Media.Animation;

namespace Fibonacci
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        private User selectedUser;
        public User SelectedUser
        {
            get
            {
                return selectedUser;
            }
            set
            {
                selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            //SelectedUser = new User("김개똥", "30294102", 4.3);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 1;
        }

        private void ContentsButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 0;
        }

        private async void GradeAddButton_Click(object sender, RoutedEventArgs e)
        {
            string item = await DialogManager.ShowInputAsync(this, "New Grade", "새로운 등급을 입력합니다.");

            if (!string.IsNullOrWhiteSpace(item))
            {
                SelectedUser.GradeSystem.Add(item, 3.5);
            }
        }

        private void GradeDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = GradeSystemListView.SelectedItem as GradeTuple;

            if(item != null)
                SelectedUser.GradeSystem.Remove(item.GradeName);
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void InfomationButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedUser = new User("박말똥", "395712", 4.5);
        }

        private async void NewButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = DialogManager.ShowInputAsync(this, "이름", "");
            string name = await dialog;
            if (string.IsNullOrWhiteSpace(name))
                return;
            dialog = DialogManager.ShowInputAsync(this, "학번", "");
            string studentNumber = await dialog;
            if (string.IsNullOrWhiteSpace(name))
                return;
            MetroDialogSettings settings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "4.5",
                NegativeButtonText = "4.3",
                ColorScheme = MetroDialogColorScheme.Inverted
               
            };
            var result = await DialogManager.ShowMessageAsync(this, "학점", "", MessageDialogStyle.AffirmativeAndNegative, settings);
            double maximum = 0;
            if (result == MessageDialogResult.Affirmative)
                maximum = 4.5;
            else
                maximum = 4.3;
            SelectedUser = new User(name, studentNumber, maximum);
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();
        }

        private async void SemesterAddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = await DialogManager.ShowInputAsync(this, "새로운 학기 입력", "");
            SelectedUser.Semesters.Add(name);
        }

        private void SemesterDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Semester item = SemesterListBox.SelectedItem as Semester;

            if(item != null)
            {
                SelectedUser.Semesters.Remove(item);
            }
        }

        private async void LectureAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (SemesterListBox.SelectedItem == null)
                return;

            string lectureName = await DialogManager.ShowInputAsync(this, "강의 제목", "");
            if (string.IsNullOrWhiteSpace(lectureName))
                return;
            string credit = await DialogManager.ShowInputAsync(this, "이수 학점", "정수");
            if (string.IsNullOrWhiteSpace(credit))
                return;
            int intCredit = 0;
            try
            {
                intCredit = Convert.ToInt32(credit);
            }
            catch (Exception ex)
            {
                await DialogManager.ShowMessageAsync(this, "에러", $"Message : {ex.Message}\nStackTrace : {ex.StackTrace}");
                return;
            }
            string grade = await DialogManager.ShowInputAsync(this, "학점", "(A+/A0/A-...., Setting 참조)");
            try
            {
                Lecture l = new Lecture(lectureName, intCredit, grade);
                (SemesterListBox.SelectedItem as Semester).Add(l);
            }
            catch(Exception ex)
            {
                await DialogManager.ShowMessageAsync(this, "에러", $"Message : {ex.Message}\nStackTrace : {ex.StackTrace}");
                return;
            }
        }
    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Hidden;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class VisibilityNotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                Visibility v = (Visibility)value;
                if (v == Visibility.Visible)
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
            }
            catch(Exception e)
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }

    public class ObjectToSemesterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as Semester;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
