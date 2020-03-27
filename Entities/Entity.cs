using WebstoreEntities.Interfaces;
using WebstoreEntities.Markers;

namespace WebstoreEntities.Entities
{
    public class Entity : ISoftDelete, IAudited, IEntity
    {
        public string Id { get; set; }
    }
}