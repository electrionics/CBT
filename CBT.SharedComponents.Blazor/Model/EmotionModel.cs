using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.SharedComponents.Blazor.Model
{
    public class EmotionModel
    {
        public int Key { get; set; }

        public string Name { get; set; }

        public bool Positive { get; set; }

        public string CssClass
        {
            get
            {
                return Positive ? "oi oi-plus" : "oi oi-minus";
            }
        }
    }
}
