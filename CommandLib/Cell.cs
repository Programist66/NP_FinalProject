using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectLib
{
    public class Cell : BindableBase
    {
        public string Value
        { 
            get; 
            set => SetProperty(ref field, value); 
        }

        public Cell(string value)
        {
            Value = value;
        }
    }
}
