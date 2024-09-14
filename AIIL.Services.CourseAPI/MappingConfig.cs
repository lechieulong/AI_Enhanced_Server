using AIIL.Services.CourseAPI.Models;
using AIIL.Services.CourseAPI.Models.Dto;
using AutoMapper;

namespace AIIL.Services.CourseAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CourseDto, Course>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
