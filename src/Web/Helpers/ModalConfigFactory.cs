using Web.Models;

namespace Web.Helpers;

public static class ModalConfigFactory
{
    public static EntityModalConfig GetSchoolModalConfig()
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
                    Options = new List<SelectOption>
                    {
                        new SelectOption { Value = "Urbà", Text = "Urbà" },
                        new SelectOption { Value = "Rural", Text = "Rural" },
                        new SelectOption { Value = "Semiurbà", Text = "Semiurbà" }
                    }
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
                    Name = "BirthDate",
                    Label = "Data de naixement",
                    Type = "date",
                    Required = false,
                    ColumnSize = 6
                },
                new ModalField
                {
                    Name = "SchoolId",
                    Label = "Escola",
                    Type = "select",
                    Required = true,
                    ColumnSize = 6,
                    Options = schoolOptions
                }
            }
        };
    }

    public static EntityModalConfig GetEnrollmentModalConfig(List<SelectOption> studentOptions)
    {
        return new EntityModalConfig
        {
            EntityName = "Matrícula",
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
                    ColumnSize = 6,
                    Options = studentOptions
                },
                new ModalField
                {
                    Name = "Year",
                    Label = "Any",
                    Type = "number",
                    Required = true,
                    ColumnSize = 4,
                    Placeholder = "Ex: 2024"
                },
                new ModalField
                {
                    Name = "CourseName",
                    Label = "Curs",
                    Type = "select",
                    Required = false,
                    ColumnSize = 4,
                    Options = new List<SelectOption>
                    {
                        new SelectOption { Value = "1r Infantil", Text = "1r Infantil" },
                        new SelectOption { Value = "2n Infantil", Text = "2n Infantil" },
                        new SelectOption { Value = "3r Infantil", Text = "3r Infantil" },
                        new SelectOption { Value = "1r Primària", Text = "1r Primària" },
                        new SelectOption { Value = "2n Primària", Text = "2n Primària" },
                        new SelectOption { Value = "3r Primària", Text = "3r Primària" },
                        new SelectOption { Value = "4t Primària", Text = "4t Primària" },
                        new SelectOption { Value = "5è Primària", Text = "5è Primària" },
                        new SelectOption { Value = "6è Primària", Text = "6è Primària" },
                        new SelectOption { Value = "1r ESO", Text = "1r ESO" },
                        new SelectOption { Value = "2n ESO", Text = "2n ESO" },
                        new SelectOption { Value = "3r ESO", Text = "3r ESO" },
                        new SelectOption { Value = "4t ESO", Text = "4t ESO" },
                        new SelectOption { Value = "1r Batxillerat", Text = "1r Batxillerat" },
                        new SelectOption { Value = "2n Batxillerat", Text = "2n Batxillerat" }
                    }
                },
                new ModalField
                {
                    Name = "EnrollmentType",
                    Label = "Tipus",
                    Type = "select",
                    Required = false,
                    ColumnSize = 4,
                    Options = new List<SelectOption>
                    {
                        new SelectOption { Value = "Ordinària", Text = "Ordinària" },
                        new SelectOption { Value = "Extraordinària", Text = "Extraordinària" },
                        new SelectOption { Value = "Trasllat", Text = "Trasllat" }
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
