using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml;
using System.Runtime.CompilerServices;

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

        public override string ToString()
        {
            return GradeName;
        }
    }

    public class GradeSystem : ObservableCollection<GradeTuple>, INotifyPropertyChanged
    {
        #region Properties
        public double Maximum { get; set; }

        [IndexerName("Item")]
        public GradeTuple this[string index]
        {
            get
            {
                foreach(var v in Items)
                {
                    if (v.GradeName == index)
                        return v;
                }
                return null;
            }
        }
        #endregion

        #region Methods
        public void Add(string gradeString, double grade)
        {
            var item = new GradeTuple(gradeString.ToUpper(), grade);
        
            Add(item);
        }

        public new void Add(GradeTuple item)
        {
            int i = 0;
            char first = item.GradeName.First();
            char last = item.GradeName.Last();


            if (first == last)
            {
                if(first == 'F')
                {
                    base.Add(item);
                }
                return;
            }

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
                    {
                        break;
                    }
                }
                i++;
            }
            
            Insert(i, item);
            OnPropertyChanged("Item[]");
        }

        public void Remove(string gradeString)
        {
            Remove(Items.First((t) => t.GradeName == gradeString.ToUpper()));
            OnPropertyChanged("Item[]");
        }
        #endregion

        #region Constructor
        public GradeSystem(double maximum)
        {
            Maximum = maximum;

            if(Maximum == 4.5)
            {
                for(int i = 0; i < 7; i++)
                {
                    char c = i % 2 == 0 ? '+' : '0';
                    Add($"{Convert.ToChar('A' + i / 2)}{c}", 4.5 - i * 0.5);
                }
                Add("F", 0);
            }
            else
            {

            }
        }
        #endregion

        #region Overriding
        public override string ToString()
        {
            return base.ToString();
        }
        
        #endregion

        #region Inherited
        protected override event PropertyChangedEventHandler PropertyChanged;
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
                OnPropertyChanged(nameof(TestString));
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
                OnPropertyChanged(nameof(GradeTuple));
                OnPropertyChanged(nameof(TestString));
            }
        }
        public double Grade
        {
            get
            {
                return 0;
            }
        }

        public GradeTuple GradeTuple
        {
            get
            {
                if (GradeString != null)
                    return GradeSystem[GradeString];
                else
                    return GradeSystem[0];
            }
        }

        public string TestString
        {
            get
            {
                return $"{LectureName}/{Credit}/{GradeString}/{Grade}";
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
            GradeString = gradeString?.ToUpper();
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
