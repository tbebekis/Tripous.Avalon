/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Db global settings
/// </summary>
public class DbGlobalSettings: SettingsBase, INotifyPropertyChanged
{
    bool fIdFieldsVisible;
    int fDefaultRowLimit;
    int fDefaultCommandTimeoutSeconds = 300;
    int fLocatorMinimumSearchTextLength;
    int fLocatorMaximumDropDownRows;
    bool fLogSqlStatements;
    
    // ● private  
    void NotifyPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    
    // ● construction  
    internal DbGlobalSettings()
    {
    }
    
    // ● properties
    /// <summary>
    /// The default RowLimit for browser SELECTs.
    /// </summary>
    public int DefaultRowLimit
    {
        get
        {
            if (fDefaultRowLimit >= 100 && fDefaultRowLimit <= 1500)
                return fDefaultRowLimit;
            return 300;
        }
        set { if (fDefaultRowLimit != value) { fDefaultRowLimit = value; NotifyPropertyChanged(nameof(DefaultRowLimit)); } }
    }
    /// <summary>
    /// Default command time in seconds
    /// </summary>
    public int DefaultCommandTimeoutSeconds
    {
        get => fDefaultCommandTimeoutSeconds >= 15 ? fDefaultCommandTimeoutSeconds : 15;
        set { if (fDefaultCommandTimeoutSeconds != value) { fDefaultCommandTimeoutSeconds = value; NotifyPropertyChanged(nameof(DefaultCommandTimeoutSeconds)); } }
    }
    /// <summary>
    /// When true then <see cref="FieldDef"/> fields with a name ending in "Id" are visible, else are hidden.
    /// </summary>
    public bool IdFieldsVisible 
    {
        get => fIdFieldsVisible;
        set { if (fIdFieldsVisible != value) { fIdFieldsVisible = value; NotifyPropertyChanged(nameof(IdFieldsVisible)); } }
    }
    
    /// <summary>
    /// How many characters to accept as minimum in locator search terms.
    /// </summary>
    public int LocatorMinimumSearchTextLength
    {
        get => fLocatorMinimumSearchTextLength >= 3 && fLocatorMinimumSearchTextLength <= 6 ? fLocatorMinimumSearchTextLength : 3;
        set { if (fLocatorMinimumSearchTextLength != value) { fLocatorMinimumSearchTextLength = value; NotifyPropertyChanged(nameof(LocatorMinimumSearchTextLength)); } }
    }
    /// <summary>
    /// How many rows to accept as valid maximum limit in locator searches. Exceeding this limit results in an empty result and warning.
    /// </summary>
    public int LocatorMaximumDropDownRows
    {
        get => fLocatorMaximumDropDownRows >= 30 && fLocatorMaximumDropDownRows <= 150 ? fLocatorMaximumDropDownRows : 75;
        set { if (fLocatorMaximumDropDownRows != value) { fLocatorMaximumDropDownRows = value; NotifyPropertyChanged(nameof(LocatorMaximumDropDownRows)); } }
    }
    /// <summary>
    /// When true then all executed SQL statements are logged.
    /// </summary>
    public bool LogSqlStatements
    {
        get => fLogSqlStatements;
        set { if (fLogSqlStatements != value) { fLogSqlStatements = value; NotifyPropertyChanged(nameof(LogSqlStatements)); } }
    }
    
    // ● events
    public event PropertyChangedEventHandler PropertyChanged;
}
