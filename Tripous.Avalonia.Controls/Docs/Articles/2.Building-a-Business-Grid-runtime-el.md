# Building a Business Grid Runtime

## 1. Introduction

Το προηγούμενο βήμα είναι να καταλάβουμε ότι ένα business grid δεν είναι απλώς ένας πίνακας. Είναι ένα μικρό framework. Το επόμενο βήμα είναι να δούμε πώς μπορεί να στηθεί εσωτερικά αυτό το framework.

Το άρθρο αυτό δεν περιγράφει συγκεκριμένη υλοποίηση. Περιγράφει τον τρόπο σκέψης πίσω από ένα grid runtime: ποια κατάσταση κρατά, ποια υποσυστήματα συνεργάζονται, ποια pipelines χρειάζονται και πώς οι αποφάσεις σχεδιασμού κρατούν το grid σταθερό, γρήγορο και επεκτάσιμο.

Ένα business grid runtime πρέπει να απαντά συνεχώς σε ερωτήσεις όπως:

- Ποιες γραμμές υπάρχουν στην τρέχουσα προβολή;
- Ποιες από αυτές είναι ορατές στο viewport;
- Ποιο κελί είναι current;
- Υπάρχει active edit session;
- Πώς μετατρέπεται η τιμή του editor σε τιμή του data source;
- Πώς επηρεάζει ένα filter το grouping και τα summaries;
- Τι πρέπει να ξαναζωγραφιστεί μετά από μια αλλαγή;

Αυτές οι ερωτήσεις δεν απαντώνται σωστά με διάσπαρτο κώδικα. Χρειάζονται runtime.

## Part I - Runtime Core

## 2. The Runtime State

### Γιατί υπάρχει αυτή η έννοια;

Το grid δεν είναι στατική εικόνα. Είναι μια ζωντανή κατάσταση. Ο χρήστης μπορεί να μετακινείται, να επιλέγει, να κάνει edit, να αλλάζει sorting, να εφαρμόζει filters, να ανοίγει groups και να κάνει scroll. Αν κάθε υποσύστημα κρατά τη δική του ανεξάρτητη εικόνα, το grid θα αρχίσει να έχει αντιφάσεις.

Το runtime state υπάρχει για να υπάρχει μία κεντρική, συνεκτική εικόνα του grid.

Τυπικά περιλαμβάνει:

- Columns.
- View rows.
- Current cell.
- Current row.
- Selection.
- Edit session.
- Scroll state.
- Sort state.
- Filter state.
- Group state.
- Summary state.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το runtime state είναι η κοινή πηγή αλήθειας. Το renderer το διαβάζει. Το navigation το αλλάζει. Το editing το χρησιμοποιεί για να ξέρει ποιο κελί επεξεργάζεται. Το scroll manager το ενημερώνει όταν αλλάζει viewport. Το projection engine το ξαναχτίζει όταν αλλάζουν sorting, filtering ή grouping.

Ένα απλό μοντέλο μπορεί να μοιάζει έτσι:

```text
GridRuntime
    DataAdapter
    Columns
    Projection
    CurrentCell
    Selection
    EditSession
    ScrollState
```

Το σημαντικό δεν είναι η ακριβής μορφή. Το σημαντικό είναι ότι όλες οι αποφάσεις περνούν από ένα κοινό runtime model.

## 3. The Data Adapter Contract

### Γιατί υπάρχει αυτή η έννοια;

Το grid δεν πρέπει να ξέρει πώς αποθηκεύονται τα δεδομένα. Μπορεί να έρχονται από αντικείμενα, πίνακα, υπηρεσία, query ή προσωρινή μνήμη. Αν το grid δεθεί με ένα συγκεκριμένο data model, χάνει τη χρησιμότητα του.

Ο data adapter υπάρχει για να μετατρέπει οποιοδήποτε data source σε μια σταθερή σύμβαση που καταλαβαίνει το grid.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Ο adapter παρέχει βασικές λειτουργίες:

```text
RowCount
GetValue(row, column)
SetValue(row, column, value)
CanSetValue(row, column)
Insert(row)
Delete(row)
```

Το rendering ζητά τιμές. Το editing κάνει write-back. Το filtering και sorting διαβάζουν τιμές. Το grouping δημιουργεί keys. Τα summaries κάνουν aggregate πάνω σε τιμές. Όλα αυτά περνούν από τον adapter, όχι απευθείας από την πηγή δεδομένων.

