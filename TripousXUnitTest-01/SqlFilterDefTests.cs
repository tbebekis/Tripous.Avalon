namespace TripousXUnitTest_01;

using Xunit;
using Tripous;
using Tripous.Data;


public class SqlFilterDefTests
{
    // ------------------------------------------------------------
    // helpers
    // ------------------------------------------------------------
    private static SqlFilterDef Create(string rawTag)
    {
        return new SqlFilterDef
        {
            RawTag = rawTag
        };
    }

    private static void AssertNoErrors(SqlFilterDef f)
    {
        Assert.False(f.HasErrors, f.Errors);
        Assert.True(string.IsNullOrWhiteSpace(f.Errors), f.Errors);
    }

    private static void AssertHasErrors(SqlFilterDef f)
    {
        Assert.True(f.HasErrors);
        Assert.False(string.IsNullOrWhiteSpace(f.Errors));
    }

    // ------------------------------------------------------------
    // general / format
    // ------------------------------------------------------------
    [Fact]
    public void Null_RawTag_Uses_Defaults_And_No_Errors()
    {
        var f = Create(null);

        Assert.Equal(SqlFilterType.String, f.Type);
        Assert.False(f.IsNumeric);
        Assert.False(f.IsMultiple);
        Assert.Equal(DateRange.Custom, f.DateRange);
        Assert.Equal(string.Empty, f.Text);
        Assert.Equal(string.Empty, f.Label);
        AssertNoErrors(f);
    }

    [Fact]
    public void Empty_RawTag_Uses_Defaults_And_No_Errors()
    {
        var f = Create(string.Empty);

        Assert.Equal(SqlFilterType.String, f.Type);
        Assert.False(f.IsNumeric);
        Assert.False(f.IsMultiple);
        Assert.Equal(DateRange.Custom, f.DateRange);
        Assert.Equal(string.Empty, f.Text);
        Assert.Equal(string.Empty, f.Label);
        AssertNoErrors(f);
    }

    [Fact]
    public void Whitespace_RawTag_Uses_Defaults_And_No_Errors()
    {
        var f = Create("   ");

        Assert.Equal(SqlFilterType.String, f.Type);
        Assert.False(f.IsNumeric);
        Assert.False(f.IsMultiple);
        Assert.Equal(DateRange.Custom, f.DateRange);
        Assert.Equal(string.Empty, f.Text);
        Assert.Equal(string.Empty, f.Label);
        AssertNoErrors(f);
    }

    [Fact]
    public void Invalid_Format_Missing_Brackets_Has_Error()
    {
        var f = Create("date:Order Date");

        AssertHasErrors(f);
    }

