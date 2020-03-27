using Autofac;
using MediatR;
using Module = Autofac.Module;

namespace Mappers
{
    public class MapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(MapperModule).Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>)).AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterAssemblyTypes(typeof(MapperModule).Assembly).AsClosedTypesOf(typeof(IRequestHandler<>)).AsImplementedInterfaces().InstancePerDependency();
        }
    }
}