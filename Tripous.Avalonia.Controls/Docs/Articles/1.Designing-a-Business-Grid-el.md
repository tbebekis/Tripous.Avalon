# Designing a Business Grid

## 1. Introduction

Ένα business grid μοιάζει, στην πιο απλή του μορφή, με έναν πίνακα δεδομένων. Έχει γραμμές, στήλες και κελιά. Ο χρήστης βλέπει τιμές, μετακινείται, επιλέγει, αλλάζει δεδομένα και εκτελεί ενέργειες. Αυτή η εικόνα όμως είναι παραπλανητικά απλή. Στις επιχειρησιακές εφαρμογές, το grid δεν είναι απλώς ένας τρόπος εμφάνισης δεδομένων. Είναι το βασικό σημείο όπου ο χρήστης συναντά το επιχειρησιακό μοντέλο.

Ένα business grid πρέπει να παρουσιάζει πολλά δεδομένα, να τα οργανώνει, να τα φιλτράρει, να τα ομαδοποιεί, να τα συνοψίζει, να τα επεξεργάζεται και να τα συνδέει με κανόνες εγκυρότητας, δικαιώματα, commands και workflows. Για αυτόν τον λόγο, η σωστή αρχιτεκτονική προσέγγιση δεν είναι να το δούμε ως ένα μεγάλο control. Είναι πιο χρήσιμο να το δούμε ως ένα μικρό runtime framework.

Αυτό το framework έχει δικό του μοντέλο κατάστασης, δικό του rendering pipeline, δικό του navigation model, δικό του editing engine και καθαρά σημεία συνεργασίας με το data source και την εφαρμογή.

Το άρθρο αυτό περιγράφει το business grid ως γενική αρχιτεκτονική έννοια. Δεν αναφέρεται σε συγκεκριμένο framework, τεχνολογία ή προϊόν. Ο στόχος είναι να εξηγήσει γιατί υπάρχουν οι βασικές έννοιες ενός σοβαρού grid και πώς συνεργάζονται μεταξύ τους.

## Part I - The Problem

## 2. Why Business Applications Need Grids

### Γιατί υπάρχει αυτή η έννοια;

Οι επιχειρησιακές εφαρμογές χειρίζονται συλλογές από ομοειδή αντικείμενα: παραστατικά, πελάτες, προϊόντα, κινήσεις, γραμμές παραγγελίας, λογιστικές εγγραφές, αποθήκες, εργασίες, αιτήματα, πληρωμές. Ο χρήστης σπάνια εργάζεται με ένα μόνο αντικείμενο απομονωμένο από τα υπόλοιπα. Συνήθως χρειάζεται να βλέπει λίστες, να συγκρίνει τιμές, να εντοπίζει αποκλίσεις, να κάνει μαζικές διορθώσεις και να καταλαβαίνει τη συνολική εικόνα.

Το grid υπάρχει επειδή αυτή η εργασία είναι φυσικά tabular. Οι επιχειρησιακές οντότητες έχουν πεδία. Οι συλλογές τους έχουν γραμμές. Οι αποφάσεις του χρήστη βασίζονται στη σύγκριση τιμών ανά στήλη και ανά γραμμή. Ένα grid επιτρέπει στον χρήστη να σαρώνει οπτικά, να ταξινομεί, να φιλτράρει, να μετακινείται γρήγορα και να επεξεργάζεται χωρίς να χάνει το πλαίσιο.

Σε πολλές εφαρμογές, το grid είναι το πραγματικό workspace. Δεν είναι βοηθητικό στοιχείο. Είναι το σημείο όπου γίνεται η καθημερινή εργασία.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Επειδή το grid είναι workspace, δεν μπορεί να περιοριστεί στην εμφάνιση. Πρέπει να συνεργάζεται με:

- Το data source, από όπου αντλεί και όπου επιστρέφει τιμές.
- Τις στήλες, που περιγράφουν πώς ερμηνεύεται κάθε πεδίο.
- Το navigation model, που επιτρέπει γρήγορη μετακίνηση.
- Το editing model, που οργανώνει την αλλαγή τιμών.
- Το filtering, sorting και grouping, που μετατρέπουν τα raw δεδομένα σε χρήσιμη προβολή.
- Τα summaries, που δίνουν συνολική εικόνα.
- Τα commands, που συνδέουν το grid με επιχειρησιακές ενέργειες.

Έτσι το grid γίνεται κόμβος συνεργασίας. Δεν κατέχει μόνο του την επιχειρησιακή λογική, αλλά πρέπει να την υποστηρίζει με καθαρό και επεκτάσιμο τρόπο.

## 3. Why Traditional Table Controls Are Not Enough

### Γιατί υπάρχει αυτή η έννοια;

Ένα απλό table control συνήθως απαντά στο ερώτημα: πώς εμφανίζω μια συλλογή τιμών σε γραμμές και στήλες; Αυτό είναι αρκετό για απλές λίστες. Δεν είναι αρκετό για επιχειρησιακή εργασία.

Σε ένα business grid, ο χρήστης χρειάζεται περισσότερα:

- Να αλλάζει τιμές μέσα στο grid.
- Να βλέπει διαφορετικούς editors ανά τύπο πεδίου.
- Να κάνει lookup σε βοηθητικούς πίνακες.
- Να βλέπει συγκεντρωτικά.
- Να φιλτράρει με επιχειρησιακούς όρους.
- Να ομαδοποιεί και να αναλύει.
- Να χειρίζεται πολλές χιλιάδες γραμμές.
- Να κρατά σταθερό context καθώς μετακινείται.
- Να βλέπει τι επιτρέπεται και τι απαγορεύεται ανά στήλη, γραμμή ή κελί.