Έτσι το grid μένει ανεξάρτητο από το είδος του data source.

## 4. The View Projection

### Γιατί υπάρχει αυτή η έννοια;

Τα raw data rows δεν είναι πάντα οι γραμμές που βλέπει ο χρήστης. Αν υπάρχει filter, κάποιες γραμμές αφαιρούνται. Αν υπάρχει sorting, αλλάζει η σειρά. Αν υπάρχει grouping, δημιουργούνται group rows που δεν υπάρχουν στο data source. Αν υπάρχουν summaries, εμφανίζονται επιπλέον summary rows.

Η προβολή είναι projection. Δεν είναι τα raw δεδομένα.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το projection engine παίρνει το data source και το τρέχον operation state:

```text
raw rows
    ↓ filtering
filtered rows
    ↓ sorting
sorted rows
    ↓ grouping
group tree
    ↓ flatten expanded nodes
view rows
```

Το renderer δεν ζωγραφίζει raw rows. Ζωγραφίζει view rows. Το navigation δεν κινείται σε raw rows. Κινείται σε view rows. Το virtual scrolling επίσης γίνεται πάνω στην προβολή.

Αυτός ο διαχωρισμός είναι κρίσιμος. Επιτρέπει στο grid να δείχνει πολύπλοκη οργάνωση χωρίς να αλλάζει το data source.

## 5. Row Kinds

### Γιατί υπάρχει αυτή η έννοια;

Σε ένα απλό table, κάθε row είναι data row. Σε ένα business grid αυτό δεν ισχύει. Η ορατή προβολή μπορεί να περιέχει διαφορετικά είδη γραμμών:

- Data row.
- Group header row.
- Group summary row.
- Footer summary row.
- Filter row.
- Placeholder row.

Κάθε row kind έχει διαφορετική συμπεριφορά. Δεν είναι όλα editable. Δεν συμμετέχουν όλα σε selection. Δεν έχουν όλα τα ίδια cells. Δεν ζωγραφίζονται όλα με τον ίδιο τρόπο.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το row kind επηρεάζει navigation, rendering, editing και hit testing.

Για παράδειγμα:

```text
if row.kind == DataRow
    can edit cells

if row.kind == GroupHeader
    can expand or collapse

if row.kind == Summary
    display aggregates
```

Το grid runtime πρέπει να γνωρίζει τι είδους row χειρίζεται. Αλλιώς θα προσπαθήσει να κάνει edit σε summary row ή να ζητήσει data value από group header.

## Part II - Layout And Rendering

## 6. Layout Metrics

### Γιατί υπάρχει αυτή η έννοια;

Το grid χρειάζεται σταθερή γεωμετρία. Πρέπει να ξέρει ύψη, πλάτη, offsets, περιοχές και scrollbars. Αν αυτά υπολογίζονται πρόχειρα σε κάθε σημείο, το rendering και το hit testing θα διαφωνούν.

Τα layout metrics υπάρχουν για να περιγράφουν τη φυσική μορφή του grid.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Τυπικά metrics είναι:

- Toolbar height.
- Group panel height.
- Header height.
- Filter row height.
- Row height.
- Summary band height.
- Horizontal scrollbar height.
- Vertical scrollbar width.
- Column widths.
- Viewport rectangle.

Το renderer χρησιμοποιεί τα metrics για drawing. Το hit testing τα χρησιμοποιεί για να βρει τι βρίσκεται κάτω από το pointer. Το scroll manager τα χρησιμοποιεί για να υπολογίσει πόσες γραμμές χωρούν. Το editor host τα χρησιμοποιεί για να τοποθετήσει τον editor πάνω από το κελί.

## 7. Visible Range Calculation

### Γιατί υπάρχει αυτή η έννοια;

Το grid δεν πρέπει να ζωγραφίζει όλες τις γραμμές. Πρέπει να ζωγραφίζει μόνο αυτές που είναι μέσα στο viewport. Για να το κάνει, πρέπει να υπολογίσει την ορατή περιοχή της προβολής.

Αυτό είναι η βάση του virtual scrolling.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Ο υπολογισμός είναι περίπου:

```text
firstVisibleRow = scrollOffset / rowHeight
visibleRowCount = viewportHeight / rowHeight
lastVisibleRow = firstVisibleRow + visibleRowCount
```

