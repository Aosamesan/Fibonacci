using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml;

namespace Fibonacci
{
    // Credit System
    public class GradeTuple : INotifyPropertyChanged
    {
        private string gradeName;
        private double gradeValue;
        public event PropertyChangedEventHandler PropertyChanged;
        private Func<double, double> intervalCalc;

        public string GradeName
        {
            get
            {
                return gradeName;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;
                gradeName = value;
                OnPropertyChanged(nameof(GradeName));
            }
        }
        public double GradeValue
        {
            get
            {
                return gradeValue;
            }
            set
            {
                if (value < 0)
                    return;
                gradeValue = intervalCalc(value);
                OnPropertyChanged(nameof(GradeValue));
            }
        }
        public GradeTuple(string gradeName, double gradeValue, string gradeSystem)
        {
            intervalCalc = GradeSystem.GradeIntervalCalcDict[gradeSystem];
            GradeName = gradeName;
            GradeValue = gradeValue;
        }

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class GradeSystem : ObservableCollection<GradeTuple>, INotifyPropertyChanged, IDisposable
    {
        #region Member Field
        private Dictionary<string, double> gradeDict;
        #endregion

        #region Static Member
        public static Dictionary<string, Func<double, double>> GradeIntervalCalcDict { get; }

        static GradeSystem()
        {
            GradeIntervalCalcDict = new Dictionary<string, Func<double, double>>();
            Func<double, double> lambda = (value) =>
            {
                double result = 0;
                var s = value - Math.Floor(value);
                if (s < 0.5)
                    result = Math.Floor(value);
                else
                    result = Math.Floor(value) + 0.5;
                return result;
            };
            GradeIntervalCalcDict.Add("4.5 만점", lambda);
            lambda = (value) =>
            {
                double result = 0;
                var flat = Math.Floor(value);
                var dif = value - flat;

                if (dif >= 0.8)
                    result = flat + 1;
                else if (dif >= 0.5)
                    result = flat + 0.5;
                else if (dif >= 0.2)
                    result = flat + 0.3;
                else
                    result = flat;
                return result;
            };
            GradeIntervalCalcDict.Add("4.5 만점(-)", lambda);
            lambda = (value) =>
            {
                double result = 0;
                var flat = Math.Floor(value);
                var dif = value - flat;

                if (dif >= 0.7)
                    result = flat + 0.7;
                else if (dif >= 0.2)
                    result = flat + 0.3;
                else
                    result = flat;
                return result;
            };
            GradeIntervalCalcDict.Add("4.3 만점", lambda);
        }
        #endregion

        #region Properties
        public string Name { get; }
        public double Maximum { get; }
        public Dictionary<string, double>.KeyCollection Keys { get { return gradeDict.Keys; } }
        public Dictionary<string, double>.ValueCollection Values { get { return gradeDict.Values; } }
        public double this[string key]
        {
            get
            {
                if (!Keys.Contains(key))
                    return 0;
                return gradeDict[key];
            }
        }
        #endregion

        #region Methods
        public void Add(string gradeString, double grade)
        {
            var item = new GradeTuple(gradeString.ToUpper(), grade, Name);
            gradeDict.Add(item.GradeName, item.GradeValue);
            Items.Add(item);
        }

        public void Remove(string gradeString)
        {
            gradeDict.Remove(gradeString);
        }
        #endregion

        #region Constructor
        public GradeSystem(string name, double max)
        {
            gradeDict = new Dictionary<string, double>();
            Name = name;
            Maximum = max;
        }
        #endregion

        #region Overriding
        public override string ToString()
        {
            return Name;
        }

        public void Dispose()
            => gradeDict.Clear();
        #endregion

        #region Inherited
        protected event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }

    public class GradeSystemCollection : ObservableCollection<GradeSystem>, INotifyPropertyChanged
    {
        private GradeSystem selectedItem;
        public GradeSystem SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                OnPropertyChanged(
                    new PropertyChangedEventArgs(nameof(SelectedItem)));
            }

        }

        public GradeSystemCollection()
            : base()
        {
            // 4.5
            GradeSystem g = new GradeSystem("4.5 만점", 4.5);
            g.Add("A+", 4.5);
            g.Add("A", 4);
            g.Add("B+", 3.5);
            g.Add("B", 3);
            g.Add("C+", 2.5);
            g.Add("C", 2);
            g.Add("D+", 1.5);
            g.Add("D", 1.0);
            g.Add("F", 0);
            Items.Add(g);

            SelectedItem = g;
            // 4.5 (-)
            g = new GradeSystem("4.5 만점(-)", 4.5);
            g.Add("A+", 4.5);
            g.Add("A", 4.3);
            g.Add("A-", 4);
            g.Add("B+", 3.5);
            g.Add("B", 3.3);
            g.Add("B-", 3);
            g.Add("C+", 2.5);
            g.Add("C", 2.3);
            g.Add("C-", 2);
            g.Add("D+", 1.5);
            g.Add("D", 1.3);
            g.Add("D-", 1.0);
            g.Add("F", 0);
            Items.Add(g);
            // 4.3
            g = new GradeSystem("4.3 만점", 4.3);
            g.Add("A+", 4.5);
            g.Add("A", 4);
            g.Add("A-", 3.7);
            g.Add("B+", 3.3);
            g.Add("B", 3);
            g.Add("B-", 2.7);
            g.Add("C+", 2.3);
            g.Add("C", 2);
            g.Add("C-", 1.7);
            g.Add("D+", 1.3);
            g.Add("D", 1);
            g.Add("D-", 0.7);
            g.Add("F", 0);
            Items.Add(g);
        }

        public new void Add(GradeSystem g) => Console.Beep();
    }

    public class Lecture : INotifyPropertyChanged
    {
        private string gradeString;

        public string LectureName { get; set; }
        public int Credit
        {
            get; set;
        }
        public string GradeString
        {
            get
            {
                return gradeString;
            }
            set
            {
                gradeString = value;
                OnPropertyChanged("GradeString");
                OnPropertyChanged("Grade");
            }
        }
        public double Grade
        {
            get
            {
                return GradeSystem[GradeString];
            }
        }


        public GradeSystem GradeSystem { get; set; }
        
                
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
