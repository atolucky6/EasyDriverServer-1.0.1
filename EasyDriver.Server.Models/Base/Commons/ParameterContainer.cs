using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EasyDriver.Core
{
    [Serializable]
    public class ParameterContainer : BindableCore, IParameterContainer
    {
        [ReadOnly(true)]
        [Display(AutoGenerateField = false)]
        public virtual string DisplayName { get => GetProperty<string>(); set => SetProperty(value); }

        [ReadOnly(true)]
        [Display(AutoGenerateField = false)]
        public virtual string DisplayParameters { get => GetProperty<string>(); set => SetProperty(value); }

        public Dictionary<string, string> Parameters { get; set; }

        [JsonConstructor]
        public ParameterContainer()
        {
            Parameters = new Dictionary<string, string>();
        }
    }
}
