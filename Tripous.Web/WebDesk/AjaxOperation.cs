/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Base class for a WebDesk Ajax operation.
/// </summary>
public abstract class AjaxOperation
{
    // ● private
    string fName;

    // ● protected
    /// <summary>
    /// Returns the operation name declared by the operation attribute.
    /// </summary>
    protected virtual string GetName()
    {
        AjaxOperationAttribute Attribute = GetType().GetCustomAttribute<AjaxOperationAttribute>();
        return Attribute != null ? Attribute.Name : string.Empty;
    }
    /// <summary>
    /// Returns a string request parameter.
    /// </summary>
    protected string GetStringParam(AjaxRequest Request, string Name)
    {
        object Value = Request.GetParam(Name);
        if (Value == null)
            return string.Empty;
        if (Value is JsonElement Element)
            return Element.ValueKind == JsonValueKind.String ? Element.GetString() : Element.ToString();
        return Convert.ToString(Value, CultureInfo.InvariantCulture)?.Trim() ?? string.Empty;
    }

    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public abstract AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context);

    // ● properties
    /// <summary>
    /// Gets the operation name.
    /// </summary>
    public string Name => fName ??= GetName();
}