Στην πράξη υπάρχουν λεπτομέρειες: partial rows, variable heights, group rows, summary rows, frozen bands. Η βασική ιδέα όμως παραμένει ίδια. Το grid υπολογίζει ποιο κομμάτι της λογικής προβολής χωράει στον θάλαμο του viewport.

Το renderer ζωγραφίζει αυτό το range. Το hit testing μεταφράζει το Y coordinate σε visible row offset και μετά σε logical view row.

## 8. Cell Geometry

### Γιατί υπάρχει αυτή η έννοια;

Κάθε κελί πρέπει να έχει rectangle. Χωρίς cell geometry δεν μπορεί να γίνει drawing, hit testing, editor placement, selection painting ή drag feedback.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το cell rectangle προκύπτει από:

- Το row position.
- Το column position.
- Το horizontal scroll offset.
- Το vertical viewport offset.
- Το column width.
- Το row height.

```text
cellRect(row, column)
    x = column.left - horizontalOffset
    y = viewport.top + rowOffset * rowHeight
    width = column.width
    height = rowHeight
```

Το ίδιο rectangle πρέπει να χρησιμοποιείται από renderer, hit tester και editor host. Αν κάθε υποσύστημα το υπολογίζει διαφορετικά, εμφανίζονται μικρές αλλά ενοχλητικές ασυνέπειες.

## 9. Rendering Order

### Γιατί υπάρχει αυτή η έννοια;

Το grid έχει πολλά οπτικά layers. Αν ζωγραφιστούν με λάθος σειρά, το αποτέλεσμα θα είναι θολό ή λάθος. Για παράδειγμα, το selection πρέπει να βρίσκεται πίσω από το κείμενο αλλά πάνω από το background. Το current cell border πρέπει να φαίνεται πάνω από το cell. Ο active editor πρέπει να βρίσκεται πάνω από το painted cell.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Μια τυπική σειρά rendering είναι:

```text
draw background
draw fixed bands
draw headers
draw filter row
draw body rows
draw group rows
draw summary rows
draw selection
draw current cell
draw scrollbars
draw drag indicators
arrange active editor overlay
```

Το rendering order είναι το σημείο όπου το runtime state γίνεται εικόνα. Δεν πρέπει να αλλάζει state. Πρέπει να αποτυπώνει state.

## 10. Hit Testing

### Γιατί υπάρχει αυτή η έννοια;

Ο χρήστης αλληλεπιδρά με σημεία στην οθόνη. Το grid πρέπει να μεταφράζει αυτά τα σημεία σε λογικές έννοιες. Αυτό κάνει το hit testing.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Ένα hit result μπορεί να περιέχει:

- Band.
- Row kind.
- View row index.
- Data row index.
- Column.
- Cell rectangle.
- Hit kind.

Παράδειγμα:

```text
HitResult
    kind = Cell
    rowKind = DataRow
    viewRow = 42
    dataRow = 128
    column = Amount
```

Το navigation χρησιμοποιεί το hit result για current cell. Το resize χρησιμοποιεί hit result για column edge. Το grouping χρησιμοποιεί hit result για drop target. Το context menu χρησιμοποιεί hit result για να ξέρει ποια στήλη ή summary αφορά.

## Part III - Interaction

## 11. Navigation Engine

### Γιατί υπάρχει αυτή η έννοια;

Η πλοήγηση πρέπει να είναι συνεπής. Το δεξί βελάκι, το Tab, το Enter και το mouse click πρέπει να καταλήγουν σε προβλέψιμες αλλαγές current cell. Αν κάθε input handler μετακινεί μόνος του την κατάσταση, το grid θα έχει διαφορετικούς κανόνες ανά input.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το navigation engine ξέρει:

- Ποιες στήλες είναι ορατές.
- Ποιες γραμμές είναι data rows.
- Ποια κελιά είναι editable.
- Πώς να μετακινηθεί στο επόμενο ή προηγούμενο κελί.
- Πότε να κάνει scroll για να φανεί το current cell.

```text
MoveNextCell()
    find next visible column
    skip non-data rows when needed
    update current cell
    scroll current cell into view
```

Το navigation συνεργάζεται στενά με selection, editing και scrolling.

## 12. Selection Model

### Γιατί υπάρχει αυτή η έννοια;

