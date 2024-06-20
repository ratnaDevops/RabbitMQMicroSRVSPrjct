using AutoMapper;
using MicroserviceSamplePrjct.Services.CouponAPI.Models;
using MicroserviceSamplePrjct.Services.CouponAPI.Models.Dto;

namespace MicroserviceSamplePrjct.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>();

                config.CreateMap<Coupon, CouponDto>();
            });
            return mappingConfig;
        }
    }
}
