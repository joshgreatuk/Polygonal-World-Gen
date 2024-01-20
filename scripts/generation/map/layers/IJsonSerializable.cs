using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map.layers
{
    public interface IJsonSerializable
    {
        public string ToJson();
        public object FromJson(string json);
    }
}
