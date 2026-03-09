using Domain.DomainExceptions;

namespace Domain.ValueObjects;

public readonly record struct MoneyAmount
{
    public decimal Value { get; }
        /// <summary>
        /// Initializes a new instance of the MoneyAmount class with its required dependencies.
        /// </summary>
        private MoneyAmount(decimal value)
    {
        Value = value;
    }
        /// <summary>
        /// Creates a new resource by applying the required business rules.
        /// </summary>
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