Το current cell δεν είναι πάντα selection. Το selected row δεν είναι πάντα current row. Η multi-selection δεν είναι απλώς πολλά current rows. Αυτές οι έννοιες χρειάζονται ξεχωριστό μοντέλο.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το selection model ενημερώνεται από mouse, keyboard και commands. Το renderer το εμφανίζει. Τα commands το χρησιμοποιούν για να αποφασίσουν σε ποιες γραμμές θα εφαρμοστούν.

```text
CurrentCell -> where keyboard action applies
SelectedRows -> where row command applies
SelectedBlock -> where copy/paste applies
```

Η καθαρή διάκριση αποφεύγει πολλά λάθη σε business workflows.

## 13. Scroll Engine

### Γιατί υπάρχει αυτή η έννοια;

Το scroll engine δεν είναι απλώς scrollbar. Είναι ο μηχανισμός που μετακινεί το viewport πάνω στη λογική προβολή. Είναι το ασανσέρ του grid.

Πάνω από το viewport υπάρχουν λογικές γραμμές που δεν ζωγραφίζονται. Κάτω από το viewport υπάρχουν επίσης λογικές γραμμές που δεν ζωγραφίζονται. Το grid πρέπει να δίνει την αίσθηση ότι ο χρήστης κινείται μέσα σε όλη τη λίστα, ενώ στην πραγματικότητα rendering γίνεται μόνο στο ορατό τμήμα.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το scroll engine συνεργάζεται με:

- Το projection, για το συνολικό row count.
- Τα layout metrics, για το viewport height.
- Το renderer, για visible range.
- Το navigation, για scroll-to-current-cell.
- Το hit testing, για mapping από visible row σε logical row.

```text
scrollOffset changes
    ↓
visible range changes
    ↓
renderer redraws visible rows
```

## 14. Drag Operations

### Γιατί υπάρχει αυτή η έννοια;

Τα business grids συχνά επιτρέπουν αλλαγή δομής με drag: resize columns, reorder columns, group columns, remove columns, move grouped fields. Αυτές οι κινήσεις είναι transient. Έχουν αρχή, preview και τέλος.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Ένα drag operation χρειάζεται:

- Start point.
- Current pointer position.
- Drag kind.
- Valid and invalid targets.
- Visual feedback.
- Commit action on release.

```text
BeginDrag(column)
    track pointer
    update drop target
    render ghost
    on release apply operation
```

Το drag συνεργάζεται με hit testing, rendering, column model και grouping.

## Part IV - Editing Runtime

## 15. Edit Session Lifecycle

### Γιατί υπάρχει αυτή η έννοια;

Το edit session είναι η πιο σημαντική εσωτερική διαδικασία του grid. Δεν αρκεί να εμφανιστεί ένας editor. Το grid πρέπει να ξέρει ότι βρίσκεται σε editing mode και ποιο κελί αφορά.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Η ροή είναι:

```text
BeginEdit(cell)
    check editable
    store editing cell
    create editor
    load current value
    position editor
    focus editor

CommitEdit()
    read editor value
    normalize
    parse
    validate
    write to data source
    close editor

CancelEdit()
    discard editor value
    close editor
    restore grid focus
```

Το editing lifecycle συνεργάζεται με current cell, column, data adapter, validation, renderer και navigation.

## 16. Editor Factory

### Γιατί υπάρχει αυτή η έννοια;

Δεν υπάρχει ένας editor για όλα τα κελιά. Το grid χρειάζεται μηχανισμό επιλογής editor ανά στήλη, τύπο δεδομένων ή κανόνα εφαρμογής.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Η στήλη είναι το φυσικό σημείο απόφασης. Μια number column δίνει number editor. Μια date column δίνει date editor. Μια lookup column δίνει lookup editor. Η εφαρμογή μπορεί να παρέμβει και να δώσει custom editor.

```text
CreateEditor(column)
    if application provides custom editor
        return custom editor
    if column is Number
        return NumberEditor
    if column is Date
        return DateEditor
    if column is Lookup
        return LookupEditor
    return TextEditor
```

Το editor factory κρατά το grid επεκτάσιμο χωρίς να γεμίζει ο editing engine με ειδικές περιπτώσεις.

## 17. Typed Editors

### Γιατί υπάρχει αυτή η έννοια;

