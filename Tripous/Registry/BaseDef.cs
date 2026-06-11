/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Base class for all descriptor classes.
/// </summary>
public class BaseDef: IDef, IJsonLoadable, INotifyPropertyChanged
{
    /// <summary>
    /// Field
    /// </summary>
    protected string fTitleKey;
    /// <summary>
    /// Field
    /// </summary>
    protected string fName;

    // ● protected  
    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    protected virtual void NotifyPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    /// <summary>
    /// Returns the effective title key.
    /// </summary>
    protected virtual string GetTitleKey() => !string.IsNullOrWhiteSpace(fTitleKey)? fTitleKey: Name;
    
    // ● construction  
    /// <summary>
    /// Constructor.
    /// </summary>
    public BaseDef()
    {
    }
    
    // ● public  
    /// <summary>
    /// Returns the descriptor name.
    /// </summary>
    public override string ToString() => Name;
    /// <summary>
    /// Updates internal references after loading or assignment.
    /// </summary>
    public virtual void UpdateReferences()
    {
    }
    /// <summary>
    /// Called after this instance is loaded from JSON.
    /// </summary>
    public virtual void JsonLoaded() => UpdateReferences();
    /// <summary>
    /// Creates a new instance of the same descriptor type.
    /// </summary>
    public virtual BaseDef CreateNew() => Activator.CreateInstance(this.GetType()) as BaseDef;
    /// <summary>
    /// Checks whether this descriptor is fully defined.
    /// Throws an exception when required values are missing.
    /// </summary>
    public virtual void CheckDescriptor()
    {
        if (string.IsNullOrWhiteSpace(this.Name))
            Sys.Throw(Texts.GS($"E_{typeof(BaseDef)}_NoName", $"{typeof(BaseDef)} must have a Name"));
    }
    /// <summary>
    /// Assigns property values from a source descriptor.
    /// </summary>
    public virtual void Assign(IDef Source) => Json.AssignObject(Source, this);
    /// <summary>
    /// Returns a clone of this descriptor.
    /// </summary>
    public virtual IDef Clone()
    {
        BaseDef Result = CreateNew();
        Json.AssignObject(this, Result);
        return Result;
    }
    /// <summary>
    /// Clears this descriptor by assigning values from a new empty instance.
    /// </summary>
    public virtual void Clear()
    {
        BaseDef Empty = CreateNew();
        Json.AssignObject(Empty, this);
    }
    /// <summary>
    /// Converts the title key to a plural, word-split form and returns it.
    /// </summary>
    public virtual string SplitTitleKeyToWordsWithPluralEnding()
    {
        fTitleKey = !string.IsNullOrWhiteSpace(fTitleKey)? fTitleKey: Name;
        fTitleKey = fTitleKey.ToPlural().SplitToWords();
        return fTitleKey;
    }
    
    // ● properties  
    /// <summary>
    /// Gets or sets the descriptor name.
    /// </summary>
    public virtual string Name
    {
        get => !string.IsNullOrWhiteSpace(fName) ? fName : this.GetType().FullName;
        set
        {
            if (fName != value)
            {
                fName = value;
                NotifyPropertyChanged(nameof(Name));
                NotifyPropertyChanged(nameof(TitleKey));
                NotifyPropertyChanged(nameof(Title));
            }
        }
    }
    /// <summary>
    /// Gets or sets the localization key used for the descriptor title.
    /// </summary>
    public virtual string TitleKey
    {
        get => GetTitleKey();
        set
        {
            if (fTitleKey != value)
            {
                fTitleKey = value;
                NotifyPropertyChanged(nameof(TitleKey));
                NotifyPropertyChanged(nameof(Title));
            }
        }
    }
    /// <summary>
    /// Gets a value indicating whether the title key is empty.
    /// </summary>
    [JsonIgnore] public virtual bool IsTitleKeyEmpty => string.IsNullOrWhiteSpace(fTitleKey);
    /// <summary>
    /// Gets the localized descriptor title.
    /// </summary>
    [JsonIgnore] public virtual string Title => Texts.L(TitleKey);
    /// <summary>
    /// Gets or sets a user-defined value associated with this descriptor.
    /// </summary>
    [JsonIgnore] public virtual object Tag { get; set; }
    /// <summary>
    /// Gets a value indicating whether this descriptor should be serialized.
    /// </summary>
    [JsonIgnore] public virtual bool IsSerializable => true;

    // ● events
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
}