    [Fact]
    public void Invalid_Format_Empty_Tag_Has_Error()
    {
        var f = Create("[[]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void Unknown_Filter_Type_Has_Error()
    {
        var f = Create("[[boing:Label]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void Trims_Whitespace_Around_Tag_And_Parts()
    {
        var f = Create("   [[  string :  Customer Name  ]]   ");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.String, f.Type);
        Assert.Equal("Customer Name", f.Label);
    }

    // ------------------------------------------------------------
    // shorthand / string
    // ------------------------------------------------------------
    [Fact]
    public void Shorthand_Label_Parses_As_String()
    {
        var f = Create("[[Customer Name]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.String, f.Type);
        Assert.Equal("Customer Name", f.Label);
        Assert.False(f.IsNumeric);
        Assert.False(f.IsMultiple);
        Assert.Equal(string.Empty, f.Text);
    }

    [Fact]
    public void String_Label_Parses()
    {
        var f = Create("[[string:Customer Name]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.String, f.Type);
        Assert.Equal("Customer Name", f.Label);
        Assert.False(f.IsNumeric);
        Assert.False(f.IsMultiple);
        Assert.Equal(string.Empty, f.Text);
    }

    [Fact]
    public void String_With_Text_IsError()
    {
        var f = Create("[[string:Customer Name:Default Text]]");

        AssertHasErrors(f);
    }



    [Fact]
    public void String_With_Invalid_Datatype_Has_Error()
    {
        var f = Create("[[string:date:Customer Name]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void String_Missing_Label_Has_Error()
    {
        var f = Create("[[string:]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void String_With_Datatype_But_Missing_Label_Has_Error()
    {
        var f = Create("[[string:string:]]");

        AssertHasErrors(f);
    }

    // ------------------------------------------------------------
    // integer
    // ------------------------------------------------------------
    [Fact]
    public void Integer_Label_Parses()
    {
        var f = Create("[[integer:Customer Id]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Integer, f.Type);
        Assert.Equal("Customer Id", f.Label);
        Assert.True(f.IsNumeric);
        Assert.Equal(string.Empty, f.Text);
    }

    [Fact]
    public void Int_Alias_Label_Parses()
    {
        var f = Create("[[int:Customer Id]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Integer, f.Type);
        Assert.Equal("Customer Id", f.Label);
    }

    [Fact]
    public void Integer_With_More_Parts()
    {
        var f = Create("[[integer:string:Customer Id]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void Integer_With_Int_Datatype_Parses_As_Numeric()
    {
        var f = Create("[[integer:Customer Id]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Integer, f.Type);
        Assert.True(f.IsNumeric);
        Assert.Equal("Customer Id", f.Label);
    }

    [Fact]
    public void Integer_With_Text_IsError()
    {
        var f = Create("[[int:Customer Id:42]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void Integer_Invalid_Datatype_Has_Error()
    {
        var f = Create("[[int:custom:Customer Id]]");

        AssertHasErrors(f);
    }

    // ------------------------------------------------------------
    // decimal
    // ------------------------------------------------------------
    [Fact]
    public void Decimal_Label_Parses()
    {
        var f = Create("[[decimal:Amount]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Decimal, f.Type);
        Assert.Equal("Amount", f.Label);
    }

    [Fact]
    public void Dec_Alias_Label_Parses()
    {
        var f = Create("[[dec:Amount]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Decimal, f.Type);
        Assert.Equal("Amount", f.Label);
    }

    [Fact]
    public void Decimal_Datatype_Parses_As_Numeric()
    {
        var f = Create("[[decimal:Amount]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Decimal, f.Type);
        Assert.True(f.IsNumeric);
        Assert.Equal("Amount", f.Label);
    }

    [Fact]
    public void Decimal_With_Text_IsError()
    {
        var f = Create("[[dec:Amount:10.5]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void Decimal_Invalid_Datatype_Has_Error()
    {
        var f = Create("[[dec:foo:Amount]]");

        AssertHasErrors(f);
    }

    // ------------------------------------------------------------
    // date
    // ------------------------------------------------------------
    [Fact]
    public void Date_With_Label_But_Not_Custom()
    {
        var f = Create("[[date:Order Date]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void Date_With_Custom_And_Label_Parses()
    {
        var f = Create("[[date:Custom:Order Date]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Date, f.Type);
        Assert.Equal(DateRange.Custom, f.DateRange);
        Assert.Equal("Order Date", f.Label);
    }

    [Fact]
    public void Date_With_Today_Parses_Without_Label()
    {
        var f = Create("[[date:Today]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Date, f.Type);
        Assert.Equal(DateRange.Today, f.DateRange);
        Assert.Equal(string.Empty, f.Label);
    }

    [Fact]
    public void Date_With_Yesterday_Parses_Without_Label()
    {
        var f = Create("[[date:Yesterday]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Date, f.Type);
        Assert.Equal(DateRange.Yesterday, f.DateRange);
        Assert.Equal(string.Empty, f.Label);
    }

    [Fact]
    public void Date_With_LastMonth_Parses_Without_Label()
    {
        var f = Create("[[date:LastMonth]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Date, f.Type);
        Assert.Equal(DateRange.LastMonth, f.DateRange);
        Assert.Equal(string.Empty, f.Label);
    }

    [Fact]
    public void Date_With_LastTwoMonths_Parses_Without_Label()
    {
        var f = Create("[[date:LastTwoMonths]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Date, f.Type);
        Assert.Equal(DateRange.LastTwoMonths, f.DateRange);
        Assert.Equal(string.Empty, f.Label);
    }

    [Fact]
    public void Date_With_NextMonth_Parses_Without_Label()
    {
        var f = Create("[[date:NextMonth]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Date, f.Type);
        Assert.Equal(DateRange.NextMonth, f.DateRange);
        Assert.Equal(string.Empty, f.Label);
    }

    [Fact]
    public void Date_With_Range_And_More_Parts()
    {
        var f = Create("[[date:LastMonth:Sales Date]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void Date_With_Custom_But_No_Label_Has_Error()
    {
        var f = Create("[[date:Custom]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void Date_With_Invalid_Range_Has_Error()
    {
        var f = Create("[[date:LastCentury]]");

        AssertHasErrors(f);
        //Assert.Equal(SqlFilterType.Date, f.Type);
        //Assert.Equal(DateRange.Custom, f.DateRange);
        //Assert.Equal("LastCentury", f.Label);
    }

    [Fact]
    public void Date_With_Invalid_Range_And_Label_Has_Error()
    {
        var f = Create("[[date:LastCentury:Order Date]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void Date_With_Custom_And_Missing_Label_Has_Error()
    {
        var f = Create("[[date:Custom:]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void Date_With_Trailing_Text_Parses()
    {
        var f = Create("[[date:Custom:Order Date:2025-01-15]]");

        AssertHasErrors(f);
 
    }

    // ------------------------------------------------------------
    // lookup
    // ------------------------------------------------------------
    [Fact]
    public void Lookup_Without_Datatype_Has_Error()
    {
        var f = Create("[[lookup:Customer]]");

        AssertHasErrors(f);
        Assert.Equal(SqlFilterType.Lookup, f.Type);
    }

    [Fact]
    public void Lookup_With_String_Datatype_Parses()
    {
        var f = Create("[[lookup:string:Customer:select Id from anywhere]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Lookup, f.Type);
        Assert.Equal("Customer", f.Label);
        Assert.False(f.IsNumeric);
        Assert.False(f.IsMultiple);
        Assert.NotEmpty(f.Text);
    }

    [Fact]
    public void Lookup_With_Int_Datatype_Parses_As_Numeric()
    {
        var f = Create("[[lookup:int:Customer Id:select Id from anywhere]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Lookup, f.Type);
        Assert.Equal("Customer Id", f.Label);
        Assert.True(f.IsNumeric);
        Assert.False(f.IsMultiple);
        Assert.NotEmpty(f.Text);
    }

    [Fact]
    public void Lookup_With_Decimal_Datatype_Parses_As_Numeric()
    {
        var f = Create("[[lookup:decimal:Amount Range:select Amount from anywhere]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Lookup, f.Type);
        Assert.Equal("Amount Range", f.Label);
        Assert.True(f.IsNumeric);
        Assert.False(f.IsMultiple);
        Assert.NotEmpty(f.Text);
    }

 

    [Fact]
    public void Lookup_With_Datatype_And_Multi_Parses()
    {
        var f = Create("[[lookup:int:multi:Customer:select Id from Country]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Lookup, f.Type);
        Assert.Equal("Customer", f.Label);
        Assert.True(f.IsMultiple);
        Assert.True(f.IsNumeric);
        Assert.NotEmpty(f.Text);
    }

    [Fact]
    public void Lookup_With_Datatype_Multi_And_Text_Parses()
    {
        var f = Create("[[lookup:int:multi:Customer:SELECT Id, Name FROM Customer]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Lookup, f.Type);
        Assert.Equal("Customer", f.Label);
        Assert.True(f.IsMultiple);
        Assert.True(f.IsNumeric);
        Assert.Equal("SELECT Id, Name FROM Customer", f.Text);
    }

 

    [Fact]
    public void Lookup_Missing_Label_Has_Error()
    {
        var f = Create("[[lookup:multi]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void Lookup_Empty_Label_Has_Error()
    {
        var f = Create("[[lookup:string:]]");

        AssertHasErrors(f);
    }

    // ------------------------------------------------------------
    // enum
    // ------------------------------------------------------------
    [Fact]
    public void Enum_With_String_Datatype_Parses()
    {
        var f = Create("[[enum:string:Status:A;B;C]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Enum, f.Type);
        Assert.Equal("Status", f.Label);
        Assert.False(f.IsNumeric);
        Assert.False(f.IsMultiple);
        Assert.NotEmpty(f.Text);
    }

    [Fact]
    public void Enum_With_Int_Datatype_Parses_As_Numeric()
    {
        var f = Create("[[enum:int:Status:A;B;C]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Enum, f.Type);
        Assert.Equal("Status", f.Label);
        Assert.True(f.IsNumeric);
        Assert.False(f.IsMultiple);
    }

    [Fact]
    public void Enum_With_Decimal_Datatype_Parses_As_Numeric()
    {
        var f = Create("[[enum:decimal:Rate:A;B;C]]");
 
        
        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Enum, f.Type);
        Assert.Equal("Rate", f.Label);
        Assert.True(f.IsNumeric);
        Assert.False(f.IsMultiple);
    }

    [Fact]
    public void Enum_With_Text_Parses()
    {
        var f = Create("[[enum:string:Status:Open;Closed;Pending]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Enum, f.Type);
        Assert.Equal("Status", f.Label);
        Assert.False(f.IsNumeric);
        Assert.False(f.IsMultiple);
        Assert.Equal("Open;Closed;Pending", f.Text);
    }

    [Fact]
    public void Enum_With_Multi_Parses()
    {
        var f = Create("[[enum:string:multi:Status:Open;Closed;Pending]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Enum, f.Type);
        Assert.Equal("Status", f.Label);
        Assert.False(f.IsNumeric);
        Assert.True(f.IsMultiple);
        Assert.Equal("Open;Closed;Pending", f.Text);
    }
 
    [Fact]
    public void Enum_Requires_Datatype()
    {
        var f = Create("[[enum:Status]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void Enum_Invalid_Datatype_Has_Error()
    {
        var f = Create("[[enum:date:Status]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void Enum_Missing_Label_Has_Error()
    {
        var f = Create("[[enum:string:]]");

        AssertHasErrors(f);
    }

    [Fact]
    public void Enum_Multi_Missing_Label_Has_Error()
    {
        var f = Create("[[enum:string:multi:]]");

        AssertHasErrors(f);
    }

    // ------------------------------------------------------------
    // defaults / reparsing
    // ------------------------------------------------------------
    [Fact]
    public void Reparse_Resets_Previous_State()
    {
        var f = Create("[[lookup:int:multi:Customer:SELECT Id, Name FROM Customer]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Lookup, f.Type);
        Assert.True(f.IsNumeric);
        Assert.True(f.IsMultiple);
        Assert.Equal("Customer", f.Label);
        Assert.Equal("SELECT Id, Name FROM Customer", f.Text);

        f.RawTag = "[[date:Today]]";

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Date, f.Type);
        Assert.False(f.IsNumeric);
        Assert.False(f.IsMultiple);
        Assert.Equal(DateRange.Today, f.DateRange);
        Assert.Equal(string.Empty, f.Label);
        Assert.Equal(string.Empty, f.Text);
    }

    [Fact]
    public void Setting_Same_RawTag_Does_Not_Break_State()
    {
        var f = Create("[[string:Name]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.String, f.Type);
        Assert.Equal("Name", f.Label);

        f.RawTag = "[[string:Name]]";

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.String, f.Type);
        Assert.Equal("Name", f.Label);
    }

 

    // ------------------------------------------------------------
    // extra edge cases
    // ------------------------------------------------------------
    [Fact]
    public void Label_Can_Contain_Spaces_And_Non_English_Characters()
    {
        var f = Create("[[string:Ημερομηνία Παραγγελίας]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.String, f.Type);
        Assert.Equal("Ημερομηνία Παραγγελίας", f.Label);
    }

    [Fact]
    public void Text_Can_Contain_SQL_With_Colons()
    {
        var f = Create("[[lookup:string:Customer:SELECT Id, Name FROM Customer WHERE Code = 'A:B']]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Lookup, f.Type);
        Assert.Equal("Customer", f.Label);
        Assert.Equal("SELECT Id, Name FROM Customer WHERE Code = 'A:B'", f.Text);
    }

    [Fact]
    public void DateRange_Is_Case_Insensitive()
    {
        var f = Create("[[date:lastmonth]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Date, f.Type);
        Assert.Equal(DateRange.LastMonth, f.DateRange);
    }

    [Fact]
    public void Type_And_Modifiers_Are_Case_Insensitive()
    {
        var f = Create("[[LoOkUp:InT:MuLtI:Customer:SELECT 1]]");

        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Lookup, f.Type);
        Assert.True(f.IsNumeric);
        Assert.True(f.IsMultiple);
        Assert.Equal("Customer", f.Label);
        Assert.Equal("SELECT 1", f.Text);
    }

    [Fact]
    public void Enum_Text_Can_Be_Empty()
    {
        var f = Create("[[enum:string:Status:]]");
        f.Statement = "A;B;C";
 
        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Enum, f.Type);
        Assert.Equal("Status", f.Label);
        Assert.Equal(string.Empty, f.Text);
    }

    [Fact]
    public void Lookup_Text_Can_Be_Empty_When_Expression_Is_Given()
    {
        var f = Create("[[lookup:string:Customer:]]");
        f.Statement = "select Id from Customer";
        
        AssertNoErrors(f);
        Assert.Equal(SqlFilterType.Lookup, f.Type);
        Assert.Equal("Customer", f.Label);
        Assert.Equal(string.Empty, f.Text);
    }
}