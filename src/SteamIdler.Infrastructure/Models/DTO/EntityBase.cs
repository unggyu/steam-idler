using SteamIdler.Infrastructure.Interfaces;
using System;

namespace SteamIdler.Infrastructure.Models
{
    public abstract class EntityBase<TKey> : Bindable, IKey<TKey>, ICloneable where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
    {
        private TKey _id;

        public TKey Id
        {
            get => _id;
            set => SetValue(ref _id, value);
        }

        public abstract object Clone();
    }
}
