using StronglyTypedIdDemo.Infrastructure;

namespace StronglyTypedIdDemo.Data
{

    public record OrderId(int Value) : StronglyTypedId<int>(Value);

    public record ProductId : OrderId
    {
        public ProductId(int value) : base(value)
        {

        }
    }
}