Ένα γενικό table control συνήθως δεν έχει αρκετά πλούσιο runtime model για όλα αυτά. Αν προσπαθήσουμε να τα προσθέσουμε πρόχειρα, το αποτέλεσμα γίνεται ένα σύνολο από ειδικές περιπτώσεις. Το grid αρχίζει να συμπεριφέρεται απρόβλεπτα επειδή δεν υπάρχει σαφής αρχιτεκτονική.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Η ανεπάρκεια του απλού table οδηγεί στην ανάγκη για ξεχωριστά υποσυστήματα. Η εμφάνιση πρέπει να διαχωριστεί από το data model. Η επιλογή πρέπει να διαχωριστεί από το current cell. Το editing πρέπει να γίνει ξεχωριστό session. Το sorting και filtering πρέπει να ανήκουν σε μηχανισμό προβολής. Το rendering πρέπει να βασίζεται σε virtualization και όχι σε χιλιάδες πραγματικά controls.

Με άλλα λόγια, το business grid υπάρχει επειδή η απλή αναπαράσταση γραμμών και στηλών δεν αρκεί. Χρειάζεται ένα runtime που να συντονίζει πολλές έννοιες.

## Part II - The Runtime Model

## 4. The Grid as a Runtime System

### Γιατί υπάρχει αυτή η έννοια;

Ένα business grid δεν μπορεί να είναι στατικό. Η κατάσταση του αλλάζει συνεχώς. Ο χρήστης μετακινείται, επιλέγει, ανοίγει editors, κλείνει editors, αλλάζει φίλτρα, ταξινομεί, ομαδοποιεί, κάνει scroll, προσθέτει γραμμές, διαγράφει γραμμές και αλλάζει την προβολή.

Αυτή η δυναμική κατάσταση χρειάζεται runtime model. Το grid πρέπει να γνωρίζει όχι μόνο ποια δεδομένα εμφανίζει, αλλά και σε ποια κατάσταση βρίσκεται η προβολή τους.

Βασικές runtime έννοιες είναι:

- Columns: η περιγραφή των πεδίων.
- Rows: οι εμφανιζόμενες γραμμές της προβολής.
- Current Cell: το κελί όπου βρίσκεται η ενεργή θέση εργασίας.
- Current Row: η ενεργή γραμμή.
- Selection: τι έχει επιλέξει ο χρήστης.
- Editing: αν υπάρχει ενεργό edit session.
- Scrolling: ποιο τμήμα της προβολής είναι ορατό.
- Groups: πώς οργανώνονται οι γραμμές σε ιεραρχία.
- Filters: ποιο subset των δεδομένων εμφανίζεται.
- Sorting: με ποια σειρά εμφανίζονται τα δεδομένα.

Χωρίς runtime model, κάθε αλληλεπίδραση θα έπρεπε να υπολογίζεται αποσπασματικά. Αυτό οδηγεί σε ασυνέπειες: άλλο κελί φαίνεται current, άλλο κελί δέχεται input, άλλη γραμμή είναι selected, άλλο row index γράφεται στο data source.

Οι στήλες δεν είναι απλές επικεφαλίδες. Είναι descriptors. Μια βασική στήλη ορίζει τα κοινά χαρακτηριστικά: όνομα πεδίου, τίτλο, πλάτος, ορατότητα, μορφοποίηση, στοίχιση, δυνατότητα επεξεργασίας και δυνατότητα συμμετοχής σε λειτουργίες όπως sorting, filtering ή summaries.

Πάνω σε αυτή τη βασική έννοια εμφανίζονται εξειδικευμένοι τύποι στηλών:

- Text column.
- Number column.
- Date column.
- Boolean column.
- Lookup column.
- Custom business column.

Η ύπαρξη τύπων στηλών είναι αρχιτεκτονικά σημαντική. Το grid δεν πρέπει να μαντεύει συνεχώς τι σημαίνει μια τιμή. Η στήλη είναι το σημείο όπου συγκεντρώνεται η γνώση για το πεδίο. Μια number column ξέρει πώς να κάνει parse αριθμούς και ποια aggregates είναι έγκυρα. Μια date column ξέρει πώς να εμφανίζει και να διαβάζει ημερομηνίες. Μια lookup column ξέρει ότι η αποθηκευμένη τιμή μπορεί να είναι κωδικός, ενώ η εμφανιζόμενη τιμή είναι περιγραφή.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το runtime model είναι το κέντρο συντονισμού. Οι στήλες περιγράφουν τι σημαίνει κάθε κελί. Το data source δίνει τις τιμές. Το rendering pipeline ρωτά το runtime model τι πρέπει να εμφανίσει. Το navigation manager αλλάζει το current cell. Το selection manager αλλάζει την επιλογή. Το editing engine ξεκινά και ολοκληρώνει edit sessions. Το scroll manager καθορίζει ποια rows είναι ορατά.

Το runtime model δεν είναι απαραίτητα ορατό στον χρήστη, αλλά κάθε ορατή συμπεριφορά περνά μέσα από αυτό.

Ένα απλό ψευδομοντέλο μπορεί να μοιάζει έτσι:

```text
GridRuntime
    Columns
        BaseColumn
        TextColumn
        NumberColumn
        DateColumn
        LookupColumn
    ViewRows
    CurrentCell
    Selection
    EditSession
    ScrollState
    SortState
    FilterState
    GroupState
```

Το σημαντικό δεν είναι η συγκεκριμένη μορφή. Το σημαντικό είναι ότι το grid χρειάζεται μία συνεκτική εικόνα της κατάστασης του.

