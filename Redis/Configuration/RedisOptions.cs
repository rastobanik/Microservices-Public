using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Redis.Configuration
{
    public class RedisOptions
    {
        public const string Section = "Redis";

        [Required]
        public string Configuration { get; set; } = string.Empty;

        [Required]
        public int ConnectRetry { get; set; } = 3;

        [Required]
        public int ReconnectTimeOut { get; set; } = 5000;
    }
}
