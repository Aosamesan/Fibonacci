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
        public bool IsSealed { get; }

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
        public GradeTuple(string gradeName, double gradeValue, bool isSealed = false)
        {
            GradeName = gradeName;
            GradeValue = gradeValue;
            IsSealed = isSealed;
        }

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public override string ToString()
        {
            return GradeName;
        }

        public XmlNode GetXMLNode(XmlDocument doc)
        {
            var name = doc.CreateAttribute("Name");
            name.Value = GradeName;
            var value = doc.CreateAttribute("Value");
            value.Value = $"{GradeValue}";
            var isSealed = doc.CreateAttribute("IsSealed");
            isSealed.Value = $"{IsSealed}";
            XmlNode node = doc.CreateElement("GradeTuple");
            node.Attributes.Append(name);
            node.Attributes.Append(value);
            node.Attributes.Append(isSealed);
            return node;
        }

        public static GradeTuple GetGradeTupleFromXMLNode(XmlNode node)
        {
            if (node.Name != "GradeTuple")
                return null;
            string s = node.Attributes["Name"].Value;
            double d = Convert.ToDouble(node.Attributes["Value"].Value);

            string isSealed = "false";
            try
            {
                isSealed = node.Attributes["IsSealed"].Value;
            }
            catch(Exception ex)
            {
                isSealed = "true";
            }

            bool b = Convert.ToBoolean(isSealed);
            return new GradeTuple(s, d, b);
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
        public void Add(string gradeString, double grade, bool isSealed = false)
        {
            var item = new GradeTuple(gradeString.ToUpper(), grade, isSealed);
        
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
                    if (myComparer(last) < myComparer(itemLast))
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
            var item = Items.First((t) => t.GradeName == gradeString.ToUpper());
            if (item == null || item.IsSealed)
                return;
            Remove(item);
            OnPropertyChanged("Item[]");
        }

        public XmlNode GetXMLNode(XmlDocument doc)
        {
            var maximum = doc.CreateAttribute("Maximum");
            maximum.Value = $"{Maximum}";
            XmlNode node = doc.CreateElement("GradeSystem");
            node.Attributes.Append(maximum);
            foreach(var v in Items)
            {
                node.AppendChild(v.GetXMLNode(doc));
            }
            return node;
        }
        
        public static GradeSystem GetGradeSystemFromXMLNode(XmlNode node)
        {
            if (node.Name != "GradeSystem")
                return null;
            double d = Convert.ToDouble(node.Attributes["Maximum"].Value);
            GradeSystem gs = new GradeSystem(d);
            foreach(XmlNode v in node.ChildNodes)
            {
                var c = GradeTuple.GetGradeTupleFromXMLNode(v);
                gs.Add(c);
            }
            return gs;
        }

        #endregion

        #region Constructor
        public GradeSystem(double maximum, bool newSystem = false)
        {
            Maximum = maximum;

            if (newSystem)
            {
                if (Maximum == 4.5)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        char c = i % 2 == 0 ? '+' : '0';
                        Add($"{Convert.ToChar('A' + i / 2)}{c}", Maximum - i * 0.5, true);
                    }
                    Add("F", 0, true);
                }
                else
                {
                    for (int i = 0; i < 12; i++)
                    {
                        char c = i % 3 == 0 ? '+' : i % 3 == 1 ? '0' : '-';
                        Add($"{Convert.ToChar('A' + i / 3)}{c}", maximum, true);
                        maximum -= (i % 3 == 2 ? 0.4 : 0.3);
                    }
                    Add("F", 0, true);
                }
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
                string s = Semesters.Grade == 0 ? "0" : $"{Semesters.Grade:#.##}";
                return $"{s} / {Maximum}";
            }
        }

        public void RefreshGradeString()
        {
            OnPropertyChanged(nameof(GradeString));
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
            GradeSystem = new GradeSystem(Maximum, true);
            Lecture.GradeSystem = GradeSystem;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public XmlNode GetXMLNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement("User");
            var attr = doc.CreateAttribute("Name");
            attr.Value = Name;
            node.Attributes.Append(attr);
            attr = doc.CreateAttribute("StudentNumber");
            attr.Value = StudentNumber;
            node.Attributes.Append(attr);
            attr = doc.CreateAttribute("Maximum");
            attr.Value = $"{Maximum}";
            node.Attributes.Append(attr);

            node.AppendChild(GradeSystem.GetXMLNode(doc));
            node.AppendChild(Semesters.GetXMLNode(doc));

            return node;
        }

        public static User GetUserFromXMLNode(XmlNode node)
        {
            if (node.Name != "User")
                return null;
            string name = node.Attributes["Name"].Value;
            string studentNumber = node.Attributes["StudentNumber"].Value;
            double max = Convert.ToDouble(node.Attributes["Maximum"].Value);
            GradeSystem gs = GradeSystem.GetGradeSystemFromXMLNode(node["GradeSystem"]);
            SemesterCollection sc = SemesterCollection.GetSemesterCollectionFromXMLNode(node["SemesterCollection"]);
            User user = new User(name, studentNumber, max);
            user.GradeSystem = gs;
            user.Semesters = sc;
            return user;
        }
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
                OnPropertyChanged(nameof(GradeTuple));
            }
        }
        public double Grade
        {
            get
            {
                if(GradeSystem?[GradeString] != null)
                    return GradeSystem[GradeString].GradeValue;
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

        public static GradeSystem GradeSystem
        {
            get; set;
        } = null;
        
        public Lecture(string lectureName, int credit, string gradeString)
        {
            LectureName = lectureName;
            Credit = credit;
            GradeString = gradeString?.ToUpper();
            OnPropertyChanged(nameof(GradeString));
        }

        public override string ToString()
        {
            return $"{LectureName}/{Credit}/{GradeString}/{Grade}";
        }

        public XmlNode GetXMLNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement("Lecture");
            var attr = doc.CreateAttribute("Name");
            attr.Value = LectureName;
            node.Attributes.Append(attr);
            attr = doc.CreateAttribute("Credit");
            attr.Value = $"{Credit}";
            node.Attributes.Append(attr);
            attr = doc.CreateAttribute("Grade");
            attr.Value = GradeString;
            node.Attributes.Append(attr);
            return node;
        }

        public static Lecture GetLectureFromXMLNode(XmlNode node)
        {
            if (node.Name != "Lecture")
                return null;
            string name = node.Attributes["Name"].Value;
            int credit = Convert.ToInt32(node.Attributes["Credit"].Value);
            string gradeString = node.Attributes["Grade"].Value;
            return new Lecture(name, credit, gradeString);
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

        public string Grade
        {
            get
            {
                return CreditSum != 0 ? $"{GradeSum/CreditSum:#.##}" : "0";
            }
        }

        public void InvokePropertyChanged()
        {
            OnPropertyChanged(nameof(CreditSum));
            OnPropertyChanged(nameof(GradeSum));
            OnPropertyChanged(nameof(Grade));
        }

        protected override event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public override string ToString()
        {
            return $"{Name}";
        }

        public XmlNode GetXMLNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement("Semester");
            var attr = doc.CreateAttribute("Name");
            attr.Value = Name;
            node.Attributes.Append(attr);
            foreach(var v in Items)
            {
                node.AppendChild(v.GetXMLNode(doc));
            }
            return node;
        }

        public static Semester GetSemesterFromXMLNode(XmlNode node)
        {
            if (node.Name != "Semester")
                return null;
            string s = node.Attributes["Name"].Value;
            Semester semester = new Semester();
            semester.Name = s;
            foreach(XmlNode v in node.ChildNodes)
            {
                var item = Lecture.GetLectureFromXMLNode(v);
                semester.Add(item);
            }

            return semester;
        }
    }
    public class SemesterCollection : ObservableCollection<Semester>, INotifyPropertyChanged
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

        public void InvokePropertyChanged()
        {
            OnPropertyChanged(nameof(CreditSum));
            OnPropertyChanged(nameof(GradeSum));
            OnPropertyChanged(nameof(Grade));
        }

        public void Add(string semesterName)
        {
            var s = new Semester();
            s.Name = semesterName;
            Add(s);
        }

        protected override event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        public XmlNode GetXMLNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement("SemesterCollection");
            foreach(var v in Items)
            {
                node.AppendChild(v.GetXMLNode(doc));
            }
            return node;
        }

        public static SemesterCollection GetSemesterCollectionFromXMLNode(XmlNode node)
        {
            if (node.Name != "SemesterCollection")
                return null;
            SemesterCollection result = new SemesterCollection();
            foreach(XmlNode n in node.ChildNodes)
            {
                var v = Semester.GetSemesterFromXMLNode(n);
                result.Add(v);
            }

            return result;
        }
    }
}
