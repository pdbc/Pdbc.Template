using System;
using Aertssen.Framework.Core.Model;

namespace MyTemplate.Domain.Model
{
    public class MyEntity : BaseEquatableEntity<MyEntity>
    {

        public String Name { get; set; }
    }
}