## 5. The Data Model

### Γιατί υπάρχει αυτή η έννοια;

Το grid παρουσιάζει δεδομένα. Δεν πρέπει να θεωρούμε ότι τα κατέχει. Τα δεδομένα ανήκουν σε κάποιο ανεξάρτητο data source: μια συλλογή αντικειμένων, ένα tabular model, ένα remote source, ένα projection ή οποιοδήποτε άλλο μοντέλο παρέχει γραμμές και τιμές.

Ο διαχωρισμός αυτός είναι κρίσιμος. Αν το grid αποθηκεύει τα δεδομένα ως δική του εσωτερική δομή, τότε γίνεται δύσκολο να συνεργαστεί με την εφαρμογή, να υποστηρίξει validation, transactions, lazy loading, permissions, undo, audit, synchronization ή server-side operations.

Το grid πρέπει να διαβάζει και να γράφει μέσω καθαρής σύμβασης. Δεν χρειάζεται να γνωρίζει πού πραγματικά ζουν τα δεδομένα.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το data source παρέχει τρεις βασικές δυνατότητες:

- Ανάγνωση τιμών.
- Εγγραφή τιμών.
- Πληροφορίες για το αν μια γραμμή ή ένα κελί μπορεί να αλλάξει.

Το grid χρησιμοποιεί αυτές τις δυνατότητες σε όλα τα υποσυστήματα:

- Το renderer ζητά τιμές για να τις ζωγραφίσει.
- Το editing engine ζητά την παλιά τιμή και γράφει τη νέα.
- Το filtering engine διαβάζει τιμές για να αποφασίσει ποιες γραμμές περνούν.
- Το sorting engine συγκρίνει τιμές.
- Το grouping engine δημιουργεί group keys.
- Το summary engine υπολογίζει aggregates.

Ένα γενικό data access contract μπορεί να εκφραστεί ως εξής:

```text
DataSource
    RowCount
    GetValue(row, column)
    SetValue(row, column, value)
    CanEdit(row, column)
    Insert(row)
    Delete(row)
```

Το grid δεν χρειάζεται να ξέρει αν πίσω από αυτό υπάρχει μνήμη, βάση δεδομένων ή υπηρεσία. Χρειάζεται μόνο σταθερή σύμβαση συνεργασίας.

## Part III - Rendering

## 6. Rendering vs Controls

### Γιατί υπάρχει αυτή η έννοια;

Ένα business grid μπορεί να εμφανίζει χιλιάδες ή εκατομμύρια κελιά. Αν κάθε κελί ήταν ένα πραγματικό UI element, το κόστος θα ήταν τεράστιο. Θα υπήρχαν χιλιάδες αντικείμενα, χιλιάδες layout calculations, χιλιάδες event routes και μεγάλη κατανάλωση μνήμης.

Γι’ αυτό τα επαγγελματικά grids συνήθως ζωγραφίζουν τα περιεχόμενά τους. Το κελί δεν είναι απαραίτητα control. Είναι ένα rectangle με κείμενο, χρώματα, borders, icons, glyphs και states. Πραγματικά controls δημιουργούνται μόνο όταν χρειάζονται, κυρίως για editing.

Αυτή η προσέγγιση επιτρέπει:

- Υψηλή απόδοση.
- Virtualization.
- Σταθερό scrolling.
- Πλήρη έλεγχο της εμφάνισης.
- Μικρό αριθμό πραγματικών UI objects.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το rendering pipeline συνεργάζεται με το runtime model. Δεν αποφασίζει μόνο του τι υπάρχει. Ρωτά:

- Ποιες στήλες είναι ορατές;
- Ποιες γραμμές είναι ορατές;
- Ποιο κελί είναι current;
- Ποιες γραμμές είναι selected;
- Ποιες στήλες έχουν sorting glyph;
- Ποια φίλτρα είναι ενεργά;
- Ποια groups είναι expanded;
- Υπάρχει active editor που πρέπει να εμφανιστεί πάνω από ένα κελί;

Το rendering δεν είναι data model. Είναι προβολή κατάστασης. Αν το runtime model είναι σωστό, το rendering μπορεί να είναι απλό και προβλέψιμο.

## 7. Coordinate System

### Γιατί υπάρχει αυτή η έννοια;

Το grid πρέπει να μεταφράζει συνεχώς ανάμεσα σε λογικές έννοιες και οπτικές συντεταγμένες. Ο χρήστης δεν κάνει click σε row object. Κάνει click σε ένα σημείο της οθόνης. Το grid πρέπει να καταλάβει αν το σημείο αυτό αντιστοιχεί σε κελί, header, filter cell, group row, scrollbar, resize handle ή κενό χώρο.

Αυτό απαιτεί coordinate system.

Βασικές έννοιες είναι:

- Row index.
- Column index.
- Cell rectangle.
- Visible area.
- Horizontal offset.
- Vertical offset.
- Hit testing.
- Scroll position.

Χωρίς σαφές coordinate system, οι αλληλεπιδράσεις γίνονται εύθραυστες. Το click μπορεί να επιλέγει λάθος κελί. Το editor μπορεί να εμφανίζεται σε λάθος θέση. Το drag μπορεί να μη βρίσκει σωστά drop target.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το coordinate system είναι γέφυρα ανάμεσα στο rendering και στο interaction.

Το renderer το χρησιμοποιεί για να ζωγραφίσει:

```text
for each visible row
    for each visible column
        rect = getCellRectangle(row, column)
        drawCell(rect, value)
```

Το hit testing το χρησιμοποιεί αντίστροφα:

