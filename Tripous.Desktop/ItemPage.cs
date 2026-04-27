namespace Tripous.Desktop;
 
public class ItemPage : UserControl
{
    private Grid CreateColumnGrid()
    {
        Grid Grid = new();

        Grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(33, GridUnitType.Star)));
        Grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(67, GridUnitType.Star)));

        return Grid;
    }
    private void AddFieldRow(Grid Grid, int RowIndex, FieldDef Field)
    {
        Grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        if (Field.Flags.HasFlag(FieldFlags.Boolean) || Field.DataType == DataFieldType.Boolean)
        {
            CheckBox Box = new()
            {
                Content = Field.Title,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 0, 0, 6)
            };

            ControlBindingHelper.Bind(DataForm, Box, Field.Name, Field);

            Grid.SetRow(Box, RowIndex);
            Grid.SetColumn(Box, 0);
            Grid.SetColumnSpan(Box, 2);

            Grid.Children.Add(Box);
            return;
        }

        TextBlock Label = new()
        {
            Text = Field.Title,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 0, 6, 6)
        };

        Control Editor;

        if (Field.HasLookup)
        {
            ComboBox Box = new();
            ControlBindingHelper.BindLookup(DataForm, Box, Field.Name, Field);
            Editor = Box;
        }
        else if (Field.IsDateTime)
        {
            DatePicker Box = new();
            ControlBindingHelper.Bind(DataForm, Box, Field.Name, Field);
            Editor = Box;
        }
        else if (Field.IsNumeric)
        {
            TextBox Box = new();
            Box.TextAlignment = TextAlignment.Right;
            ControlBindingHelper.Bind(DataForm, Box, Field);
            Editor = Box;
        }
        else
        {
            TextBox Box = new();
            ControlBindingHelper.Bind(DataForm, Box, Field);
            Editor = Box;
        }

        Editor.HorizontalAlignment = HorizontalAlignment.Stretch;
        Editor.Margin = new Thickness(0, 0, 0, 6);

        Grid.SetRow(Label, RowIndex);
        Grid.SetColumn(Label, 0);

        Grid.SetRow(Editor, RowIndex);
        Grid.SetColumn(Editor, 1);

        Grid.Children.Add(Label);
        Grid.Children.Add(Editor);
    }
    public virtual void Bind()
    {
        TableDef Table = ModuleDef.Table;
        if (Table == null)
            return;

        List<FieldDef> Fields = Table.Fields
            .Where(f => f.Flags.HasFlag(FieldFlags.Visible) && !f.Flags.HasFlag(FieldFlags.Extra) && !f.IsBlob)
            .ToList();

        int Count = Fields.Count;
        int LeftCount;
        int RightCount;

        if (Count <= 8)
        {
            LeftCount = Count;
            RightCount = 0;
        }
        else if (Count <= 16)
        {
            LeftCount = 8;
            RightCount = Count - 8;
        }
        else
        {
            LeftCount = (Count + 1) / 2;
            RightCount = Count - LeftCount;
        }

        Grid Root = new();
        Root.HorizontalAlignment = HorizontalAlignment.Left;

        Root.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(Ui.FormColumnWidth)));

        if (RightCount > 0)
            Root.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(Ui.FormColumnWidth)));

        Grid Left = CreateColumnGrid();
        Grid.SetColumn(Left, 0);
        Root.Children.Add(Left);
        Left.Margin = new Thickness(0, 12, 0, 0);

        Grid Right = null;

        if (RightCount > 0)
        {
            Right = CreateColumnGrid();
            Right.Margin = new Thickness(16, 12, 0, 0);

            Grid.SetColumn(Right, 1);
            Root.Children.Add(Right);
        }

        int LeftRow = 0;
        int RightRow = 0;

        for (int i = 0; i < Fields.Count; i++)
        {
            FieldDef Field = Fields[i];

            if (i < LeftCount)
                AddFieldRow(Left, LeftRow++, Field);
            else
                AddFieldRow(Right, RightRow++, Field);
        }

        Content = Root;
    }

    // ● properties
    /// <summary>
    /// The parent form
    /// </summary>
    public DataForm DataForm { get; internal set; }
    /// <summary>
    /// Form context
    /// </summary>
    public DataFormContext DataFormContext => DataForm.DataFormContext;
    /// <summary>
    /// The form definition.
    /// </summary>
    public FormDef FormDef => DataFormContext.FormDef;
    /// <summary>
    /// The module definition
    /// </summary>
    public ModuleDef ModuleDef => DataFormContext.ModuleDef;
    /// <summary>
    /// The data module
    /// </summary>
    public DataModule Module => DataFormContext.Module;
    /// <summary>
    /// Returns the form mode
    /// </summary>
    public virtual FormType FormType => FormDef.FormType;
    /// <summary>
    /// Form actions the form is not allowed to execute.
    /// </summary>
    public DataFormAction InvalidActions => DataFormContext.InvalidActions;
    /// <summary>
    /// The first action the form should execute after initialization.
    /// </summary>
    public DataFormAction StartAction => DataFormContext.StartAction;
}
 

 