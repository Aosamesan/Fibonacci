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
using System.Xml;
using System.IO;

namespace Fibonacci
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        public static string FilterString { get; }

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
        }

        static MainWindow()
        {
            FilterString = "성적 파일(*.I_GOT_F)|*.I_GOT_F";
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

        private async void GradeDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = GradeSystemListView.SelectedItem as GradeTuple;

            if (item?.IsSealed == false)
            {
                SelectedUser.GradeSystem.Remove(item.GradeName);
            }
            else if(item != null)
            {
                await DialogManager.ShowMessageAsync(this, "에러", "기본으로 등록된 등급은 삭제할 수 없습니다.");
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void InfomationButton_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
            doc.AppendChild(SelectedUser.GetXMLNode(doc));
            MemoryStream ms = new MemoryStream();
            doc.Save(ms);
            MemoryStream rs = new MemoryStream(ms.ToArray());
            StreamReader sr = new StreamReader(rs);
            string s = sr.ReadToEnd();
            await DialogManager.ShowMessageAsync(this, "Xml", s);
            sr.Close();
            rs.Close();
            ms.Close();
        }

        private async void NewButton_Click(object sender, RoutedEventArgs e)
        {
            MetroDialogSettings s = new MetroDialogSettings()
            {
                ColorScheme = MetroDialogColorScheme.Accented
            };
            var dialog = DialogManager.ShowInputAsync(this, "이름", "",s);
            string name = await dialog;
            if (string.IsNullOrWhiteSpace(name))
                return;
            dialog = DialogManager.ShowInputAsync(this, "학번", "",s);
            string studentNumber = await dialog;
            if (string.IsNullOrWhiteSpace(name))
                return;
            MetroDialogSettings settings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "4.5",
                NegativeButtonText = "4.3",
                ColorScheme = MetroDialogColorScheme.Accented

            };
            var result = await DialogManager.ShowMessageAsync(this, "학점", "", MessageDialogStyle.AffirmativeAndNegative, settings);
            double maximum = 0;
            if (result == MessageDialogResult.Affirmative)
                maximum = 4.5;
            else
                maximum = 4.3;
            SelectedUser = new User(name, studentNumber, maximum);
        }

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = FilterString;
            var result = dlg.ShowDialog();

            if (result == true)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(dlg.FileName);
                try
                {
                    SelectedUser = User.GetUserFromXMLNode(doc.DocumentElement);
                }
                catch (Exception ex)
                {
                    await DialogManager.ShowMessageAsync(this, "Error", $"= Message\n {ex.Message}\n\n= StackTrace\n {ex.StackTrace}");
                    return;
                }
                if(SelectedUser == null)
                {
                    await DialogManager.ShowMessageAsync(this, "올바르지 않은 파일입니다.", "파일 형식이 올바르지 않거나 손상된 파일입니다.");
                }
            }
        }

        private async void SemesterAddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = await DialogManager.ShowInputAsync(this, "새로운 학기 입력", "");
            SelectedUser.Semesters.Add(name);
        }

        private void SemesterDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Semester item = SemesterListBox.SelectedItem as Semester;

            if (item != null)
            {
                SelectedUser.Semesters.Remove(item);
                SelectedUser.Semesters.InvokePropertyChanged();
                SelectedUser.RefreshGradeString();
            }
        }

        private async void LectureAddButton_Click(object sender, RoutedEventArgs e)
        {
            Semester selectedSemester = SemesterListBox.SelectedItem as Semester;

            if (selectedSemester == null)
                return;

            try
            {
                
                string lectureName = await DialogManager.ShowInputAsync(this, "강의 제목", "");
                if (string.IsNullOrWhiteSpace(lectureName))
                    return;
                Lecture l = new Lecture(lectureName, 3, "A+");
                
                selectedSemester.Add(l);
                selectedSemester.InvokePropertyChanged();
                SelectedUser.Semesters.InvokePropertyChanged();
                SelectedUser.RefreshGradeString();
            }
            catch (Exception ex)
            {
                await DialogManager.ShowMessageAsync(this, "에러", $"Message : {ex.Message}\nStackTrace : {ex.StackTrace}");
                return;
            }
        }

        private void NumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            Semester selectedSemester = SemesterListBox.SelectedItem as Semester;

            if (selectedSemester != null)
            {
                selectedSemester.InvokePropertyChanged();
                SelectedUser.Semesters.InvokePropertyChanged();
                SelectedUser.RefreshGradeString();
            }
        }

        private void SplitButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Semester selectedSemester = SemesterListBox.SelectedItem as Semester;

            if (selectedSemester != null)
            {
                selectedSemester.InvokePropertyChanged();
                SelectedUser.Semesters.InvokePropertyChanged();
                SelectedUser.RefreshGradeString();
            }
        }

        private void LectureDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Lecture selectedLecture = LectureListView.SelectedItem as Lecture;
            Semester selectedSemester = SemesterListBox.SelectedItem as Semester;

            if (selectedLecture != null && selectedSemester != null)
            {
                selectedSemester.Remove(selectedLecture);
                selectedSemester.InvokePropertyChanged();
                SelectedUser.Semesters.InvokePropertyChanged();
                SelectedUser.RefreshGradeString();
            }
        }

        private void GradeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            foreach(var v in SelectedUser.Semesters)
            {
                v.InvokePropertyChanged();
            }
            SelectedUser.Semesters.InvokePropertyChanged();
            SelectedUser.RefreshGradeString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = FilterString;

            var result = dlg.ShowDialog(this);

            if (result == true)
            {
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
                doc.AppendChild(SelectedUser.GetXMLNode(doc));
                doc.Save(dlg.FileName);
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
            catch (Exception e)
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

    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NotBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? b = value as bool?;
            if (b == true)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
