DataWindow: Window

DataForm: UserControl
    ToolBar (btnHome (with dropdown), btnList, btnInsert, btnEdit, btnDelete, btnCancel, btnOK, btnClose)  
    pnlList
        pnlSideBar
            pnlFindToolBar
            pnlFilters
        Splitter
            gridList 
    pnlItem
      ItemPage: UserControl
        // all data entry controls