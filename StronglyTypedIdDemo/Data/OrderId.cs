using StronglyTypedIdDemo.Infrastructure;

namespace StronglyTypedIdDemo.Data
{

    public record OrderId(int Value) : StronglyTypedId<int>(Value);
}
