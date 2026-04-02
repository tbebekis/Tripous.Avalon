Ο pivot engine κάνει ουσιαστικά 4 φάσεις.

**1. Παίρνει την πηγή και τη μετατρέπει σε ουδέτερες γραμμές**
Κάθε record από `IEnumerable<T>` ή `DataView` γίνεται ένα `RowSourceEntry`, δηλαδή ένα dictionary `FieldName -> Value`.
Άρα από εκεί και πέρα ο engine δεν νοιάζεται αν η πηγή ήταν object ή `DataRowView`. Όλα γίνονται ένα κοινό εσωτερικό format.

**2. Διαβάζει το PivotDef και χωρίζει τις στήλες σε 3 ρόλους**
Από το `PivotDef` βγάζει:

* row defs
* column defs
* value defs

Τα `valueDefs` είναι υποχρεωτικά. Αν δεν υπάρχουν, πετάει exception.
Άρα όλο το pivot χτίζεται πάνω σε:

* τι ομαδοποιώ στις rows
* τι ομαδοποιώ στις columns
* τι μετράω/αθροίζω ως values

**3. Κάνει μία πλήρη διέλευση στα δεδομένα και γεμίζει buckets**
Αυτή είναι η καρδιά.

Για κάθε source row:

* φτιάχνει `rowValues`
* φτιάχνει `columnValues`
* βγάζει keys με `ComposeKey()`

Μετά ενημερώνει πολλά dictionaries από `AggregateBucket`.

Το bucket είναι ο accumulator:

* count
* sum
* min/max
* product
* variance/stddev
* distinct set

Κάθε bucket ξέρει μόνο ένα aggregate type και με `Add()` μαζεύει τιμές, ενώ με `GetResult()` δίνει το τελικό aggregate.

Τα βασικά bucket layers είναι:

**A. Detail buckets**
Εδώ αποθηκεύονται τα καθαρά κελιά:
`detail row key + detail column key + value field`
Δηλαδή το πιο “κανονικό” pivot cell.

**B. Row subtotal buckets**
Για κάθε row level κρατάει buckets subtotal.
Παράδειγμα αν rows είναι `Region, Country`, τότε:

* level 0 => subtotal per Region
* level 1 => subtotal per Region/Country

Άρα μπορεί μετά να φτιάξει subtotal rows σε κάθε επίπεδο.

**C. Column subtotal buckets**
Αντίστοιχα για columns.
Έτσι μπορεί να βγάλει subtotal columns σε κάθε επίπεδο column grouping.

**D. Grand buckets**
Εδώ μπαίνουν τα grand totals, τόσο overall όσο και ανά column group όταν χρειάζεται.

**E. Sort buckets**
Εδώ είναι το ενδιαφέρον για το bug σου.
Υπάρχουν χωριστά:

* `rowSortBucketsByLevel`
* `columnSortBucketsByLevel`

Και γεμίζουν ανά level, ώστε κάθε group να μπορεί να αποκτήσει ένα numeric `SortValue` για sorting by value.

---

### Τι είναι το bucket key

Όλα σχεδόν βασίζονται σε string keys με separator.

Το bucket key είναι:
`rowKey + separator + columnKey + separator + valueFieldName`

Άρα το pivot δεν δουλεύει με tree objects στο aggregation.
Δουλεύει κυρίως με flat dictionaries keyed by composed strings. Αυτό είναι σημαντικό.

---

## Μετά αρχίζει η φάση “χτίζω το output”

Αυτό γίνεται στη `BuildPivotData()`.

Πρώτα φτιάχνει τις output row-header columns.
Για κάθε row def δημιουργεί μία `PivotDataColumn` kind `RowHeader`.

Μετά φτιάχνει τις output value columns.
Αυτό γίνεται με τη `BuildOutputColumns()`.

---

## Πώς χτίζονται οι output columns

Η `BuildOutputColumns()` παίρνει τα distinct detail column keys και τα ταξινομεί μέσω `SortAxisEntries(...)`.
Μετά, με recursion (`AppendColumnLevel`) τα περνάει επίπεδο-επίπεδο:

* αν έχει deeper level, κατεβαίνει
* αν είναι leaf, βάζει detail columns
* αν πρέπει, βάζει subtotal column μετά το group
* στο τέλος, αν ζητείται, βάζει grand total column

Σημαντικό:
το output column δεν είναι ακόμα τελική visible στήλη value.
Είναι axis output entry.
Μετά, για **κάθε** τέτοιο output column και για **κάθε** value def, δημιουργείται μια πραγματική visible value column στο result.
Άρα αν έχεις:

* 3 column groups
* 2 value fields

