using System;

namespace GT.Agreement.Entity
{
    internal interface IEntity<T> : IMiniEntity<T>
    {
        DateTimeOffset UpdatedTime { get; set; }
    }
}
