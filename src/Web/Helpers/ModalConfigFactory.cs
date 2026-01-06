using Web.Models;

namespace Web.Helpers;

public static class ModalConfigFactory
{
    public static EntityModalConfig GetSchoolModalConfig(List<SelectOption> scopeOptions)
    {
        return new EntityModalConfig
        {
            EntityName = "Escola",
            ModalId = "createSchoolModal",
            Controller = "Schools",
            IconClass = "bi-building",
            Fields = new List<ModalField>
            {
                new ModalField
                {
                    Name = "Code",
                    Label = "Codi",
                    Type = "text",
                    Required = true,
                    MaxLength = 10,
                    ColumnSize = 6,
                    Placeholder = "Ex: 08001234"
                },
                new ModalField
                {
                    Name = "Name",
                    Label = "Nom",
                    Type = "text",
                    Required = true,
                    MaxLength = 200,
                    ColumnSize = 6,
                    Placeholder = "Nom de l'escola"
                },
                new ModalField
                {
                    Name = "City",
                    Label = "Ciutat",
                    Type = "text",
                    Required = true,
                    MaxLength = 100,
                    ColumnSize = 6,
                    Placeholder = "Ciutat"
                },
                new ModalField
                {
                    Name = "Scope",
                    Label = "Àmbit",
                    Type = "select",
                    Required = false,
                    ColumnSize = 6,
                    Options = scopeOptions
                },
                new ModalField
                {
                    Name = "IsFavorite",
                    Label = "Favorita",
                    Type = "checkbox",
                    Required = false,
                    ColumnSize = 12
                }
            }
        };
    }

    public static EntityModalConfig GetStudentModalConfig(List<SelectOption> schoolOptions)
    {
        return new EntityModalConfig
        {
            EntityName = "Alumne",
            ModalId = "createStudentModal",
            Controller = "Students",
            IconClass = "bi-person",
            Fields = new List<ModalField>
            {
                new ModalField
                {
                    Name = "FirstName",
                    Label = "Nom",
                    Type = "text",
                    Required = true,
                    MaxLength = 100,
                    ColumnSize = 6,
                    Placeholder = "Nom de l'alumne"
                },
                new ModalField
                {
                    Name = "LastName",
                    Label = "Cognoms",
                    Type = "text",
                    Required = true,
                    MaxLength = 100,
                    ColumnSize = 6,
                    Placeholder = "Cognoms de l'alumne"
                },
                new ModalField
                {
                    Name = "Email",
                    Label = "Email",
                    Type = "email",
                    Required = true,
                    MaxLength = 255,
                    ColumnSize = 6,
                    Placeholder = "correu@exemple.cat"
                },
                new ModalField
                {
                    Name = "BirthDate",
                    Label = "Data de naixement",
                    Type = "date",
                    Required = true,
                    ColumnSize = 6
                },
                new ModalField
                {
                    Name = "SchoolId",
                    Label = "Escola",
                    Type = "select",
                    Required = true,
                    ColumnSize = 12,
                    Options = schoolOptions
                }
            }
        };
    }

    public static EntityModalConfig GetEnrollmentModalConfig(List<SelectOption> studentOptions)
    {
        return new EntityModalConfig
        {
            EntityName = "Inscripció",
            ModalId = "createEnrollmentModal",
            Controller = "Enrollments",
            IconClass = "bi-journal-text",
            Fields = new List<ModalField>
            {
                new ModalField
                {
                    Name = "StudentId",
                    Label = "Alumne",
                    Type = "select",
                    Required = true,
                    ColumnSize = 12,
                    Options = studentOptions
                },
                new ModalField
                {
                    Name = "AcademicYear",
                    Label = "Any acadèmic",
                    Type = "text",
                    Required = true,
                    ColumnSize = 12,
                    Placeholder = "Ex: 2024-2025"
                },
                new ModalField
                {
                    Name = "CourseName",
                    Label = "Nom del curs",
                    Type = "text",
                    Required = false,
                    ColumnSize = 12,
                    Placeholder = "Ex: 1r ESO, 2n Batxillerat, etc."
                },
                new ModalField
                {
                    Name = "Status",
                    Label = "Estat",
                    Type = "select",
                    Required = true,
                    ColumnSize = 12,
                    Options = new List<SelectOption>
                    {
                        new SelectOption { Value = "Activa", Text = "Activa" },
                        new SelectOption { Value = "Pendent", Text = "Pendent" },
                        new SelectOption { Value = "Cancel·lada", Text = "Cancel·lada" }
                    }
                }
            }
        };
    }

    public static EntityModalConfig GetAnnualFeeModalConfig(List<SelectOption> enrollmentOptions)
    {
        return new EntityModalConfig
        {
            EntityName = "Quota Anual",
            ModalId = "createAnnualFeeModal",
            Controller = "AnnualFees",
            IconClass = "bi-currency-euro",
            Fields = new List<ModalField>
            {
                new ModalField
                {
                    Name = "EnrollmentId",
                    Label = "Matrícula",
                    Type = "select",
                    Required = true,
                    ColumnSize = 12,
                    Options = enrollmentOptions
                },
                new ModalField
                {
                    Name = "Amount",
                    Label = "Import (€)",
                    Type = "number",
                    Required = true,
                    ColumnSize = 6,
                    Placeholder = "Ex: 150.00"
                },
                new ModalField
                {
                    Name = "DueDate",
                    Label = "Data de venciment",
                    Type = "date",
                    Required = true,
                    ColumnSize = 6
                },
                new ModalField
                {
                    Name = "IsPaid",
                    Label = "Pagada",
                    Type = "checkbox",
                    Required = false,
                    ColumnSize = 12
                }
            }
        };
    }
}
