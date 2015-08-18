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
            char first = item.GradeName.First();
            char last = item.GradeName.Last();
            if (first == last)
                return;
            Func<char, int> myComparer = (ch) =>
            {
                switch (ch)
                {
                    case '+':
                        return 0;
                    case '0':
                        return 1;
                    case '-':
                        return 2;
                    default:
                        return 3;
                }
            };

            foreach(var v in Items)
            {
                char itemFirst = v.GradeName.First();
                char itemLast = v.GradeName.Last();
                if(itemFirst == first)
                {
                    if (myComparer(last) <= myComparer(itemLast))
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
            : this(4.5)
        {
        }

        public GradeSystem(double maximum)
        {
            Maximum = maximum;
            gradeDict = new Dictionary<string, double>();
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

    public class User : INotifyPropertyChanged
    {
        private string name;
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }
        private string studentNumber;
        public string StudentNumber { get { return studentNumber; } set { studentNumber = value; OnPropertyChanged(nameof(StudentNumber)); } }
        private double maximum;
        public double Maximum { get { return maximum; } set { maximum = value; OnPropertyChanged(nameof(Maximum)); } }
        public string GradeString
        {
            get
            {
                return $"{Semesters.CreditSum} / {Maximum}";
            }
        }

        private SemesterCollection semesters;
        public SemesterCollection Semesters { get { return semesters; }
            set { semesters = value; OnPropertyChanged(nameof(Semesters)); }
        }
        private GradeSystem gradeSystem;
        public GradeSystem GradeSystem
        {
            get { return gradeSystem; }
            set { gradeSystem = value; OnPropertyChanged(nameof(GradeSystem)); }
        }
        
        public User(string name, string studentNumber, double maximum)
        {
            Name = name;
            StudentNumber = studentNumber;
            Maximum = maximum;
            Semesters = new SemesterCollection();
            GradeSystem = new GradeSystem(Maximum);
            Lecture.GradeSystem = GradeSystem;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class Lecture : INotifyPropertyChanged
    {
        private string gradeString;
        private int credit;

        public string LectureName { get; set; }
        public int Credit
        {
            get
            {
                return credit;
            }
            set
            {
                credit = value;
                OnPropertyChanged(nameof(Credit));
            }
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
                OnPropertyChanged(nameof(GradeString));
                OnPropertyChanged(nameof(Grade));
            }
        }
        public double Grade
        {
            get
            {
                var v = GradeSystem?[GradeString];
                if (v.HasValue)
                    return v.Value;
                else
                    return 0;
            }
        }

        public static GradeSystem GradeSystem
        {
            get; set;
        } = null;
        
        public Lecture(string lectureName, int credit, string gradeString)
        {
            LectureName = lectureName;
            Credit = credit;
            GradeString = gradeString;
        }

        public override string ToString()
        {
            return $"{LectureName}/{Credit}/{GradeString}/{Grade}";
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }

    public class Semester : ObservableCollection<Lecture>, INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public double GradeSum
        {
            get
            {
                double sum = 0;
                foreach (var v in Items)
                {
                    sum += v.Grade * v.Credit;
                }
                return sum;
            }
        }

        public int CreditSum
        {
            get
            {
                int sum = 0;
                foreach(var v in Items)
                {
                    sum += v.Credit;
                }
                return sum;
            }
        }

        public double Grade
        {
            get
            {
                return CreditSum != 0 ? GradeSum / CreditSum : 0;
            }
        }

        protected override event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public override string ToString()
        {
            return $"{Name}";
        }
    }
    public class SemesterCollection : ObservableCollection<Semester>
    {
        public double CreditSum
        {
            get
            {
                double sum = 0;
                foreach (var v in Items)
                {
                    sum += v.CreditSum;
                }
                return sum;
            }
        }

        public double GradeSum
        {
            get
            {
                double sum = 0;
                foreach (var v in Items)
                {
                    sum += v.GradeSum;
                }
                return sum;
            }
        }

        public double Grade
        {
            get
            {
                return CreditSum != 0 ? GradeSum / CreditSum : 0;
            }
        }

        public void Add(string semesterName)
        {
            var s = new Semester();
            s.Name = semesterName;
            Add(s);
        }
    }
}
