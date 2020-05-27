using Entities;
using WebstoreEntities.Interfaces;

namespace WebstoreEntities.Entities
{
    public class Property: IEntity
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public Entity Entity { get; set; }
    }
}