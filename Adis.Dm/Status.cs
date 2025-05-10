using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Adis.Dm
{
    [JsonConverter(typeof(JsonStringEnumConverter<Status>))]
    public enum Status
    {
        ToDo,
        Doing,
        Checking,
        Completed
    }
}