θα πάρεις 3 × 2 visible value columns.

---

## Πώς χτίζονται οι output rows

Αντίστοιχα η `BuildOutputRows()`:

* παίρνει τα distinct detail row keys
* τα ταξινομεί με `SortAxisEntries(...)`
* μετά η `AppendRowLevel()` περνάει recursive στα levels

Η λογική είναι:

* ομαδοποίησε τα συνεχόμενα items με ίδιο value στο current level
* αν υπάρχει deeper level, ξαναμπες μέσα
* αλλιώς πρόσθεσε detail rows
* και μετά, αν το group έχει πάνω από 1 detail row, πρόσθεσε subtotal row

Τέλος, αν χρειάζεται, προσθέτει ένα grand total row.

---

## Το sorting layer

Εδώ είναι όλη η ιστορία του bug.

Η `SortAxisEntries()`:

1. ξεκινά από ένα αρχικό `OrderBy` με `ObjectArrayComparer`
2. μετά, αν υπάρχει λόγος, καλεί `SortAxisEntriesLevel(..., level = 0)`

Η `SortAxisEntriesLevel()` κάνει:

1. `GroupByLevel(items, level)`
2. για κάθε group:

    * πρώτα ταξινομεί recursive τα παιδιά του
    * μετά, αν το axisDef έχει `SortByValue`, υπολογίζει `group.SortValue`
3. μετά κάνει `groups.Sort(...)`
4. μετά flatten όλων των `group.Items` σε ένα νέο list

Δηλαδή το sorting είναι **ιεραρχικό recursive sort**, όχι ένα απλό global sort.

---

## Πώς βγαίνει το SortValue

Το `GetGroupSortValue()`:

* βρίσκει το σωστό bucket map του συγκεκριμένου level
* φτιάχνει το group key από `GetSubtotalGroupValues(axisValues, level)`
* μετά δεν ζητάει ένα μόνο bucket
* κάνει scan σε όλα τα buckets του level και αθροίζει όσα ταιριάζουν στο:

    * row prefix ή grand-row prefix
    * σωστό value field suffix

Άρα το `SortValue` του group βγαίνει σαν **sum όλων των σχετικών bucket results** για εκείνο το group.

Αυτό είναι πολύ κρίσιμο:
το sorting by value δεν κοιτάζει τα raw rows.
Κοιτάζει προϋπολογισμένα sort buckets.

---

## Πού νομίζω ότι είναι η πιο ύποπτη περιοχή

Με λόγια, η ροή του sort είναι:

* φτιάχνω sort buckets ανά level
* ομαδοποιώ items ανά level
* για κάθε group βρίσκω sort value από τα buckets
* κάνω sort τα groups
* κάνω flatten

Άρα το bug στο level 0 μπορεί να είναι μόνο σε λίγα σημεία:

**1. Το level 0 group key**
Το `GetSubtotalGroupValues(axisValues, 0)` κρατά μόνο το πρώτο element.
Αν εκεί το key που βγαίνει δεν ταιριάζει ακριβώς με το key που γράφτηκε όταν γέμιζες τα `rowSortBucketsByLevel`, το `SortValue` θα βγει null.

**2. Το scan μέσα στο GetGroupSortValue**
Εδώ γίνεται prefix/suffix matching πάνω σε composed string keys.
Αν στο level 0 το prefix πιάνει περισσότερα ή λιγότερα buckets απ’ όσα πρέπει, τότε το sort value θα είναι λάθος, ειδικά για top groups.

**3. Το flatten μετά το group sort**
Αν τα groups ταξινομούνται σωστά αλλά το τελικό list ξαναβγαίνει σε “φυσική” σειρά λόγω του recursive regrouping/append logic, τότε φαίνεται σαν να μη δούλεψε το level 0 sort, ενώ στην πραγματικότητα χάθηκε αργότερα.

---

## Με μία πρόταση, τι κάνει όλος ο engine

Ο engine:
**παίρνει source rows, τις μετατρέπει σε flat key/value entries, προϋπολογίζει όλα τα detail/subtotal/grand aggregates σε buckets, μετά ταξινομεί ιεραρχικά τα distinct row/column groups και τέλος συνθέτει ένα τελικό tabular PivotData.**

## Και με μία δεύτερη πρόταση, πού θα κοίταζα πρώτο

Για το bug που περιγράφεις, εγώ θα κοίταζα πρώτα:
**αν στο `SortAxisEntriesLevel(... level 0)` τα `GroupInfo.SortValue` για Region γεμίζουν σωστά ή βγαίνουν null/λάθος πριν καν γίνει το `groups.Sort()`.**
