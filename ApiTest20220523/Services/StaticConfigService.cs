using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest20220523.Services
{
    public static class StaticConfigService
    {
        public static IConfiguration Configuration { get; set; }
    }
}
