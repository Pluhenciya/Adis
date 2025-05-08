using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Adis.Dm
{
    [JsonConverter(typeof(JsonStringEnumConverter<Role>))]
    public enum Role
    {
        Admin,
        Projecter,
        ProjectManager
    }
}
