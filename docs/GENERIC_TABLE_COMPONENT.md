# Component GenericTable - Documentació

## 📋 Resum

S'ha creat un **ViewComponent genèric** per renderitzar taules de manera reutilitzable. Això segueix el principi **DRY (Don't Repeat Yourself)** i facilita el manteniment del codi.

## 🏗️ Estructura Implementada

### 1. Models (`TableViewModel.cs`)

#### `ColumnConfig`
Defineix la configuració d'una columna:
- `PropertyName`: Nom de la propietat del model (ex: "Name", "City")
- `DisplayName`: Text mostrat a la capçalera (ex: "Nom", "Ciutat")
- `IsSortable`: Si la columna es pot ordenar (default: true)
- `IsVisibleOnMobile`: Si la columna es mostra en mòbil (default: true)
- `Format`: Format per dates/números (ex: "dd/MM/yyyy", "C2")
- `CustomRender`: Funció personalitzada per renderitzar la cel·la
- `IsActionColumn`: Indica si és columna d'accions

#### `TableAction`
Defineix una acció (botó) disponible:
- `ActionName`: Nom de l'acció del controller (ex: "Edit", "Delete")
- `DisplayText`: Text del botó
- `CssClass`: Classes CSS (ex: "btn-primary", "btn-danger")
- `Icon`: Icona opcional (emoji o classe CSS)
- `Controller`: Controller personalitzat (si és diferent)
- `RequiresConfirmation`: Si necessita confirmació (per Delete)
- `ConfirmationMessage`: Missatge de confirmació

#### `TableViewModel<T>`
Model principal per configurar la taula:
- `Data`: Col·lecció de dades a mostrar
- `Columns`: Llista de configuracions de columnes
- `Actions`: Llista d'accions disponibles
- `EntityName`: Nom de l'entitat (ex: "Escoles", "Alumnes")
- `ControllerName`: Nom del controller per les accions
- `HasSearch`: Activa cerca (default: true)
- `HasPagination`: Activa paginació (default: false)
- `EmptyMessage`: Missatge quan no hi ha dades

### 2. ViewComponent (`GenericTableViewComponent.cs`)

ViewComponent simple que accepta un `TableViewModel<T>` i el passa a la vista.

```csharp
public IViewComponentResult Invoke(object model)
{
    // Validació del model
    return View("Default", model);
}
```

### 3. Vista del Component (`Default.cshtml`)

Vista Razor que renderitza la taula amb:
- ✅ **Cerca** en temps real (filtra totes les columnes)
- ✅ **Ordenació** per columnes (ascendent/descendent)
- ✅ **Responsive** automàtic (taula → cards en mòbil)
- ✅ **Accions** configurables (Edit, Delete, Details, etc.)
- ✅ **Renderització personalitzada** de cel·les
- ✅ **Confirmacions** per accions destructives
- ✅ **Empty state** quan no hi ha dades

## 🎯 Ús del Component

### Exemple: Schools/Index

```csharp
@{
    var tableConfig = new TableViewModel<SchoolViewModel>
    {
        Data = Model,
        EntityName = "Escoles",
        ControllerName = "Schools",
        EmptyMessage = "No hi ha escoles registrades.",
        
        Columns = new List<ColumnConfig>
        {
            new ColumnConfig 
            { 
                PropertyName = "Code", 
                DisplayName = "Codi",
                IsSortable = true 
            },
            new ColumnConfig 
            { 
                PropertyName = "IsFavorite", 
                DisplayName = "Favorit",
                CustomRender = (item) => 
                {
                    var school = item as SchoolViewModel;
                    return school?.IsFavorite == true
                        ? "<span class='star-icon star-filled'>★</span>"
                        : "<span class='star-icon star-outline'>☆</span>";
                }
            }
        },
        
        Actions = new List<TableAction>
        {
            new TableAction 
            { 
                ActionName = "Details", 
                DisplayText = "Detalls",
                CssClass = "btn-info",
                Icon = "👁️"
            },
            new TableAction 
            { 
                ActionName = "Delete", 
                DisplayText = "Esborrar",
                CssClass = "btn-danger",
                Icon = "🗑️",
                RequiresConfirmation = true,
                ConfirmationMessage = "Estàs segur?"
            }
        }
    };
}

@await Component.InvokeAsync("GenericTable", tableConfig)
```

