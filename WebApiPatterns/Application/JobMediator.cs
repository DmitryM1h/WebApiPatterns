using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using WebApiPatterns.Exceptions;
using WebApiPatterns.Interfaces;

namespace WebApiPatterns.Application
{
    public class JobMediator(IServiceProvider serviceProvider)
    {
        private static readonly ConcurrentDictionary<Type, TypeInfo> _cachedHandlers = new();

        public async Task ReceiveCommand(CommandBase command)
        {

            var type = command.GetType();

            TypeInfo handler;

            if (_cachedHandlers.TryGetValue(type, out var _handler))
            {
                handler = _handler;
                return;
            }
            else
            {
                var handlers = Assembly.GetExecutingAssembly()
                            .DefinedTypes
                            .Where(t => t.IsClass)
                            .Where(t =>
                                t.ImplementedInterfaces
                                .Any(x => x.Name == typeof(IJobHandler<>).Name && x.GenericTypeArguments.Contains(type))
                             ).ToList();

                if (handlers.Count == 0)
                    throw new HandlerNotFoundException($"Unable to resolve for command {type.Name}");

                if (handlers.Count > 1)
                    throw new MultipleHandlersException($"Multiple handlers for command {type.Name}");

                handler = handlers.First();

                _cachedHandlers.TryAdd(type, handler);
            }

            var handlertype = handler.AsType();

            var interfaceType = typeof(IJobHandler<>).MakeGenericType(type);

            var handlerInstance = Activator.CreateInstance(handlertype, serviceProvider);

            var method = interfaceType.GetMethod("ExecuteJob");

            var task = () => Task.Run(() => method!.Invoke(handlerInstance,[command]));

            _ = task();

        }
    }
}
