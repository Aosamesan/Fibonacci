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
                gradeValue = Convert.ToInt32(value * 10) / 10.0;
                OnPropertyChanged(nameof(GradeValue));
            }
        }
        public GradeTuple(string gradeName, double gradeValue)
        {
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

        static GradeSystem()
        {
        }
        #endregion

        #region Properties
        public double Maximum { get; set; }
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
            if (gradeDict.ContainsKey(gradeString.ToUpper()))
                return;
            var item = new GradeTuple(gradeString.ToUpper(), grade);
            gradeDict.Add(item.GradeName, item.GradeValue);
            Add(item);
        }

        public new void Add(GradeTuple item)
        {
            int i = 0;
            foreach(var t in Items)
            {
                if(string.Compare(t.GradeName, item.GradeName) == 1)
                {
                    break;
                }
                i++;
            }
            base.Insert(i, item);
        }

        public void Remove(string gradeString)
        {
            gradeDict.Remove(gradeString.ToUpper());
            base.Remove(Items.First((t) => t.GradeName == gradeString.ToUpper()));
        }
        #endregion

        #region Constructor
        public GradeSystem()
        {
            gradeDict = new Dictionary<string, double>();
            Maximum = 4.5;
        }
        #endregion

        #region Overriding
        public override string ToString()
        {
            return base.ToString();
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

    //public class GradeSystemCollection : ObservableCollection<GradeSystem>, INotifyPropertyChanged
    //{
    //    private GradeSystem selectedItem;
    //    public GradeSystem SelectedItem
    //    {
    //        get
    //        {
    //            return selectedItem;
    //        }
    //        set
    //        {
    //            selectedItem = value;
    //            OnPropertyChanged(
    //                new PropertyChangedEventArgs(nameof(SelectedItem)));
    //        }

    //    }

    //    public GradeSystemCollection()
    //        : base()
    //    {
    //        // 4.5
    //        GradeSystem g = new GradeSystem("4.5 만점", 4.5);
    //        g.Add("A+", 4.5);
    //        g.Add("A", 4);
    //        g.Add("B+", 3.5);
    //        g.Add("B", 3);
    //        g.Add("C+", 2.5);
    //        g.Add("C", 2);
    //        g.Add("D+", 1.5);
    //        g.Add("D", 1.0);
    //        g.Add("F", 0);
    //        Items.Add(g);

    //        SelectedItem = g;
    //        // 4.5 (-)
    //        g = new GradeSystem("4.5 만점(-)", 4.5);
    //        g.Add("A+", 4.5);
    //        g.Add("A", 4.3);
    //        g.Add("A-", 4);
    //        g.Add("B+", 3.5);
    //        g.Add("B", 3.3);
    //        g.Add("B-", 3);
    //        g.Add("C+", 2.5);
    //        g.Add("C", 2.3);
    //        g.Add("C-", 2);
    //        g.Add("D+", 1.5);
    //        g.Add("D", 1.3);
    //        g.Add("D-", 1.0);
    //        g.Add("F", 0);
    //        Items.Add(g);
    //        // 4.3
    //        g = new GradeSystem("4.3 만점", 4.3);
    //        g.Add("A+", 4.5);
    //        g.Add("A", 4);
    //        g.Add("A-", 3.7);
    //        g.Add("B+", 3.3);
    //        g.Add("B", 3);
    //        g.Add("B-", 2.7);
    //        g.Add("C+", 2.3);
    //        g.Add("C", 2);
    //        g.Add("C-", 1.7);
    //        g.Add("D+", 1.3);
    //        g.Add("D", 1);
    //        g.Add("D-", 0.7);
    //        g.Add("F", 0);
    //        Items.Add(g);
    //    }

    //    public new void Add(GradeSystem g) => Console.Beep();
    //}

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
