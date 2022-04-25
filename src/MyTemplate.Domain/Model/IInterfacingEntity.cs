using System;
using Aertssen.Framework.Core.Model;

namespace MyTemplate.Domain.Model
{
    public interface IInterfacingEntity : IExternallyIdentifiable
    {
        DateTimeOffset DateModified { get; set; }
    }
}