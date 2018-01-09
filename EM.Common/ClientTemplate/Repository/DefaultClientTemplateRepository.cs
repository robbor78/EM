﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Common.ClientTemplate.Repository
{
  public class DefaultClientTemplateRepository : IClientTemplateRepository
  {
    private Dictionary<string, IClientTemplate> templates = new Dictionary<string, IClientTemplate>();

    public void Add(string key, IClientTemplate template)
    {
      templates.Add(key, template);
    }

    public IClientTemplate Get(string key)
    {
      return templates[key];
    }
  }
}