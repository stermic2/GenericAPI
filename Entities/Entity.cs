using System.Collections.Generic;
using WebstoreEntities.Entities;
using WebstoreEntities.Interfaces;
using WebstoreEntities.Markers;

namespace Entities
{
    public class Entity : ISoftDelete, IAudited, IEntity
    {
        public string Id { get; set; }
        public List<StringProperty> StringProperties { get; set; }
        public List<IntProperty> IntProperties { get; set; }
    }
}