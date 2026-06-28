/*
 * Tripous.Avalon JavaScript Demo Data
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

/**
 * Provides sample data helpers for Tripous Web demo pages.
 * This namespace is demo-only and is not part of the Tripous runtime API.
 * @type {object}
 */
var DemoData = DemoData || {};

// ● source lists
/**
 * Sample first names.
 * @type {string[]}
 */
DemoData.FirstNames = ["Julian", "Nigel", "Sawyer", "Cullen", "Brennan", "Boris", "Guy", "Kato", "Bevis", "Cain", "Lester", "Kevin", "Herman", "Isaac", "Jerome", "Benjamin", "Phelan", "Calvin", "Yuli", "Amery", "Michael", "Demetrius", "Ethan", "Jacob", "Castor", "Peter", "Richard", "Brody", "Ray", "Todd", "Thaddeus", "Arden", "Hamish", "Hayes", "Davis", "Steven", "Addison", "Gannon", "Lev", "Burton", "Ross", "Macon", "Rooney", "Chester", "Eric", "Wesley", "David", "Octavius", "Keaton", "Maxwell", "Caleb", "Geoffrey", "Lars", "Clayton", "Nasim", "Channing", "Alexander", "Hakeem", "Magee", "Valentine", "Zane", "Asher", "Ali", "Hu", "Justin", "Raphael", "Clark", "Quinn", "Slade", "Deacon", "Abbot", "George", "Seth", "Ulric", "Noah", "Jesse", "Tobias", "Vaughan", "Wayne", "Gabriel", "Roth", "Murphy", "Joshua", "Marvin", "Cameron", "Uriah", "Axel", "Joseph", "Zachary", "Alvin", "Warren", "Erasmus", "Blaze", "Nathan", "Armand"];
/**
 * Sample last names.
 * @type {string[]}
 */
DemoData.LastNames = ["Bonner", "Fields", "Norman", "Jacobson", "Weiss", "Forbes", "Garrett", "Martin", "Turner", "Parsons", "Campos", "Golden", "Allison", "Lindsey", "Hines", "Weber", "Douglas", "Hendrix", "Le", "Deleon", "Lyons", "Mann", "Goff", "Mccarty", "Silva", "Rogers", "Horn", "Crane", "Mays", "Bennett", "Savage", "Bauer", "Contreras", "Knox", "Schultz", "Rodriquez", "Sharpe", "Haney", "Good", "Harrell", "Terrell", "Vincent", "Malone", "Ewing", "Bryan", "Jenkins", "Lindsay", "Gates", "Stanton", "Guerrero", "Hubbard", "Berg", "Torres", "Hooper", "Ochoa", "Smith", "Gardner", "Lane", "Robertson", "Bowman", "Gilmore", "Lamb", "Odom", "Young", "Riddle", "Herring", "Peters", "Jarvis", "Bates", "Quinn", "Blackwell", "Chavez", "Kinney", "Fitzgerald", "Delaney", "Mckenzie", "Andrews", "Foster", "Richardson", "Copeland", "Chang", "Mayer", "Kerr", "Strickland", "House", "Castillo", "Talley", "Ortega", "Morin", "Neal", "Duncan", "Morales"];
/**
 * Sample country names.
 * @type {string[]}
 */