Κάθε τύπος δεδομένων έχει διαφορετική εμπειρία επεξεργασίας. Το text θέλει ελεύθερη πληκτρολόγηση. Ο αριθμός θέλει right alignment και numeric validation. Η ημερομηνία θέλει normalization και ίσως calendar. Το lookup θέλει επιλογή από λίστα.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Όλοι οι typed editors κληρονομούν από κοινό base editor ή υλοποιούν κοινή σύμβαση. Έτσι το grid τους χειρίζεται ομοιόμορφα:

```text
editor.Value
editor.Focus()
editor.CommitRequested
editor.CancelRequested
editor.DropDownRequested
editor.Cleanup()
```

Ο editor μπορεί εσωτερικά να είναι απλός ή σύνθετος. Το grid ενδιαφέρεται για τη σύμβαση, όχι για τις εσωτερικές λεπτομέρειες.

## 18. Value Pipeline

### Γιατί υπάρχει αυτή η έννοια;

Η τιμή που επιστρέφει ο editor δεν είναι πάντα έτοιμη για το data source. Μπορεί να είναι text που πρέπει να γίνει αριθμός. Μπορεί να είναι partial date input. Μπορεί να είναι selected lookup item που πρέπει να γίνει foreign key.

Το value pipeline υπάρχει για να μετατρέψει την editor value σε τελική data value.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Η ροή είναι:

```text
editor value
    ↓ normalize
normalized value
    ↓ parse by column
typed value
    ↓ validate
accepted value
    ↓ write to data source
```

Το normalization μπορεί να είναι γενικό ή custom. Το parsing ανήκει συχνά στη στήλη. Το validation μπορεί να ανήκει στην εφαρμογή ή στο business layer. Το grid συντονίζει τη διαδικασία.

## 19. Drop-down Editors

### Γιατί υπάρχει αυτή η έννοια;

Μερικοί editors δεν μπορούν να εκφραστούν καλά με απλό text input. Η ημερομηνία μπορεί να θέλει calendar. Το lookup μπορεί να θέλει λίστα επιλογών. Ένα σύνθετο πεδίο μπορεί να θέλει custom picker.

Το drop-down υπάρχει για να δώσει πρόσθετη επιφάνεια επιλογής χωρίς να αλλάξει το κελί σε μόνιμο control.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το drop-down συνεργάζεται με το edit session:

- Ανοίγει από editor command.
- Τοποθετείται σε σχέση με το edited cell.
- Μπορεί να πάρει focus.
- Επιστρέφει τιμή στον editor.
- Κλείνει με commit ή cancel.

```text
Editor requests drop-down
    ↓
Grid creates drop-down host
    ↓
User selects value
    ↓
Editor receives value
    ↓
Commit pipeline continues
```

Το drop-down δεν είναι ανεξάρτητο παράθυρο επιχειρησιακής λογικής. Είναι μέρος του editing pipeline.

## Part V - Data Operations

## 20. Sorting Pipeline

### Γιατί υπάρχει αυτή η έννοια;

Το sorting αλλάζει τη σειρά της προβολής. Πρέπει να γίνει με βάση τις πραγματικές τιμές και τους τύπους στηλών, όχι μόνο με βάση το εμφανιζόμενο κείμενο.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το sorting pipeline χρησιμοποιεί sort descriptors:

```text
SortDescriptor
    column
    direction
    comparer
```

Το projection engine εφαρμόζει τους descriptors πριν από το grouping ή μέσα στα groups, ανάλογα με την πολιτική. Το renderer εμφανίζει sort indicators. Το navigation συνεχίζει να δουλεύει πάνω στη νέα σειρά.

## 21. Filtering Pipeline

### Γιατί υπάρχει αυτή η έννοια;

Το filtering μετατρέπει user input σε κανόνα επιλογής γραμμών. Το text που γράφει ο χρήστης δεν είναι ακόμη predicate. Πρέπει να γίνει filter expression.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Η ροή είναι:

```text
filter text
    ↓ parse
filter expression
    ↓ compile/evaluate
predicate
    ↓ apply to rows
filtered projection
```

Το filtering επηρεάζει row count, grouping, summaries, selection και navigation. Γι’ αυτό πρέπει να ανήκει στο projection pipeline.

## 22. Grouping Pipeline

### Γιατί υπάρχει αυτή η έννοια;

