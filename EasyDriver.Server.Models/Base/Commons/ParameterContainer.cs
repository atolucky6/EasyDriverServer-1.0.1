using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EasyDriver.Server.Models
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

        public Dictionary<string, object> Parameters { get; set; }

        public ParameterContainer()
        {
            Parameters = new Dictionary<string, object>();
        }
    }
}
