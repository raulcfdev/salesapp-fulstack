using AutoMapper;
using SalesApp.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesApp.Tests.Helpers
{
    public static class MapperHelper
    {
        public static IMapper GetMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfiles());
            });

            return mapperConfig.CreateMapper();
        }
    }
}