```text
hit = hitTest(pointerPosition)
if hit.kind == Cell
    setCurrentCell(hit.row, hit.column)
```

Το scrolling αλλάζει το visible area. Το navigation μπορεί να ζητήσει να γίνει scroll ώστε το current cell να γίνει ορατό. Το editing engine ζητά το rectangle του active cell για να τοποθετήσει τον editor.

Άρα οι συντεταγμένες δεν είναι λεπτομέρεια εμφάνισης. Είναι κοινή γλώσσα για rendering, input, navigation και editing.

## 8. Virtual Scrolling

### Γιατί υπάρχει αυτή η έννοια;

Ένα business grid μπορεί να έχει πολύ περισσότερες γραμμές από όσες χωρούν στην οθόνη. Μπορεί να έχει δέκα χιλιάδες γραμμές, αλλά ο χρήστης να βλέπει μόνο τριάντα. Αν το grid προσπαθήσει να δημιουργήσει ή να ζωγραφίσει όλες τις γραμμές κάθε φορά, η απόδοση θα καταρρεύσει.

Το virtual scrolling υπάρχει για να λύνει αυτό το πρόβλημα. Η βασική ιδέα είναι ότι το grid δεν αντιμετωπίζει όλη τη λίστα ως ορατή επιφάνεια. Αντιμετωπίζει την οθόνη σαν έναν θάλαμο ασανσέρ. Πάνω από τον θάλαμο υπάρχει μια μεγάλη αόρατη περιοχή. Κάτω από τον θάλαμο υπάρχει άλλη μια μεγάλη αόρατη περιοχή. Ο χρήστης νιώθει ότι κινείται μέσα σε ολόκληρο το κτίριο, αλλά το grid ζωγραφίζει μόνο ό,τι βρίσκεται μέσα στον θάλαμο.

Ο θάλαμος αυτός είναι το viewport.

```text
virtual space

    rows above viewport
    not rendered

    ┌─────────────────────┐
    │ viewport             │
    │ rendered rows only   │
    └─────────────────────┘

    rows below viewport
    not rendered
```

Το scrollbar δεν σημαίνει ότι υπάρχουν πραγματικά visual objects για όλες τις γραμμές. Σημαίνει ότι υπάρχει μια συνολική λογική έκταση. Το thumb δείχνει σε ποιο σημείο αυτής της έκτασης βρίσκεται το viewport.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το virtual scrolling συνεργάζεται με το coordinate system και το rendering pipeline. Το grid πρέπει να ξέρει:

- Πόσες λογικές γραμμές υπάρχουν συνολικά.
- Ποια είναι η πρώτη ορατή γραμμή.
- Πόσες γραμμές χωρούν στο viewport.
- Ποιο vertical offset αντιστοιχεί στη θέση του scrollbar.
- Ποια cell rectangles υπάρχουν μόνο για τις ορατές γραμμές.

Η γενική ροή είναι:

```text
scroll position
    ↓
first visible row
    ↓
visible row range
    ↓
render only that range
```

Το hit testing επίσης δουλεύει μόνο μέσα στο viewport. Αν ο χρήστης κάνει click στο τρίτο ορατό row, το grid μεταφράζει αυτή τη θέση σε logical row index:

```text
logicalRow = firstVisibleRow + visibleRowOffset
```

Το editing engine χρειάζεται το ίδιο μοντέλο. Ένας editor μπορεί να εμφανιστεί μόνο για κελί που βρίσκεται μέσα στο viewport. Αν ο χρήστης κάνει scroll ενώ υπάρχει active editor, το grid πρέπει να αποφασίσει αν θα τον μετακινήσει, θα τον κλείσει με commit ή θα τον ακυρώσει. Η απόφαση είναι θέμα πολιτικής, αλλά το virtual scrolling κάνει την ανάγκη σαφή: ο editor δεν είναι μέρος όλων των rows, είναι προσωρινό στοιχείο πάνω από το visible cell.

Το virtual scrolling συνεργάζεται επίσης με grouping και filtering. Το grid δεν κάνει scroll πάνω στα raw data rows. Κάνει scroll πάνω στην τρέχουσα προβολή. Αν υπάρχουν groups, collapsed nodes ή filtered rows, το virtual space αντιστοιχεί στο projection, όχι απλώς στο αρχικό data source.

## Part IV - User Interaction

## 9. Navigation

### Γιατί υπάρχει αυτή η έννοια;

Ο χρήστης ενός business grid δουλεύει γρήγορα. Περιμένει να μετακινείται με βελάκια, Tab, Enter, Page Up, Page Down, Home και End. Περιμένει επίσης το mouse να συνεργάζεται με το keyboard, όχι να δημιουργεί ξεχωριστή κατάσταση.

Το navigation model υπάρχει για να ορίσει πώς κινείται η ενεργή θέση εργασίας μέσα στο grid. Η κεντρική έννοια είναι το current cell. Αυτό είναι το σημείο στο οποίο θα εφαρμοστεί η επόμενη ενέργεια: edit, toggle, paste, command ή navigation.

Χωρίς navigation model, κάθε key handler θα αποφάσιζε μόνος του τι σημαίνει “δεξιά”, “κάτω” ή “επόμενο editable cell”. Αυτό οδηγεί σε διαφορετικές συμπεριφορές ανάλογα με το σημείο εισόδου.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το navigation manager συνεργάζεται με:

- Το column model, για να ξέρει ποιες στήλες είναι ορατές ή editable.
- Το row model, για να ξέρει ποιες γραμμές είναι data rows και ποιες group ή summary rows.
- Το scroll manager, για να κρατά το current cell ορατό.
- Το editing engine, για να αποφασίζει τι γίνεται όταν ο χρήστης πατά Tab ή Enter μέσα σε editor.
- Το selection manager, γιατί πολλές φορές η αλλαγή current cell ενημερώνει και την επιλογή.

