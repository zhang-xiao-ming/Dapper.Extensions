using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DomainModel.Annotations;

namespace DomainModel
{
    public class PropertyChangedModel : PropertyChangedBase
    {
        private string _id;
        private string _name;
        private int _age;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;

                //模拟开发人员忘记通知属性值变化的情况，如果不通知，则这个属性值的变更将无法更新到DB中去
                //NotifyPropertyChanged("Age");
            }
        }
    }

    public class PropertyChangedBase
    {
        private readonly IList<string> _propertyChangedList;

        public PropertyChangedBase()
        {
            _propertyChangedList = new List<string>();
        }

        public IList<string> PropertyChangedList
        {
            get { return _propertyChangedList; }
        }
        public void NotifyPropertyChanged(string propertyName)
        {
            if (_propertyChangedList != null && !_propertyChangedList.Contains(propertyName))
            {
                _propertyChangedList.Add(propertyName);
            }
        }
    }


}
