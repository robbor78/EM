﻿using Common.Logging;
using EM.Common.Client;
using EM.Common.Client.Factory;
using EM.Common.Client.Repository;
using EM.Common.Client.Template;
using EM.Common.Client.Template.Repository;
using EM.Common.Plugin;
using EM.Common.Plugin.Template.Repository;
using EM.Common.PluginTemplate;
using EM.Common.PluginTemplate.Repository;
using EM.Common.Utils;
using System;
using System.Linq;
using System.Reflection;

namespace EM.Client.Factory
{
  [Serializable]
  public class DefaultClientFactory : IClientFactory
  {
    private ILog logger = LogManager.GetLogger<DefaultClientFactory>();
    private IIoCFactory iocFactory;
 
    public DefaultClientFactory(IIoCFactory iocFactory) => (this.iocFactory) = (iocFactory);

    public IClient MakeClient(IClientTemplate template)
    {
      Type t = GetPluginType(template.PluginTemplate);

      IPlugin plugin = (IPlugin)Activator.CreateInstance(t);
      plugin.Properties = template.Properties.Properties;
      GetPropertiesFromIoC(plugin);

      DefaultClient client = new DefaultClient()
      {
        Plugin = plugin,
        Properties = GetClientProperties(template),
        Schedule = GetClientSchedule(template),
        Status = GetClientStatus(template)
      };

      return client;
    }

    private Type GetPluginType(IPluginTemplate template)
    {
      Assembly SampleAssembly;
      SampleAssembly = Assembly.LoadFrom(template.DLLName);
      return SampleAssembly.GetTypes().Where(x => x.FullName == template.FullClassName && x.GetInterfaces().Contains(typeof(IPlugin))).FirstOrDefault();
    }

    private void GetPropertiesFromIoC(IPlugin plugin)
    {
      Type t = plugin.GetType();
      foreach (PropertyInfo prop in t.GetProperties())
      {
        try
        {
          Type propType = prop.PropertyType;
          prop.SetValue(plugin, iocFactory.GetInstance(propType)); //TODO Memory leak? Use "using"?
        }
        catch (Exception e)
        {
          logger.Warn("Failed to populated property ("+prop.Name+") using reflection. This could be an error.", e);
        }
      }
    }

    private ClientStatus GetClientStatus(IClientTemplate template)
    {
      return template.Status.Clone();
    }

    private ClientSchedule GetClientSchedule(IClientTemplate template)
    {
      return template.Schedule.Clone();
    }

    private ClientProperties GetClientProperties(IClientTemplate template)
    {
      return template.Properties.Clone();
    }

    private IClientRepository GetClientRepository()
    {
      return iocFactory.GetInstance<IClientRepository>();
    }
  }
}
