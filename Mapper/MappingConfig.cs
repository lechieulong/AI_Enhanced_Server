﻿using AutoMapper;
using Entity;
using Entity.Test;
using Model;
using Model.Test;
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
                config.CreateMap<Class, ClassDto>();
                config.CreateMap<ClassDto, Class>();
                config.CreateMap<ApplicationUser, UserDto>();
                config.CreateMap<UserDto, ApplicationUser>();
                config.CreateMap<EventDto, Event>();
                config.CreateMap<Event, EventDto>();
                config.CreateMap<TeacherAvailableSchedule, TeacherAvailableScheduleDto>();
                config.CreateMap<TeacherAvailableScheduleDto, TeacherAvailableSchedule>();
                config.CreateMap<TestExam, TestModel>();

                config.CreateMap<UserEducation, UserEducationDto>();
                config.CreateMap<UserEducationDto, UserEducation>();
                config.CreateMap<SpecializationDto, Specialization>();
                config.CreateMap<Specialization, SpecializationDto>();
            });
            return mappingConfig;
        }
    }
}
