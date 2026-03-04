using Domain.DomainExceptions;

namespace Domain.ValueObjects;

public readonly record struct MoneyAmount
{
    public decimal Value { get; }

    private MoneyAmount(decimal value)
    {
        Value = value;
    }

    public static MoneyAmount Create(decimal amount, string propertyName = "Amount")
    {
        if (amount <= 0)
        {
            throw new ValidationException(propertyName, "L'import ha de ser superior a 0");
        }

        return new MoneyAmount(decimal.Round(amount, 2, MidpointRounding.AwayFromZero));
    }

    public override string ToString() => Value.ToString("0.00");
}
