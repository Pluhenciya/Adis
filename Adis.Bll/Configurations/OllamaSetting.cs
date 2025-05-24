using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Configurations
{
    public class OllamaSetting
    {
        public string OllamaUrl { get; set; } = null!;

        public string EmbeddingModel { get; set; } = null!;

        public string LlmModel { get; set; } = null!;
    }
}