DemoData.Countries = ["Greece", "Niger", "Isle of Man", "British Indian Ocean Territory", "Chad", "Cambodia", "Chile", "Uruguay", "Yemen", "Anguilla", "Macao", "Mozambique", "Papua New Guinea", "Mauritania", "Albania", "Belgium", "Northern Mariana Islands", "Saint Lucia", "Slovenia", "Maldives", "Ecuador", "Botswana", "Morocco", "Iran", "Solomon Islands", "Mongolia", "Luxembourg", "Martinique", "Saint Pierre and Miquelon", "Bosnia and Herzegovina", "United States", "French Polynesia", "Gambia", "Guernsey", "Cyprus", "Mauritius", "Sandwich Islands", "Venezuela", "Bermuda", "Moldova", "Cuba", "American Samoa", "Lesotho", "Micronesia", "New Caledonia", "Nepal", "Cape Verde", "Philippines", "Haiti", "Uganda", "Kenya", "Portugal", "Malawi", "Mauritania", "Vanuatu", "Guam", "Benin", "Armenia", "Vatican", "Latvia", "Tonga", "Congo", "Kazakhstan", "Ghana", "Thailand", "Kyrgyzstan", "Cameroon", "United Kingdom", "Gibraltar", "Senegal", "Antarctica", "Trinidad and Tobago", "Honduras", "Cayman Islands", "Italy", "Bangladesh", "Madagascar", "United Arab Emirates", "Monaco", "Spain", "Romania", "Bonaire", "France", "Ivory Coast", "Burundi", "Saint Barthelemy"];
/**
 * Sample department names.
 * @type {string[]}
 */
DemoData.Departments = ["RnD", "I.T.", "Marketing", "Sales", "Finance"];
/**
 * Sample boolean values.
 * @type {boolean[]}
 */
DemoData.Booleans = [true, false, false, false, false, true, true, false, false, true, false, false, true, false, false, false, true, false, true, true, false, false, true, true, false, true, true, true, false, true];
/**
 * Sample numeric values.
 * @type {number[]}
 */
DemoData.Numbers = [1877, 1375, 2653, 1090, 1550, 1963, 1305, 1021, 2020, 2867, 1034, 915, 857, 2001, 2965, 1737, 2802, 2099, 2671, 2559, 2515, 2803, 1895, 1854, 2852, 1421, 1734, 1140, 2840, 1802, 2285, 2517, 1263, 2285, 1249, 1430, 2651, 2705, 1820, 1385, 1404, 1035, 2901, 2745, 1612, 2377, 2506, 1860, 1805, 1678, 2235, 1803, 1813, 1291, 1498, 1184, 960, 2874, 1541, 1631, 1621, 1314, 2646, 1041, 2674, 2408, 2421, 1793, 1683, 1677, 2633, 2186, 1715, 1641, 1491, 1392, 2615, 1215, 1414, 840, 822, 1965, 2140, 2540, 1387, 1934, 2993, 2872, 2873, 1393, 1247, 2160, 1041, 1078, 1196, 1458, 2595, 1371, 917, 2703];

// ● private
/**
 * Returns a random item from a list.
 * @param {Array} List The source list.
 * @returns {*} Returns a random item.
 */
DemoData.RandomItem = function (List) {
    return List[Math.floor(Math.random() * List.length)];
};
/**
 * Adds days to a date.
 * @param {Date} DateValue The source date.
 * @param {number} Days The days to add.
 * @returns {Date} Returns the new date.
 */
DemoData.AddDays = function (DateValue, Days) {
    var Result = new Date(DateValue.getTime());
    Result.setDate(Result.getDate() + Days);
    return Result;
};
/**
 * Gets sample date values.
 * @returns {Date[]} Returns date values.
 */
DemoData.GetDates = function () {
    var Index;
    var BaseDate;
    if (!DemoData.fDates) {
        DemoData.fDates = [];
        BaseDate = new Date();
        for (Index = 0; Index < 100; Index++)
            DemoData.fDates.push(DemoData.AddDays(BaseDate, Index + 1));
    }
    return DemoData.fDates;
};

// ● public
/**
 * Creates a lookup table with Id and Name columns.
 * @param {string} TableName The table name, Countries or Departments.
 * @returns {tp.DataTable} Returns the lookup table.
 */