Το grouping δημιουργεί ιεραρχική προβολή από επίπεδα rows. Δεν είναι απλή εμφάνιση. Είναι νέα δομή.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Η ροή είναι:

```text
filtered and sorted rows
    ↓ group by first descriptor
groups
    ↓ group by next descriptor
nested groups
    ↓ flatten expanded nodes
view rows
```

Το grouping παράγει group headers, group summaries και data rows. Το renderer πρέπει να ξέρει το row kind. Το navigation πρέπει να ξέρει ποια rows είναι editable. Το summary engine υπολογίζει aggregates ανά group.

## 23. Summary Pipeline

### Γιατί υπάρχει αυτή η έννοια;

Τα summaries απαντούν σε επιχειρησιακές ερωτήσεις. Πόσα; Πόσο σύνολο; Ποιο minimum; Ποιο maximum; Ποιος μέσος όρος;

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το summary pipeline χρησιμοποιεί aggregate descriptors:

```text
SummaryDescriptor
    column
    aggregateKind
    scope
```

Το scope μπορεί να είναι group ή total. Το aggregate πρέπει να είναι έγκυρο για τον τύπο της στήλης. Το αποτέλεσμα εμφανίζεται από το renderer αλλά υπολογίζεται από το runtime πάνω στην τρέχουσα προβολή.

## Part VI - Extensibility And API

## 24. Public API Surface

### Γιατί υπάρχει αυτή η έννοια;

Ένα grid runtime έχει πολλές εσωτερικές λεπτομέρειες. Δεν πρέπει όλες να γίνουν public API. Ένα μεγάλο και ακαθόριστο API κάνει το grid δύσκολο στη χρήση και δύσκολο στην εξέλιξη.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το public API πρέπει να εκθέτει έννοιες που έχουν νόημα για την εφαρμογή:

- Columns.
- Data source.
- Current cell.
- Selection.
- Commands.
- Events.
- Settings.
- Extension points.

Οι εσωτερικές δομές projection, layout cache και rendering state μπορούν να μείνουν private. Η εφαρμογή πρέπει να συνεργάζεται με το grid, όχι να μπαίνει μέσα στη μηχανή του.

## 25. Events vs Overrides vs Services

### Γιατί υπάρχει αυτή η έννοια;

Η επέκταση μπορεί να γίνει με πολλούς τρόπους. Αν όλα είναι events, η εφαρμογή μπορεί να κάνει πολλά αλλά δύσκολα αντικαθιστά συμπεριφορά. Αν όλα είναι overrides, η επέκταση απαιτεί subclassing. Αν όλα είναι services, η αρχιτεκτονική μπορεί να γίνει βαριά.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Ένας πρακτικός διαχωρισμός:

- Events για αποφάσεις της εφαρμογής.
- Overrides για εξειδικευμένες παραλλαγές του grid.
- Services για αντικατάσταση ολόκληρων υποσυστημάτων.

Παράδειγμα:

```text
Need to validate a value -> event
Need a custom editor for one column -> event or column factory
Need completely different rendering policy -> renderer service
```

Το ζητούμενο είναι να υπάρχει επέκταση χωρίς να σπάει το runtime model.

## 26. Custom Columns

### Γιατί υπάρχει αυτή η έννοια;

Οι business εφαρμογές έχουν ειδικά πεδία. Δεν αρκούν πάντα text, number και date. Μπορεί να υπάρχει status column, progress column, lookup column, command column ή calculated column.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Μια custom column πρέπει να συμμετέχει στη σύμβαση:

```text
CustomColumn
    FormatValue(value)
    ParseValue(editorValue)
    CanAggregate(kind)
    CreateEditor()
    CompareValues(a, b)
```

Έτσι η στήλη δεν είναι απλώς εμφάνιση. Είναι type-specific συμπεριφορά που συνεργάζεται με rendering, editing, sorting, filtering και summaries.

## 27. Custom Editors

### Γιατί υπάρχει αυτή η έννοια;

Καμία βιβλιοθήκη editors δεν καλύπτει όλες τις ανάγκες. Ένας χρήστης μπορεί να χρειαστεί editor με lookup, validation, multi-field input ή ειδικό picker.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Ο custom editor πρέπει να μπει στο ίδιο lifecycle:

```text
load value
focus
return value
request commit
request cancel
cleanup
```

