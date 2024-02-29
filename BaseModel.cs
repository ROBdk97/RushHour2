using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RushHour2
{
    public class BaseModel : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        #endregion PropertyChanged
    }
}
