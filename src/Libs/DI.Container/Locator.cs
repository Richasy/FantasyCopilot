// Copyright (c) Fantasy Copilot. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace FantasyCopilot.DI.Container;

/// <summary>
/// Dependency Injection Locator.
/// </summary>
public sealed class Locator
{
    private static readonly Lazy<Locator> _lazyLocator = new(() => new Locator());
    private readonly IDictionary<Type, object> _variables;
    private readonly ServiceCollection _serviceCollection;
    private ServiceProvider _serviceProvider;
    private bool _hasBuild;

    /// <summary>
    /// Initializes a new instance of the <see cref="Locator"/> class.
    /// </summary>
    private Locator()
    {
        _serviceCollection = new ServiceCollection();
        _variables = new Dictionary<Type, object>();
    }

    /// <summary>
    /// Global variable changed.
    /// </summary>
    public event EventHandler<Type> VariableChanged;

    /// <summary>
    /// Locator instance.
    /// </summary>
    public static Locator Current => _lazyLocator.Value;

    /// <summary>
    /// Register a singleton implementation.
    /// </summary>
    /// <typeparam name="TInterface">Singleton interface.</typeparam>
    /// <typeparam name="TImplementation">Singleton implementation.</typeparam>
    /// <returns>Locator.</returns>
    public Locator RegisterSingleton<TInterface, TImplementation>()
        where TInterface : class
        where TImplementation : class, TInterface
    {
        _serviceCollection.AddSingleton<TInterface, TImplementation>();
        return this;
    }

    /// <summary>
    /// Register transient implementation.
    /// </summary>
    /// <typeparam name="TInterface">Transient interface.</typeparam>
    /// <typeparam name="TImplementation">Transient implementation.</typeparam>
    /// <returns>Locator.</returns>
    public Locator RegisterTransient<TInterface, TImplementation>()
        where TInterface : class
        where TImplementation : class, TInterface
    {
        _serviceCollection.AddTransient<TInterface, TImplementation>();
        return this;
    }

    /// <summary>
    /// Register variable.
    /// </summary>
    /// <param name="type">Variable type.</param>
    /// <param name="instance">Variable object.</param>
    /// <returns>Locator.</returns>
    public Locator RegisterVariable(Type type, object instance)
    {
        if (_variables.ContainsKey(type))
        {
            _variables[type] = instance;
            VariableChanged?.Invoke(this, type);
        }
        else
        {
            _variables.Add(type, instance);
        }

        return this;
    }

    /// <summary>
    /// Register the logger.
    /// </summary>
    /// <param name="path">Log path.</param>
    /// <returns>Current instance.</returns>
    public Locator RegisterLogger(string path)
    {
        NLog.GlobalDiagnosticsContext.Set("LogPath", path);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        _serviceCollection.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddNLog();
        });

        return this;
    }

    /// <summary>
    /// Build a service provider to enable registered services to take effect.
    /// </summary>
    public void Build()
    {
        if (_hasBuild)
        {
            throw new InvalidOperationException("A service provider cannot be build twice.");
        }

        _serviceProvider = _serviceCollection.BuildServiceProvider();
        _hasBuild = true;
    }

    /// <summary>
    /// Get registered services.
    /// </summary>
    /// <typeparam name="T">The service registry type.</typeparam>
    /// <returns>registered service.</returns>
    public T GetService<T>()
        where T : class
        => _serviceProvider.GetRequiredService<T>();

    /// <summary>
    /// Get logger service.
    /// </summary>
    /// <typeparam name="T">Logger type.</typeparam>
    /// <returns>Logger.</returns>
    public ILogger<T> GetLogger<T>()
        => _serviceProvider.GetRequiredService<ILogger<T>>();

    /// <summary>
    /// Get registered services.
    /// </summary>
    /// <param name="typeName">Service type.</param>
    /// <typeparam name="T">The type to be converted.</typeparam>
    /// <returns>registered service.</returns>
    public T GetService<T>(Type typeName)
        where T : class
        => (T)_serviceProvider.GetRequiredService(typeName);

    /// <summary>
    /// Get registered variable object.
    /// </summary>
    /// <typeparam name="T">Variable type.</typeparam>
    /// <param name="type">Variable id.</param>
    /// <returns>Variable.</returns>
    public T GetVariable<T>()
        => _variables.TryGetValue(typeof(T), out var value)
            ? (T)value
            : default;
}