Το focus είναι επίσης σημαντικό. Το grid πρέπει να ξέρει πότε έχει keyboard focus και πότε ένα active editor έχει focus. Αυτές οι δύο καταστάσεις συνεργάζονται αλλά δεν είναι ίδιες.

## 10. Selection

### Γιατί υπάρχει αυτή η έννοια;

Η επιλογή δεν είναι μία έννοια. Σε ένα grid υπάρχουν πολλές σχετικές αλλά διαφορετικές καταστάσεις:

- Current Cell: το ενεργό κελί.
- Current Row: η γραμμή του ενεργού κελιού.
- Focused Row: η γραμμή που έχει οπτική έμφαση.
- Selected Rows: οι γραμμές που έχουν επιλεγεί για κάποια ενέργεια.
- Multiple Selection: περισσότερες από μία επιλεγμένες γραμμές.
- Block Selection: ορθογώνια περιοχή κελιών.

Αυτές οι έννοιες συχνά συγχέονται. Όμως η σύγχυση δημιουργεί προβλήματα. Μια γραμμή μπορεί να είναι current χωρίς να ανήκει σε multi-selection. Ένα κελί μπορεί να έχει focus ενώ πολλές γραμμές είναι selected. Μια εντολή delete μπορεί να αφορά selected rows, ενώ μια εντολή edit αφορά current cell.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το selection manager συνεργάζεται με navigation, rendering και commands.

Το navigation αλλάζει current cell. Το selection μπορεί να ακολουθεί ή όχι. Το rendering πρέπει να δείχνει διαφορετικά το current cell, την current row και τις selected rows. Τα commands πρέπει να ξέρουν ποιο σύνολο αφορά η ενέργεια τους.

Για παράδειγμα:

```text
currentCell = row 10, column Amount
selectedRows = [10, 11, 12]

EditCommand -> uses currentCell
DeleteCommand -> uses selectedRows
CopyCommand -> may use selected block or selected rows
```

Η αρχιτεκτονική πρέπει να επιτρέπει αυτές τις διαφορές αντί να τις κρύβει.

## 11. Editing

### Γιατί υπάρχει αυτή η έννοια;

Η επεξεργασία σε ένα business grid δεν είναι απλώς “ένα textbox μέσα στο κελί”. Είναι session. Έχει αρχή, ενδιάμεση κατάσταση και τέλος.

Ένα edit session πρέπει να απαντά:

- Ποιο κελί επεξεργαζόμαστε;
- Ποια ήταν η αρχική τιμή;
- Ποιος editor χρειάζεται;
- Πώς μετατρέπεται το text ή selected value σε πραγματική τιμή;
- Πότε γίνεται commit;
- Πότε γίνεται cancel;
- Ποιος κάνει validation;
- Τι συμβαίνει αν η τιμή είναι άκυρη;

Το editing υπάρχει ως ξεχωριστό υποσύστημα επειδή έχει δικούς του κανόνες ζωής. Δεν είναι μέρος του rendering, αν και εμφανίζεται οπτικά. Δεν είναι μέρος του data source, αν και γράφει τελικά σε αυτό. Δεν είναι απλή πλοήγηση, αν και επηρεάζεται από Tab και Enter.

Η γενική ροή είναι:

```text
Begin Edit
    ↓
Create Editor
    ↓
Editor Host
    ↓
User Input
    ↓
Validate / Normalize
    ↓
Commit or Cancel
```

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το editing engine συνεργάζεται με:

- Το current cell, για να ξέρει τι επεξεργάζεται.
- Τη στήλη, για να επιλέξει editor και parser.
- Το data source, για να διαβάσει την παλιά τιμή και να γράψει τη νέα.
- Το validation layer, για να εγκρίνει ή να απορρίψει την αλλαγή.
- Το renderer, για να εμφανίσει edit state.
- Το coordinate system, για να τοποθετήσει τον editor πάνω από το κελί.
- Το navigation model, για Tab, Enter και Escape.

Ο editor είναι προσωρινός μηχανισμός. Δημιουργείται μόνο όταν υπάρχει ενεργό edit session και εξαφανίζεται όταν το session ολοκληρωθεί. Αυτό κρατά το grid ελαφρύ και προβλέψιμο.

Αρχιτεκτονικά, είναι χρήσιμο να υπάρχει ένας base in-place editor. Αυτός δεν είναι απαραίτητα ένα απλό text input. Είναι ένας μικρός editor host με κοινό lifecycle:

- Load value.
- Focus.
- Select content, όταν έχει νόημα.
- Return current editor value.
- Commit request.
- Cancel request.
- Cleanup.

Πάνω σε αυτόν τον base editor μπορούν να χτιστούν εξειδικευμένοι editors:

- Text editor.
- Number editor.
- Date editor.
- Boolean editor.
- Lookup editor.
- Custom business editor.

Η κληρονομικότητα εδώ δεν είναι απλώς τεχνική ευκολία. Εκφράζει μια αρχιτεκτονική σύμβαση: όλοι οι editors συμμετέχουν στο ίδιο editing pipeline, αλλά ο καθένας έχει διαφορετικό εσωτερικό μηχανισμό.

Μερικοί editors είναι απλοί. Ένας text editor μπορεί να είναι μόνο ένα text input. Ένας number editor μπορεί να είναι text input με right alignment και numeric validation. Άλλοι editors είναι σύνθετοι. Ένας date editor μπορεί να έχει text input και κουμπί που ανοίγει calendar. Ένας lookup editor μπορεί να έχει text display, κουμπί drop-down και λίστα επιλογών. Ένας custom editor μπορεί να περιέχει πολλά εσωτερικά controls.

