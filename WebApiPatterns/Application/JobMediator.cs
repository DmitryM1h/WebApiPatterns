using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using WebApiPatterns.Exceptions;
using WebApiPatterns.Interfaces;

namespace WebApiPatterns.Application
{
    public class JobMediator
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> _cachedHandlers = new();

        public async Task ReceiveCommand(CommandBase command)
        {

            var type = command.GetType();

            if(_cachedHandlers.TryGetValue(type, out var _handler))
            {
                _handler();
                return;
            }    

            var handlers = Assembly.GetExecutingAssembly()
                        .DefinedTypes
                        .Where(t => t.IsClass)
                        .Where(t =>
                            t.ImplementedInterfaces
                            .Any(x => x.Name == typeof(IJobHandler<>).Name && x.GenericTypeArguments.Contains(type))
                         ).ToList();

            if(handlers.Count == 0)
                throw new HandlerNotFoundException($"Unable to resolve for command {type.Name}");

            if (handlers.Count > 1)
                throw new MultipleHandlersException($"Multiple handlers for command {type.Name}");

            var handler = handlers.First();

            var handlertype = handler.AsType();

            var interfaceType = typeof(IJobHandler<>).MakeGenericType(type);

            var handlerInstance = Activator.CreateInstance(handlertype);

            var method = interfaceType.GetMethod("ExecuteJob");

            var task = () => method!.Invoke(handlerInstance, null);

            _cachedHandlers.TryAdd(type, task);

            task();

        }
    }
}
