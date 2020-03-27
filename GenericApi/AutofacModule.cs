using Autofac;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using GenericApi;
using Mappers;
using MediatR;

namespace WebstoreApi
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Uncomment to enable polymorphic dispatching of requests, but note that
            // this will conflict with generic pipeline behaviors
            // builder.RegisterSource(new ContravariantRegistrationSource());

            // Mediator itself
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            // request & notification handlers
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

// finally register our custom code (individually, or via assembly scanning)
// - requests & handlers as transient, i.e. InstancePerDependency()
// - pre/post-processors as scoped/per-request, i.e. InstancePerLifetimeScope()
// - behaviors as transient, i.e. InstancePerDependency()
//builder.RegisterType<MyHandler>().AsImplementedInterfaces().InstancePerDependency(); 
            
            var mapperCfg = new MapperConfiguration(cfg =>
            {
                cfg.AddCollectionMappers();
                cfg.AddMaps(typeof(MapperModule).Assembly);
            });
            builder.Register(c =>
                {
                    var mapper = mapperCfg.CreateMapper();
                        

                    return mapper;
                }).AsImplementedInterfaces().InstancePerLifetimeScope()
                .Named<IMapper>(nameof(WebstoreApi));
        }
    }
}