Το σημαντικό είναι ότι το grid δεν πρέπει να μπερδεύει τον editor με το κελί. Το κελί είναι λογική θέση και rendered rectangle. Ο editor είναι προσωρινός μηχανισμός που τοποθετείται πάνω από το κελί.

```text
InplaceEditorBase
    TextEditor
    NumberEditor
    DateEditor
        text area
        drop-down calendar
    LookupEditor
        display area
        drop-down list
    CustomEditor
```

Το drop-down είναι επίσης μέρος της αρχιτεκτονικής. Δεν ανήκει στο data source και δεν είναι μόνιμο παιδί του grid. Είναι προσωρινό overlay που συνεργάζεται με το editor session. Πρέπει να ξέρει πότε ανοίγει, πότε παίρνει focus, πότε επιστρέφει τιμή και πότε κλείνει χωρίς commit.

## Part V - Data Operations

## 12. Sorting

### Γιατί υπάρχει αυτή η έννοια;

Ο χρήστης χρειάζεται να αλλάζει τη σειρά των δεδομένων για να βρει μοτίβα. Ποια παραστατικά είναι πιο πρόσφατα; Ποιοι πελάτες έχουν μεγαλύτερο υπόλοιπο; Ποιες κινήσεις έχουν μηδενική ποσότητα; Το sorting υπάρχει για να αλλάζει η προβολή χωρίς να αλλάζουν απαραίτητα τα ίδια τα δεδομένα.

Σε ένα business grid, sorting δεν σημαίνει απλώς σύγκριση strings. Η στήλη έχει τύπο. Οι τιμές μπορεί να είναι αριθμοί, ημερομηνίες, lookup values ή calculated values. Η ταξινόμηση πρέπει να σέβεται την πραγματική τιμή, όχι μόνο το εμφανιζόμενο κείμενο.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το sorting engine συνεργάζεται με:

- Το column model, για να ξέρει ποια στήλη ταξινομείται.
- Το data source, για να πάρει τιμές.
- Το view projection, για να αλλάξει τη σειρά εμφάνισης.
- Το rendering, για να εμφανίσει sort glyph.
- Το grouping, γιατί sorting μέσα σε groups μπορεί να έχει διαφορετική σημασία από sorting ολόκληρης της λίστας.

Η ταξινόμηση πρέπει να είναι κατάσταση της προβολής. Δεν πρέπει αναγκαστικά να αναδιατάσσει το underlying data source.

## 13. Filtering

### Γιατί υπάρχει αυτή η έννοια;

Το filtering υπάρχει επειδή ο χρήστης σπάνια θέλει να βλέπει όλα τα δεδομένα ταυτόχρονα. Θέλει να περιορίσει την προβολή σε ό,τι έχει νόημα εκείνη τη στιγμή: συγκεκριμένο πελάτη, ημερομηνία, κατάσταση, ποσό, κείμενο ή συνδυασμό όρων.

Ένα business filter πρέπει να είναι πιο πλούσιο από απλή αναζήτηση κειμένου. Χρειάζεται operators, wildcards, comparison semantics και σωστή αντιμετώπιση τύπων.

Παραδείγματα:

```text
> 100
<= 30
%abc
<> closed
```

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το filtering engine δημιουργεί ένα subset της προβολής. Αυτό επηρεάζει:

- Το row count που βλέπει το renderer.
- Το navigation, γιατί κάποιες γραμμές εξαφανίζονται.
- Το selection, γιατί selected rows μπορεί να μη φαίνονται πλέον.
- Το grouping, γιατί τα groups πρέπει να δημιουργούνται πάνω στα filtered rows.
- Τα summaries, γιατί τα totals συνήθως αφορούν την τρέχουσα προβολή.

Το filter δεν είναι απλώς text σε ένα filter cell. Είναι κανόνας που συμμετέχει στο projection των δεδομένων.

## 14. Grouping

### Γιατί υπάρχει αυτή η έννοια;

Το grouping υπάρχει για να μετατρέπει μια επίπεδη λίστα σε αναλυτική δομή. Ο χρήστης μπορεί να θέλει να δει παραγγελίες ανά πελάτη, κινήσεις ανά αποθήκη, έξοδα ανά κατηγορία ή εργασίες ανά κατάσταση.

Το grouping δεν είναι απλή ταξινόμηση. Δημιουργεί νέες γραμμές που δεν υπάρχουν στο data source: group headers, group summaries, ίσως collapsed placeholders. Άρα αλλάζει τη μορφή της προβολής.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το grouping engine συνεργάζεται με:

- Τις grouped columns, που ορίζουν τα group keys.
- Το sorting, γιατί τα groups και τα παιδιά τους έχουν σειρά.
- Το filtering, γιατί τα groups περιέχουν μόνο τις γραμμές που περνούν τα filters.
- Το summary engine, για να υπολογίζονται aggregates ανά group.
- Το rendering, για να εμφανίζονται group headers και expand/collapse glyphs.
- Το navigation, γιατί όχι όλες οι εμφανιζόμενες γραμμές είναι editable data rows.

Η προβολή του grid παύει να είναι απλή λίστα data rows. Γίνεται projection tree.

## 15. Summaries

### Γιατί υπάρχει αυτή η έννοια;

