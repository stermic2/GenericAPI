using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities;
using MediatR;
using WebstoreEntities.Entities;

namespace GenericApi.Handlers
{

    public interface ICreateCommand<TEntity> : IRequest<bool> where TEntity : Entity, new() { }

    public class CreateCommandHandler<TEntity, TCommand> : IRequestHandler<TCommand, bool>
        where TEntity : Entity, new()
        where TCommand : class, ICreateCommand<TEntity>, new()
    {
        //private readonly DatabaseContext context;
        private readonly IMapper mapper;

        public CreateCommandHandler(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public Task<bool> Handle(TCommand request, CancellationToken cancellationToken)
        {
            var entity = mapper.Map<TCommand, TEntity>(request);

           //var entities = context.Set<TEntity>().Add(entity);

            //var result = context.SaveChanges();

            //return Task.FromResult(result > 0);
            return Task.FromResult(false);
        }
    }
 }