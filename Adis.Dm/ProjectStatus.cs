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
    [JsonConverter(typeof(JsonStringEnumConverter<ProjectStatus>))]
    public enum ProjectStatus
    {
        /// <summary>
        /// Проектируется
        /// </summary>
        Designing,

        /// <summary>
        /// Поиск подрядчика
        /// </summary>
        ContractorSearch,

        /// <summary>
        /// Исполняется
        /// </summary>
        InExecution,

        /// <summary>
        /// Завершен
        /// </summary>
        Completed
    }
}