Αν ακολουθεί τη σύμβαση, το grid δεν χρειάζεται να ξέρει τις εσωτερικές λεπτομέρειες του. Το edit session παραμένει ενιαίο.

## 28. Custom Commands And Toolbar

### Γιατί υπάρχει αυτή η έννοια;

Ένα business grid δεν είναι μόνο προβολή και edit. Συχνά έχει actions: insert, delete, edit, refresh, export, approve, post, cancel, custom business commands.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Τα commands πρέπει να ξέρουν το runtime context:

- Current row.
- Current cell.
- Selection.
- Data source capabilities.
- Edit state.

Ένα command δεν πρέπει να ψάχνει μόνο του μέσα στο UI. Πρέπει να παίρνει καθαρό context από το grid runtime.

## Part VII - Reliability

## 29. Invalid State Prevention

### Γιατί υπάρχει αυτή η έννοια;

Τα πιο δύσκολα bugs σε grids είναι bugs κατάστασης. Editor ανοιχτός σε κελί που δεν είναι πια ορατό. Current cell σε hidden column. Selection σε row που αφαιρέθηκε από filter. Summary που υπολογίστηκε πριν τελειώσει το grouping.

Η πρόληψη invalid state είναι βασική ευθύνη του runtime.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Κάθε αλλαγή πρέπει να περνά από μικρό αριθμό κεντρικών μεθόδων:

```text
SetCurrentCell()
SetSelection()
BeginEdit()
CommitEdit()
CancelEdit()
RebuildProjection()
SetScrollOffset()
```

Αν όλα τα υποσυστήματα αλλάζουν state απευθείας, οι ασυνέπειες είναι αναπόφευκτες. Αν το runtime ελέγχει τις μεταβάσεις, το grid παραμένει σταθερό.

## 30. Performance Rules

### Γιατί υπάρχει αυτή η έννοια;

Ένα business grid μπορεί να χρησιμοποιείται όλη μέρα. Δεν αρκεί να δουλεύει σωστά. Πρέπει να δουλεύει γρήγορα και σταθερά.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Βασικοί κανόνες:

- Μη δημιουργείς πραγματικό control για κάθε κελί.
- Ζωγράφισε μόνο το viewport.
- Κράτα projection cache.
- Κάνε rebuild projection μόνο όταν χρειάζεται.
- Μη μετράς ξανά layout χωρίς λόγο.
- Μην ξαναζωγραφίζεις όλο το grid για μικρή αλλαγή αν μπορείς να περιορίσεις το invalidation.
- Κλείσε ή ακύρωσε active editors όταν το underlying context δεν είναι πια έγκυρο.

Η απόδοση δεν είναι ένα τελικό optimization. Είναι αρχιτεκτονική απόφαση.

## 31. Testing The Runtime

### Γιατί υπάρχει αυτή η έννοια;

Το rendering είναι δύσκολο να τεσταριστεί πλήρως, αλλά το runtime μπορεί να τεσταριστεί πολύ καλά. Και αξίζει, γιατί εκεί βρίσκονται οι κανόνες.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Καλά test targets:

- Filtering expressions.
- Sorting comparers.
- Group projection.
- Summary calculations.
- Navigation rules.
- Edit commit/cancel.
- Date normalization.
- Numeric parsing.
- Selection transitions.
- Scroll visible range.

Αν αυτά είναι σωστά, το visual layer γίνεται πιο αξιόπιστο επειδή πατά σε σταθερή μηχανή.

## 32. Conclusion

Ένα business grid runtime είναι μικρή μηχανή εφαρμογής. Δεν είναι απλώς renderer και δεν είναι απλώς data viewer. Είναι σύστημα που συντονίζει data access, projection, layout, rendering, interaction, editing και extension points.

Η βασική ιδέα είναι να μην αφήσουμε τις συμπεριφορές να απλωθούν τυχαία. Το grid χρειάζεται κεντρικό runtime state, καθαρό data adapter, projection pipeline, coordinate system, virtual scrolling, typed columns, typed editors και σαφές editing lifecycle.

Όταν αυτά υπάρχουν, το grid μπορεί να μεγαλώσει χωρίς να γίνει εύθραυστο. Μπορεί να δεχθεί custom columns, custom editors, filtering, grouping, summaries και commands χωρίς να χάσει τη συνοχή του.

Αυτό είναι το όριο ανάμεσα σε ένα απλό table και ένα πραγματικό business grid runtime.
