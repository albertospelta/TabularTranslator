using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularTranslator.ComponentModel
{
    internal class SortableBindingList<T> : BindingList<T> where T : class
    {
        private ListSortDirection _direction = ListSortDirection.Ascending;
        private PropertyDescriptor _property;
        private bool _sorted;

        protected override bool SupportsSortingCore => true;

        protected override bool IsSortedCore => _sorted;

        protected override ListSortDirection SortDirectionCore => _direction;
        
        protected override PropertyDescriptor SortPropertyCore => _property;
        
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            _property = prop;
            _direction = direction;

            if (Items is List<T> list)
            {
                list.Sort(Compare);
                _sorted = true;
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }

            int Compare(T x, T y)
            {
                var result = OnComparison(x, y);

                if (_direction == ListSortDirection.Descending)
                    result = -result;

                return result;
            }

            int OnComparison(T x, T y)
            {
                var xValue = x == null ? null : _property.GetValue(x);
                var yValue = y == null ? null : _property.GetValue(y);

                if (xValue == null)
                    return yValue == null ? 0 : -1;

                if (yValue == null)
                    return 1;

                if (xValue is IComparable)
                    return ((IComparable)xValue).CompareTo(yValue);

                if (xValue.Equals(yValue))
                    return 0;

                return xValue.ToString().CompareTo(yValue.ToString());
            }
        }

        protected override void RemoveSortCore()
        {
            _direction = ListSortDirection.Ascending;
            _property = null;
            _sorted = false;
        }


    }
}
