using System;

namespace SteamIdler.Infrastructure.Interfaces
{
    public interface IKey<T> where T : IComparable, IComparable<T>, IEquatable<T>
    {
        T Id { get; }
    }
}
