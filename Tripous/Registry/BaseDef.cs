/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Base class for all descriptors
/// </summary>
public class BaseDef: IDef, IJsonLoadable, INotifyPropertyChanged
{
    protected string fTitleKey;
    protected string fName;

    // ● protected  
    protected virtual void NotifyPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    protected virtual string GetTitleKey() => !string.IsNullOrWhiteSpace(fTitleKey)? fTitleKey: Name;
    
    // ● construction  
    public BaseDef()
    {
    }
    
    // ● public  
    public override string ToString() => Name;
    /// <summary>
    /// Updates references such as when an instance has references to other instances, e.g. tables of a module definition.
    /// </summary>
    public virtual void UpdateReferences()
    {
    }
    public virtual void JsonLoaded() => UpdateReferences();
 
    public virtual BaseDef CreateNew() => Activator.CreateInstance(this.GetType()) as BaseDef;
    /// <summary>
    /// Throws an exception if this descriptor is not fully defined
    /// </summary>
    public virtual void CheckDescriptor()
    {
        if (string.IsNullOrWhiteSpace(this.Name))
            Sys.Throw(Texts.GS($"E_{typeof(BaseDef)}_NoName", $"{typeof(BaseDef)} must have a Name"));
    }

    /// <summary>
    /// Assigns property values from a source instance.
    /// </summary>
    public virtual void Assign(IDef Source) => Json.AssignObject(Source, this);
    /// <summary>
    /// Returns a clone of this instance.
    /// </summary>
    public virtual IDef Clone()
    {
        BaseDef Result = CreateNew();
        Json.AssignObject(this, Result);
        return Result;
    }
    /// <summary>
    /// Clears the property values of this instance.
    /// </summary>
    public virtual void Clear()
    {
        BaseDef Empty = CreateNew();
        Json.AssignObject(Empty, this);
    }

    public virtual string SplitTitleKeyToWordsWithPluralEnding()
    {
        fTitleKey = !string.IsNullOrWhiteSpace(fTitleKey)? fTitleKey: Name;
        fTitleKey = fTitleKey.ToPlural().SplitToWords();
        return fTitleKey;
    }
    
    // ● properties  
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

    [JsonIgnore] public virtual bool IsTitleKeyEmpty => string.IsNullOrWhiteSpace(fTitleKey);
    [JsonIgnore] public virtual string Title => Texts.L(TitleKey);
    [JsonIgnore] public virtual object Tag { get; set; }
    [JsonIgnore] public virtual bool IsSerializable => true;

    // ● events
    public event PropertyChangedEventHandler PropertyChanged;
}