## ✨ Funcionalitats Implementades

### 1. **Cerca en Temps Real**
- Input de cerca a la part superior
- Filtra per totes les columnes
- Case-insensitive
- JavaScript inline

### 2. **Ordenació de Columnes**
- Click a capçalera per ordenar
- Indicador visual (↑ ↓)
- Alterna entre ascendent/descendent
- Suporta números i text

### 3. **Responsive Design**
- **Desktop**: Taula clàssica
- **Mobile**: Transforma a cards
- Usa `data-label` per mostrar noms de columnes
- Botons d'accions en columna

### 4. **Renderització Personalitzada**
- `CustomRender`: Funció lambda per HTML personalitzat
- Automàtic per dates, booleans, decimals
- Format amb string.Format()

### 5. **Accions amb Confirmació**
- Forms POST per Delete
- AntiForgeryToken automàtic
- Confirmació JavaScript

## 📱 Responsive CSS

```css
@media (max-width: 767.98px) {
    .table thead { display: none; }
    
    .table tbody tr {
        display: block;
        margin-bottom: 1rem;
        border: 1px solid #dee2e6;
        border-radius: 4px;
    }
    
    .table tbody td:before {
        content: attr(data-label);
        font-weight: 600;
    }
}
```

## 🎨 Avantatges d'aquest Patró

### 1. **DRY (Don't Repeat Yourself)**
- Codi de taula escrit una sola vegada
- Reutilitzable per totes les entitats

### 2. **Mantenibilitat**
- Canvi en un lloc → afecta a totes les taules
- Fàcil afegir noves funcionalitats (ex: paginació)

### 3. **Coherència**
- Totes les taules funcionen igual
- Mateixa UX per tot el sistema

### 4. **Escalabilitat**
- Afegir nova entitat és trivial
- Només cal configurar columnes i accions

### 5. **Testabilitat**
- Component aïllat
- Es pot testejar independentment

## 🔄 Pròxims Passos

Amb aquest component creat, ara podem afegir:

1. **Paginació** centralitzada
2. **Filtres avançats** (per columna)
3. **Export** a Excel/CSV
4. **Selecció múltiple** (checkboxes)
5. **Accions en bloc** (esborrar múltiples)
6. **Drag & Drop** per reordenar
7. **Column visibility toggle**
8. **Temes** personalitzables

## 📚 Exemple Complet per Nova Entitat

Si vols afegir una taula per "Matrícules":

```csharp
var tableConfig = new TableViewModel<EnrollmentViewModel>
{
    Data = Model,
    EntityName = "Matrícules",
    ControllerName = "Enrollments",
    
    Columns = new List<ColumnConfig>
    {
        new ColumnConfig { PropertyName = "StudentName", DisplayName = "Alumne" },
        new ColumnConfig { PropertyName = "Year", DisplayName = "Any" },
        new ColumnConfig { PropertyName = "Status", DisplayName = "Estat" }
    },
    
    Actions = new List<TableAction>
    {
        new TableAction { ActionName = "Details", DisplayText = "Veure", CssClass = "btn-info" }
    }
};
```

**Així de simple!** 🎉

## 🎓 Conceptes Apresos

- **ViewComponents**: Components reutilitzables de Razor
- **Generics**: `TableViewModel<T>` funciona amb qualsevol tipus
- **Lambda expressions**: `CustomRender = (item) => { ... }`
- **Reflection**: Obtenir propietats dinàmicament amb `GetProperty()`
- **Responsive design**: Transform taula → cards amb CSS
- **JavaScript inline**: Cerca i ordenació sense frameworks
- **Clean Code**: Separació de responsabilitats

---

**Fitxers creats/modificats:**
- ✅ `Models/TableViewModel.cs` - Models de configuració
- ✅ `ViewComponents/GenericTableViewComponent.cs` - Component
- ✅ `Views/Shared/Components/GenericTable/Default.cshtml` - Vista
- ✅ `Views/Schools/Index.cshtml` - Actualitzada per usar component
- ✅ `Views/Students/Index.cshtml` - Actualitzada per usar component

**Resultat:** Component totalment funcional, responsive i reutilitzable! 🚀
