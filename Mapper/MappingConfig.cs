using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                //Register mapper here nhe:))
                //config.CreateMap<CouponDto, Coupon>();
                //config.CreateMap<Coupon, CouponDto>();
            });
            return mappingConfig;
        }
    }
}