Οι επιχειρησιακοί χρήστες χρειάζονται συνολική εικόνα. Δεν αρκεί να βλέπουν τις γραμμές. Θέλουν count, sum, min, max, average και άλλα aggregates. Τα summaries υπάρχουν για να απαντούν γρήγορα σε ερωτήσεις όπως:

- Πόσες γραμμές υπάρχουν;
- Ποιο είναι το συνολικό ποσό;
- Ποια είναι η μικρότερη ή μεγαλύτερη τιμή;
- Ποιος είναι ο μέσος όρος;

Τα summaries είναι ιδιαίτερα σημαντικά όταν συνεργάζονται με filtering και grouping. Το ίδιο grid μπορεί να δείχνει totals για όλη την προβολή και summaries για κάθε group.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το summary engine διαβάζει την τρέχουσα προβολή, όχι απαραίτητα όλα τα δεδομένα. Συνεργάζεται με:

- Το filtering, για να υπολογίζει πάνω στις visible rows.
- Το grouping, για group-level summaries.
- Το column model, για να ξέρει ποια aggregates επιτρέπονται ανά τύπο.
- Το rendering, για να εμφανίζει footer summaries ή group summaries.

Ένα σημαντικό αρχιτεκτονικό σημείο είναι ότι δεν είναι όλα τα aggregates έγκυρα για όλους τους τύπους δεδομένων. Το sum έχει νόημα για αριθμούς, όχι για ημερομηνίες ή κείμενα. Το count έχει νόημα σχεδόν παντού. Το average μπορεί να χρειάζεται ειδική απόφαση ανά τύπο.

## Part VI - Architecture

## 16. Major Components

### Γιατί υπάρχει αυτή η έννοια;

Ένα business grid είναι αρκετά σύνθετο ώστε να χρειάζεται καθαρή εσωτερική διαίρεση. Αν όλα μπουν σε ένα μεγάλο αντικείμενο, η συμπεριφορά γίνεται δύσκολη στην κατανόηση και στην επέκταση.

Τα βασικά υποσυστήματα είναι:

- Grid: το δημόσιο σημείο συντονισμού.
- Data Source: η σύμβαση πρόσβασης στα δεδομένα.
- Column: η περιγραφή πεδίου και συμπεριφοράς, συχνά με εξειδικευμένους τύπους.
- Row: η λογική γραμμή της προβολής.
- Cell: η τομή row και column.
- Renderer: η εμφάνιση.
- Editor: η επεξεργασία, συνήθως μέσω base editor και εξειδικευμένων editors.
- Selection Manager: η επιλογή.
- Navigation Manager: η πλοήγηση.
- Scroll Manager: η ορατή περιοχή και η αντιστοίχιση logical rows σε viewport rows.

Οι στήλες και οι editors είναι συχνά μικρές ιεραρχίες τύπων. Υπάρχει μια βασική στήλη που εκφράζει την κοινή σύμβαση όλων των πεδίων και εξειδικευμένες στήλες που προσθέτουν type-specific συμπεριφορά. Αντίστοιχα, υπάρχει ένας βασικός in-place editor που εκφράζει το κοινό lifecycle και εξειδικευμένοι editors που αλλάζουν το εσωτερικό UI ή τον τρόπο επιλογής τιμής.

Το scroll manager δεν είναι απλώς αντικείμενο που ζωγραφίζει scrollbar. Είναι το υποσύστημα που κρατά τη σχέση ανάμεσα στη μεγάλη λογική προβολή και στο μικρό ορατό viewport. Χωρίς αυτό, το renderer δεν ξέρει ποιες γραμμές πρέπει να ζωγραφίσει και το hit testing δεν μπορεί να μεταφράσει ένα ορατό row σε πραγματικό logical row.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το grid λειτουργεί ως orchestrator. Δεν πρέπει απαραίτητα να κάνει τα πάντα μόνο του. Συντονίζει τα υποσυστήματα.

Μια τυπική συνεργασία για click σε κελί είναι:

```text
Pointer Input
    ↓
Hit Testing
    ↓
Navigation Manager updates Current Cell
    ↓
Selection Manager updates Selection
    ↓
Renderer invalidates affected area
```

Μια τυπική συνεργασία για edit είναι:

```text
Command: Begin Edit
    ↓
Column selects Editor
    ↓
Editor Host positions editor
    ↓
Data Source provides old value
    ↓
User changes value
    ↓
Column parses value
    ↓
Validation accepts or rejects
    ↓
Data Source receives new value
```

Η αξία της αρχιτεκτονικής βρίσκεται στη σαφή συνεργασία, όχι στον αριθμό των classes.

## 17. The Rendering Pipeline

### Γιατί υπάρχει αυτή η έννοια;

Το rendering pipeline υπάρχει για να μετατρέπει την runtime κατάσταση σε εικόνα. Χωρίς pipeline, κάθε αλλαγή θα οδηγούσε σε αποσπασματικό drawing. Αυτό κάνει δύσκολη τη συνέπεια και την απόδοση.

Ένα grid έχει πολλά bands και layers:

- Toolbar.
- Group panel.
- Column headers.
- Filter row.
- Body rows.
- Group rows.
- Summary rows.
- Scrollbars.
- Active editor.
- Drag indicators.

Το rendering pipeline ορίζει τη σειρά και τους κανόνες ζωγραφικής.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το pipeline διαβάζει state από όλα τα άλλα υποσυστήματα:

```text
Render
    draw background
    draw fixed bands
    draw headers
    draw filters
    draw visible rows
    draw summaries
    draw selection/current cell
    draw scrollbars
    draw transient overlays
```

