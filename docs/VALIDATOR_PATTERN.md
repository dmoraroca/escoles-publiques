# Patró Validator amb FluentValidation

## Descripció
S'ha implementat el **Patró Validator** utilitzant **FluentValidation** per garantir la validació de dades seguint els principis SOLID.

## Beneficis del Patró Validator

### 1. **Single Responsibility Principle (SRP)**
Cada validator té una única responsabilitat: validar un ViewModel específic.

### 2. **Open/Closed Principle (OCP)**
Les regles de validació es poden estendre sense modificar els controllers o models existents.

### 3. **Dependency Inversion Principle (DIP)**
Els controllers depenen d'abstraccions (`ModelState.IsValid`) en lloc d'implementacions concretes de validació.

### 4. **Separació de Concerns**
La lògica de validació està separada dels models (ViewModels) i dels controllers.

### 5. **Reutilització**
Les regles de validació són reutilitzables i es poden aplicar automàticament a través de l'integració amb ASP.NET Core.

## Implementació

### Validators Creats

#### 1. **SchoolViewModelValidator**
```csharp
- Code: NotEmpty, MaxLength(10)
- Name: NotEmpty, MaxLength(200)
- City: NotEmpty, MaxLength(100)
- Scope: MaxLength(50) (opcional)
```

#### 2. **StudentViewModelValidator**
```csharp
- FirstName: NotEmpty, MaxLength(100)
- LastName: NotEmpty, MaxLength(100)
- Email: EmailAddress, MaxLength(150) (opcional)
- BirthDate: LessThan(Now) (opcional)
- SchoolId: GreaterThan(0)
```

#### 3. **EnrollmentViewModelValidator**
```csharp
- StudentId: GreaterThan(0)
- Year: NotEmpty, InclusiveBetween(2000, 2100)
- EnrollmentType: MaxLength(50) (opcional)
```

#### 4. **AnnualFeeViewModelValidator**
```csharp
- EnrollmentId: GreaterThan(0)
- Amount: NotEmpty, GreaterThan(0), LessThanOrEqualTo(100000)
- DueDate: NotEmpty, GreaterThanOrEqualTo(Now - 365 dies)
```

## Configuració a Program.cs

```csharp
using FluentValidation;
using FluentValidation.AspNetCore;

// Registre FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<SchoolViewModelValidator>();
```

## Ús als Controllers

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(SchoolViewModel model)
{
    if (!ModelState.IsValid)
    {
        SetErrorMessage("Si us plau, omple tots els camps obligatoris.");
        return RedirectToAction(nameof(Index));
    }
    
    // Lògica de creació...
}
```

## Flux de Validació

1. **Client-side**: FluentValidation genera validació JavaScript automàticament
2. **Server-side**: Les regles s'executen abans d'arribar al controller action
3. **ModelState**: Es popula automàticament amb els errors de validació
4. **Controller**: Només verifica `ModelState.IsValid`

## Missatges d'Error Personalitzats

Tots els validators utilitzen missatges en català amb `.WithMessage()`:
- "El codi de l'escola és obligatori"
- "El nom no pot tenir més de 200 caràcters"
- "L'adreça d'email no és vàlida"

## Validació Condicional

S'utilitzen condicions `.When()` per camps opcionals:
```csharp
.MaximumLength(50)
.When(x => !string.IsNullOrEmpty(x.Scope))
```

## Testing

Els validators es poden testar de manera independent:
```csharp
var validator = new SchoolViewModelValidator();
var model = new SchoolViewModel { Code = "" };
var result = validator.Validate(model);
Assert.IsFalse(result.IsValid);
```

## Patrons Relacionats

- **Strategy Pattern**: Cada validator és una estratègia de validació
- **Chain of Responsibility**: Les regles s'encadenen
- **Template Method**: FluentValidation proporciona el template, nosaltres les regles

## SOLID Compliance

✅ **S**ingle Responsibility: Cada validator una responsabilitat  
✅ **O**pen/Closed: Obert per extensió, tancat per modificació  
✅ **L**iskov Substitution: Tots implementen AbstractValidator<T>  
✅ **I**nterface Segregation: Cada interface és petita i específica  
✅ **D**ependency Inversion: Dependem d'abstraccions (IValidator)
