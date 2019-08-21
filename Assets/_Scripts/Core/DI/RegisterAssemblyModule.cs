using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Autofac;
using Core.Attributes;
using Examples;
using UI;

namespace Core.DI
{
  /// <summary>
  /// Class responsible for registration all the dependencies.
  /// </summary>
  public static class RegisterAssemblyModule
  {
    public static void Load(ContainerBuilder builder)
    {
#if UNITY_ANDROID || UNITY_IPHONE
      LoadCommonTypes(builder);
      LoadViewModels(builder);
#else
      StandaloneRegistration(builder);      
#endif
    }

    private static void LoadCommonTypes(ContainerBuilder builder)
    {
      builder.Register(c => new CoreMainThreadActionsQueue())
        .As<CoreMainThreadActionsQueue>()
        .Keyed<object>(typeof(CoreMainThreadActionsQueue).FullName)
        .SingleInstance();

      builder.Register(c => new Resources.ResourcesManagement.AssetBundleManager())
        .As<Resources.ResourcesManagement.AssetBundleManager>()
        .Keyed<object>(typeof(Resources.ResourcesManagement.AssetBundleManager).FullName)
        .SingleInstance();

      builder.Register(c => new GameTranslator())
        .As<GameTranslator>()
        .Keyed<object>(typeof(GameTranslator).FullName)
        .SingleInstance();
    }

    private static void LoadViewModels(ContainerBuilder builder)
    {
      builder.Register(c => new UI.PopupViewModel(c.Resolve<UI.GameTranslator>()))
        .As<UI.PopupViewModel>()
        .Keyed<object>(typeof(UI.PopupViewModel).FullName)
        .InstancePerDependency();

      builder.Register(c => new Examples.MainMenuViewModel(c.Resolve<GameTranslator>()))
        .As<Examples.MainMenuViewModel>()
        .Keyed<object>(typeof(Examples.MainMenuViewModel).FullName)
        .InstancePerDependency();

      builder.Register(c => new ExampleDataSourceViewModel())
        .As<ExampleDataSourceViewModel>()
        .Keyed<object>(typeof(ExampleDataSourceViewModel).FullName)
        .InstancePerDependency();
    }

    /// <summary>
    /// Fully automated registration. IDisposable is added manually because sometimes this interface is ignored by AsImplementedInterfaces function.
    /// This registration is not supported on mobile devices because resolving such instance is done by dynamic constructor compilation.
    /// </summary>
    [Conditional("UNITY_STANDALONE")]
    private static void StandaloneRegistration(ContainerBuilder builder)
    {
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        IEnumerable<Type> typesToRegister;
        try
        {
          typesToRegister = assembly.GetTypes().Where(t => t.IsDefined(typeof(CoreRegisterAttribute)));
        }
        catch
        {
          // Ignore types loading exception
          continue;
        }

        foreach (Type t in typesToRegister)
        {
          bool singletone = t.GetCustomAttribute<CoreRegisterAttribute>(true).IsSingletone;
          if (singletone)
          {
            if (t.GetInterfaces().Contains(typeof(IDisposable)))
            {
              builder.RegisterType(t)
                .AsSelf()
                .AsImplementedInterfaces()
                .As<IDisposable>()
                .Keyed<object>(t.FullName)
                .SingleInstance();
            }
            else
            {
              builder.RegisterType(t)
                .AsSelf()
                .AsImplementedInterfaces()
                .Keyed<object>(t.FullName)
                .SingleInstance();
            }
          }
          else
          {
            if (t.GetInterfaces().Contains(typeof(IDisposable)))
            {
              builder.RegisterType(t)
                .AsSelf()
                .AsImplementedInterfaces()
                .As<IDisposable>()
                .Keyed<object>(t.FullName);
            }
            else
            {
              builder.RegisterType(t)
                .AsSelf()
                .AsImplementedInterfaces()
                .Keyed<object>(t.FullName);
            }
          }
        }
      }
    }
  }  
}
