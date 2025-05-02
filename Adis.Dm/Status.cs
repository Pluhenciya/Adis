using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Adis.Dm
{
    /// <summary>
    /// Статус
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<Status>))]
    public enum Status
    {
        /// <summary>
        /// Черновик
        /// </summary>
        Draft,
        /// <summary>
        /// Выполняется
        /// </summary>
        InProgress,
        /// <summary>
        /// Завершен
        /// </summary>
        Completed,
        /// <summary>
        /// Просрочен
        /// </summary>
        Overdue
    }
}