DemoData.CreateLookupTable = function (TableName) {
    var List = tp.IsSameText(TableName, "Countries") ? DemoData.Countries : DemoData.Departments;
    var Table = new tp.DataTable(TableName);
    var Index;
    Table.AutoGenerateGuidKeys = false;
    Table.AddColumn("Id", tp.DataType.Integer);
    Table.AddColumn("Name", tp.DataType.String);
    for (Index = 0; Index < List.length; Index++)
        Table.AddRow(Index + 1, List[Index]).AcceptChanges();
    return Table;
};
/**
 * Creates a lookup list with Id and Name properties.
 * @param {string} TableName The table name, Countries or Departments.
 * @returns {object[]} Returns lookup objects.
 */
DemoData.CreateLookupList = function (TableName) {
    var Source = tp.IsSameText(TableName, "Countries") ? DemoData.Countries : DemoData.Departments;
    var Result = [];
    var Index;
    for (Index = 0; Index < Source.length; Index++)
        Result.push({ Id: Index + 1, Name: Source[Index] });
    return Result;
};
/**
 * Gets the countries table.
 * @returns {tp.DataTable} Returns the countries table.
 */
DemoData.GetCountriesTable = function () {
    if (!DemoData.fCountriesTable)
        DemoData.fCountriesTable = DemoData.CreateLookupTable("Countries");
    return DemoData.fCountriesTable;
};
/**
 * Gets the departments table.
 * @returns {tp.DataTable} Returns the departments table.
 */
DemoData.GetDepartmentsTable = function () {
    if (!DemoData.fDepartmentsTable)
        DemoData.fDepartmentsTable = DemoData.CreateLookupTable("Departments");
    return DemoData.fDepartmentsTable;
};
/**
 * Creates an employee sample data table.
 * @param {number|null|undefined} RowCount The number of rows to create.
 * @returns {{Table: tp.DataTable, MSecs: number}} Returns the table and elapsed milliseconds.
 */
DemoData.CreateEmployeeTable = function (RowCount) {
    var StartTime = Date.now();
    var Table = new tp.DataTable("Employees");
    var Rows = [];
    var Dates = DemoData.GetDates();
    var Index;
    var Id;
    var Row;
    RowCount = tp.IsNumber(RowCount) && RowCount > 0 ? RowCount : 100;
    Table.AutoGenerateGuidKeys = false;
    Table.AddColumn("Id", tp.DataType.Integer);
    Table.AddColumn("Code", tp.DataType.String);
    Table.AddColumn("Name", tp.DataType.String);
    Table.AddColumn("Age", tp.DataType.Integer);
    Table.AddColumn("DepartmentId", tp.DataType.Integer);
    Table.AddColumn("Salary", tp.DataType.Double);
    Table.AddColumn("Married", tp.DataType.Boolean);
    Table.AddColumn("CountryId", tp.DataType.Integer);
    Table.AddColumn("EntryDate", tp.DataType.Date);
    Rows.length = RowCount;
    for (Index = 0; Index < RowCount; Index++) {
        Id = Index + 1;
        Row = new tp.DataRow(Table, [
            Id,
            tp.PadLeft(Id.toString(), "0", 6),
            DemoData.RandomItem(DemoData.FirstNames) + " " + DemoData.RandomItem(DemoData.LastNames),
            Math.floor(Math.random() * 20) + 25,
            Math.floor(Math.random() * DemoData.Departments.length) + 1,
            DemoData.RandomItem(DemoData.Numbers) * Math.random(),
            DemoData.RandomItem(DemoData.Booleans),
            Math.floor(Math.random() * DemoData.Countries.length) + 1,
            DemoData.RandomItem(Dates)
        ]);
        Row.State = tp.DataRowState.Unchanged;
        Rows[Index] = Row;
    }
    Table.Rows = Rows;
    return {
        Table: Table,
        MSecs: Date.now() - StartTime
    };
};

// ● fields
/**
 * Cached date values.
 * @type {Date[]|null}
 */
DemoData.fDates = null;
/**
 * Cached countries table.
 * @type {tp.DataTable|null}
 */
DemoData.fCountriesTable = null;
/**
 * Cached departments table.
 * @type {tp.DataTable|null}
 */
DemoData.fDepartmentsTable = null;
