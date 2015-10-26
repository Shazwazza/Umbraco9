using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Umbraco.ViewModels
{
    public class UmbracoFormTest
    {
        [Required]
        public string Name { get; set; }
    }
}
