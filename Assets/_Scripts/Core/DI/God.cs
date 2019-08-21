using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Core.Attributes;

namespace Core.DI
{
  public static class God
  {
    private static IContainer _container;

    private static IContainer Container => _container ?? (_container = WorldCreation());

    public static IContainer WorldCreation()
    {
      try
      {
        ContainerBuilder builder = new ContainerBuilder();
        RegisterAssemblyModule.Load(builder);

        var container = builder.Build();

        // In unity editor validate that all the types requested for registration by CoreRegisterAttribute are already registered.
#if UNITY_EDITOR
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          foreach (Type type in assembly.GetTypes().Where(t => t.IsDefined(typeof(CoreRegisterAttribute))))
          {
            if (!container.TryResolve(type, out var _))
            {
              UnityEngine.Debug.LogError($"Class '{type.Name}' marked with CoreRegister attribute is not registered.");
            }
          }
        }
#endif
        return container;
      }
      catch (Exception e)
      {
        throw new Exception("[AUTOFAC] \n" +
                            "Exception occur during WorldCreation. \n" +
                            $"{e.Message} \n" +
                            "For more information - catch stack.");
      }
    }

    /// <summary>
    /// Generic types resolver.
    /// </summary>
    /// <typeparam name="T">Type of registered object.</typeparam>
    /// <returns>Instance.</returns>
    public static T PrayFor<T>()
    {
      return Container.Resolve<T>();
    }

    /// <summary>
    /// Gets registered object instance by type.
    /// This function is used by internal dependency injection automatization for game objects. 
    /// For normal case use <see cref="PrayFor{T}"/>.
    /// </summary>
    /// <param name="type">Requested type.</param>
    /// <returns>Instance.</returns>
    public static object PrayFor(Type type)
    {
      return Container.ResolveNamed<object>(type.FullName);
    }
  }
}
