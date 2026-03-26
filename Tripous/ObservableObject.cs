using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tripous;

public class ObservableObject : INotifyPropertyChanged
{
    protected void OnPropertyChanged([CallerMemberName] string PropertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    protected bool SetProperty<T>(ref T PropertyBackingField, T NewValue, [CallerMemberName] string PropertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(PropertyBackingField, NewValue))
        {
            return false;
        }

        PropertyBackingField = NewValue;
        OnPropertyChanged(PropertyName);
        return true;
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
}