Το renderer δεν πρέπει να αλλάζει επιχειρησιακή κατάσταση. Πρέπει να είναι κυρίως συνάρτηση της τρέχουσας κατάστασης. Όταν το navigation αλλάζει current cell, το renderer απλώς το απεικονίζει. Όταν το filtering αλλάζει view rows, το renderer απλώς ζωγραφίζει τη νέα προβολή.

## 18. The Editing Pipeline

### Γιατί υπάρχει αυτή η έννοια;

Το editing pipeline υπάρχει επειδή η αλλαγή τιμής είναι διαδικασία, όχι στιγμή. Από το πρώτο input μέχρι το τελικό commit παρεμβάλλονται επιλογή editor, αρχική τιμή, input, parsing, normalization, validation και write-back.

Αν αυτά δεν οργανωθούν σε pipeline, τότε κάθε editor θα συμπεριφέρεται διαφορετικά. Το text editor θα κάνει commit αλλιώς, το numeric αλλιώς, το date αλλιώς, το lookup αλλιώς. Αυτό κουράζει τον χρήστη και κάνει το grid δύσκολο στη συντήρηση.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Το editing pipeline πρέπει να είναι κοινό, ενώ οι editors μπορούν να διαφέρουν.

```text
BeginEdit(cell)
    if cell is editable
        create editor for column
        load current value
        position editor
        focus editor

CommitEdit()
    read editor value
    normalize value
    parse according to column
    validate
    write to data source
    close editor

CancelEdit()
    discard editor value
    restore visual state
    close editor
```

Οι editors είναι plugins στο pipeline. Δεν ορίζουν μόνοι τους το συνολικό lifecycle. Αυτό επιτρέπει στο grid να έχει ενιαία συμπεριφορά για Enter, Tab, Escape, focus loss και validation.

## Part VII - Extensibility

## 19. Extension Points

### Γιατί υπάρχει αυτή η έννοια;

Κανένα business grid δεν μπορεί να προβλέψει όλες τις επιχειρησιακές ανάγκες. Κάποια εφαρμογή θα χρειαστεί ειδικό lookup. Άλλη θα χρειαστεί custom renderer για status cells. Άλλη θα χρειαστεί command buttons, custom aggregates, ειδικό hit testing ή εξειδικευμένο editor.

Τα extension points υπάρχουν για να επιτρέπουν αυτές τις ανάγκες χωρίς να σπάει η βασική αρχιτεκτονική.

Πιθανά σημεία επέκτασης:

- Editors.
- Painters.
- Commands.
- Custom rendering.
- Custom columns.
- Hit testing.
- Validation.
- Normalization.
- Aggregates.
- Toolbar actions.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Ένα καλό extension point δεν παρακάμπτει το runtime model. Συνεργάζεται μαζί του.

Ένας custom editor πρέπει να μπαίνει στο editing pipeline. Ένας custom painter πρέπει να χρησιμοποιεί τα ίδια cell rectangles. Ένα custom command πρέπει να γνωρίζει current cell και selection. Ένα custom column πρέπει να παρέχει format, parse και type semantics.

Η επέκταση πρέπει να μοιάζει με συμμετοχή στο framework, όχι με patch έξω από αυτό.

Ένα γενικό παράδειγμα:

```text
Column
    FormatValue(value)
    ParseValue(editorValue)
    CreateEditor()
    CanAggregate(kind)
```

Αυτό δεν είναι υλοποίηση συγκεκριμένης τεχνολογίας. Είναι αρχιτεκτονική ιδέα: η στήλη είναι το σημείο όπου συγκεντρώνεται η γνώση για ένα πεδίο.

## Part VIII - Conclusion

## 20. A Business Grid Is a Small Framework

### Γιατί υπάρχει αυτή η έννοια;

Ένα business grid ξεκινά από μια απλή ανάγκη: να εμφανίσουμε γραμμές και στήλες. Πολύ γρήγορα όμως γίνεται κάτι περισσότερο. Πρέπει να διαχειριστεί κατάσταση, είσοδο χρήστη, προβολές, μετασχηματισμούς, editors, validation, scrolling, grouping, summaries και commands.

Αυτός είναι ο λόγος που ένα business grid δεν πρέπει να σχεδιάζεται ως ένα μεγάλο control με πολλές ειδικές περιπτώσεις. Πρέπει να σχεδιάζεται ως μικρό framework.

Έχει:

- Runtime model.
- Rendering engine.
- Navigation engine.
- Selection model.
- Editing engine.
- Data source contract.
- Projection engine.
- Extension points.

### Πώς συνεργάζεται με τις υπόλοιπες έννοιες;

Όλες οι έννοιες συνεργάζονται γύρω από ένα κοινό κέντρο: την προβολή επιχειρησιακών δεδομένων με τρόπο γρήγορο, συνεπή και επεκτάσιμο.

Το data source παρέχει τιμές. Οι στήλες δίνουν νόημα στις τιμές. Το runtime model κρατά την κατάσταση. Το renderer την εμφανίζει. Το navigation και selection model επιτρέπουν εργασία. Το editing engine επιτρέπει αλλαγές. Το sorting, filtering, grouping και summaries μετατρέπουν τα δεδομένα σε γνώση. Τα extension points επιτρέπουν στην εφαρμογή να προσαρμόσει το grid στις δικές της ανάγκες.

Η επιτυχία ενός business grid δεν κρίνεται μόνο από το αν εμφανίζει δεδομένα. Κρίνεται από το αν μπορεί να γίνει αξιόπιστο περιβάλλον εργασίας. Για να το πετύχει αυτό, πρέπει να έχει αρχιτεκτονική. Και αυτή η αρχιτεκτονική μοιάζει πολύ περισσότερο με μικρό runtime framework παρά με απλό UI component.
