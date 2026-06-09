/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Represents a person address.
/// </summary>
public class PersonAddress
{
    
    // ● construction
    public PersonAddress()
    {
    }
    public PersonAddress(DataRow Row)
    {
        LoadFrom(Row);
    }

    // ● public
    public void LoadFrom(DataRow Row)
    {
        Id = Row.AsString("Id");
        PersonId = Row.AsString("PersonId");
        AddressType = (AddressType)Row.AsInteger("AddressTypeId");
        Code = Row.AsString("Code");
        Name = Row.AsString("Name");
        CountryId = Row.AsString("CountryId");
        CountryCode = Row.AsString("CountryCode");
        Country = Row.AsString("Country");
        Region = Row.AsString("Region");
        City = Row.AsString("City");
        PostalCode = Row.AsString("PostalCode");
        AddressLine1 = Row.AsString("AddressLine1");
        AddressLine2 = Row.AsString("AddressLine2");
        IsDefault = Row.AsBoolean("IsDefault");
        Notes = Row.AsString("Notes");
    }
    
    // ● properties
    /// <summary>
    /// Address Id.
    /// </summary>
    public string Id { get; set; } = "";
    /// <summary>
    /// Person Id.
    /// </summary>
    public string PersonId { get; set; } = "";
    /// <summary>
    /// Address type.
    /// </summary>
    public AddressType AddressType { get; set; }
    /// <summary>
    /// User-defined address code.
    /// </summary>
    public string Code { get; set; } = "";
    /// <summary>
    /// Address name.
    /// </summary>
    public string Name { get; set; } = "";
    /// <summary>
    /// Country Id.
    /// </summary>
    public string CountryId { get; set; } = "";
    /// <summary>
    /// Country code.
    /// </summary>
    public string CountryCode { get; set; } = "";
    /// <summary>
    /// Country name.
    /// </summary>
    public string Country { get; set; } = "";
    /// <summary>
    /// Region, state or province.
    /// </summary>
    public string Region { get; set; } = "";
    /// <summary>
    /// City.
    /// </summary>
    public string City { get; set; } = "";
    /// <summary>
    /// Postal code.
    /// </summary>
    public string PostalCode { get; set; } = "";
    /// <summary>
    /// First address line.
    /// </summary>
    public string AddressLine1 { get; set; } = "";
    /// <summary>
    /// Second address line.
    /// </summary>
    public string AddressLine2 { get; set; } = "";
    /// <summary>
    /// Indicates whether this is the default address of its type.
    /// </summary>
    public bool IsDefault { get; set; }
    /// <summary>
    /// User notes.
    /// </summary>
    public string Notes { get; set; } = "";
}