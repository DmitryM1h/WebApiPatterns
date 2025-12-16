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

        public void ReceiveCommand(CommandBase command)
        {

            var commandType = command.GetType();

            TypeInfo handler;

            if (_cachedHandlers.TryGetValue(commandType, out var _handler))
            {
                handler = _handler;
            }
            else
            {
                var handlers = Assembly.GetExecutingAssembly()
                            .DefinedTypes
                            .Where(t => t.IsClass)
                            .Where(t =>
                                t.ImplementedInterfaces
                                .Any(x => x.Name == typeof(IJobHandler<>).Name && x.GenericTypeArguments.Contains(commandType))
                             ).ToList();

                if (handlers.Count == 0)
                    throw new HandlerNotFoundException($"Unable to resolve for command {commandType.Name}");

                if (handlers.Count > 1)
                    throw new MultipleHandlersException($"Multiple handlers for command {commandType.Name}");

                handler = handlers.First();

                _cachedHandlers.TryAdd(commandType, handler);
            }

            var handlertype = handler.AsType();

            var interfaceType = typeof(IJobHandler<>).MakeGenericType(commandType);

            var handlerInstance = Activator.CreateInstance(handlertype, [serviceProvider, command.UserName]);

            var method = interfaceType.GetMethod("ExecuteJob");

            var task = () => Task.Run(() => method!.Invoke(handlerInstance, [command]));

            _ = task();

        }
    }
}
