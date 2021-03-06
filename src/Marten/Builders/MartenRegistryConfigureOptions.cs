using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marten;
using Marten.Events.Projections;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Extensions.Marten.Projections;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.Marten.Builders
{
    class MartenRegistryConfigureOptions : IConfigureOptions<StoreOptions>
    {
        private readonly IEnumerable<MartenRegistry> _martenRegistries;

        public MartenRegistryConfigureOptions(IEnumerable<MartenRegistry> martenRegistries)
        {
            _martenRegistries = martenRegistries;
        }

        public void Configure(StoreOptions options)
        {
            foreach (var registry in _martenRegistries)
            {
                options.Schema.Include(registry);
            }
        }
    }

    class MartenProjectionsConfigureOptions : IConfigureOptions<StoreOptions>
    {
        private static readonly MethodInfo _AddTransform = typeof(MartenProjectionsConfigureOptions)
            .GetTypeInfo()
            .GetDeclaredMethod(nameof(AddTransform));
        private static readonly MethodInfo _AddAggregate = typeof(MartenProjectionsConfigureOptions)
            .GetTypeInfo()
            .GetDeclaredMethod(nameof(AddAggregate));
        private readonly ProjectionDescriptorCollection _projectionDescriptorCollection;
        private readonly IEnumerable<IInlineProjection> _inlineProjections;
        private readonly IEnumerable<IAsyncProjection> _asyncProjections;

        public MartenProjectionsConfigureOptions(
            ProjectionDescriptorCollection projectionDescriptorCollection,
            IEnumerable<IInlineProjection> inlineProjections,
            IEnumerable<IAsyncProjection> asyncProjections)
        {
            this._projectionDescriptorCollection = projectionDescriptorCollection;
            _inlineProjections = inlineProjections;
            _asyncProjections = asyncProjections;
        }

        public void Configure(StoreOptions options)
        {
            foreach (var projection in _inlineProjections)
            {
                InferProjectionType(options.Events.InlineProjections, projection);
            }

            foreach (var projection in _asyncProjections)
            {
                InferProjectionType(options.Events.AsyncProjections, projection);
            }

            foreach (var assembly in _projectionDescriptorCollection.GetProjectionDescriptors())
            {
                var collection = assembly.ProjectionType == ProjectionType.Inline ? options.Events.InlineProjections : options.Events.AsyncProjections;
                InferProjectionType(collection, assembly.Type);
            }
        }

        private void InferProjectionType(ProjectionCollection projections, object instance)
        {
            var type = instance.GetType().GetTypeInfo();
            if (instance is IProjection projection)
            {
                projections.Add(projection);
            }
            else if (type.GetInterfaces()
              .Any(z => z.GetTypeInfo().IsGenericType && z.GetTypeInfo().GetGenericTypeDefinition() == typeof(ITransform<,>)))
            {
                foreach (var @interface in type.GetInterfaces()
                    .Where(z => z.GetTypeInfo().IsGenericType && z.GetTypeInfo().GetGenericTypeDefinition() == typeof(ITransform<,>)))
                {
                    var tEvent = @interface.GetGenericArguments()[0];
                    var tView = @interface.GetGenericArguments()[1];
                    _AddTransform.MakeGenericMethod(tEvent, tView).Invoke(null, new[] { projections, instance });
                }
            }
            else
            {
                throw new NotSupportedException($"The project instance of type '{type.FullName}' is not a supported projection or transform.");
            }
        }

        private void InferProjectionType(ProjectionCollection projections, Type type)
        {
            if (typeof(IProjection).IsAssignableFrom(type))
            {
                try
                {
                    projections.Add((IProjection)Activator.CreateInstance(type));
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Could not create an instance of '{type.FullName}'. If you need to register it through Dependency Injection use the IInlineProjection or IAsyncProjection interfaces.");
                }
            }
            else if (type.GetInterfaces()
              .Any(z => z.GetTypeInfo().IsGenericType && z.GetTypeInfo().GetGenericTypeDefinition() == typeof(ITransform<,>)))
            {
                object instance;
                try
                {
                    instance = Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Could not create an instance of '{type.FullName}'. If you need to register it through Dependency Injection use the IInlineProjection or IAsyncProjection interfaces.");
                }

                foreach (var @interface in type.GetInterfaces()
                    .Where(z => z.GetTypeInfo().IsGenericType && z.GetTypeInfo().GetGenericTypeDefinition() == typeof(ITransform<,>)))
                {
                    var tEvent = @interface.GetGenericArguments()[0];
                    var tView = @interface.GetGenericArguments()[1];
                    _AddTransform.MakeGenericMethod(tEvent, tView).Invoke(null, new[] { projections, instance });
                }
            }
            else
            {
                _AddAggregate.MakeGenericMethod(type).Invoke(null, new object[] { projections });
            }
        }

        private static void AddTransform<TEvent, TView>(ProjectionCollection projections, ITransform<TEvent, TView> transform)
        {
            projections.TransformEvents(transform);
        }

        private static void AddAggregate<T>(ProjectionCollection projections)
            where T : class, new()
        {
            projections.AggregateStreamsWith<T>();
        }
    }
}
