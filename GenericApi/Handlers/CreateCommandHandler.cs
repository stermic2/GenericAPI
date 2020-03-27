using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using WebstoreEntities.Entities;

namespace GenericApi.Handlers
{

    public interface ICreateCommand<TEntity> : IRequest<bool> where TEntity : Entity, new() { }

    public class CreateCommandHandler<TEntity, TCommand> : IRequestHandler<TCommand, bool>
        where TEntity : Entity, new()
        where TCommand : class, ICreateCommand<TEntity>, new()
    {
        private readonly DatabaseContext context;
        private readonly IMapper mapper;

        public CreateCommandHandler(DatabaseContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public Task<bool> Handle(TCommand request, CancellationToken cancellationToken)
        {
            var entity = mapper.Map<TCommand, TEntity>(request);

            var entities = context.Set<TEntity>().Add(entity);

            var result = context.SaveChanges();

            return Task.FromResult(result > 0);
        }
    }
 }