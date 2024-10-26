using AutoMapper;
using Entity;
using Entity.Live;
using Entity.Test;
using Model;
using Model.Live;
using Model.Payment;
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
                config.CreateMap<QuestionDto, Question>()
 .ForMember(dest => dest.Answers, opt => opt.Ignore()); // Ignore Answers during mapping, handled later

                config.CreateMap<Question, QuestionResponse>()
               .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers)); // Ensure answers are mapped

                config.CreateMap<Answer, AnswerResponse>(); // Ensure Answer is mapped to AnswerResponse

                config.CreateMap<UserEducation, UserEducationDto>();
                config.CreateMap<UserEducationDto, UserEducation>();
                config.CreateMap<SpecializationDto, Specialization>();
                config.CreateMap<Specialization, SpecializationDto>();
                config.CreateMap<TeacherRequest, TeacherRequestDto>();
                config.CreateMap<TeacherRequestDto, TeacherRequest>();

                config.CreateMap<StreamSession, StreamSessionModel>();
                config.CreateMap<StreamSessionModel, StreamSessionModel>();
                config.CreateMap<Ticket, TicketModel>();
                config.CreateMap<TicketModel, Ticket>();
                config.CreateMap<Transaction, TransactionModel>();
                config.CreateMap<TransactionModel, Transaction>();

            });
            return mappingConfig;
        }
    }
}
