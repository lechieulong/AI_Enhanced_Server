using AutoMapper;
using Entity;
using Entity.CourseFolder;
using Entity.Live;
using Entity.Test;
using Model;
using Model.Live;
using Model.Payment;
using Model.Test;
using Model.Course;
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
                //config.CreateMap<ApplicationUser, UserDto>();
                config.CreateMap<ApplicationUser, UserDto>()
                    .ForMember(dest => dest.UserEducationDto,
                        opt => opt.MapFrom(src => src.UserEducation));

                config.CreateMap<UserDto, ApplicationUser>();
                config.CreateMap<UpdateProfileDto, ApplicationUser>();
                config.CreateMap<ApplicationUser, UpdateProfileDto>();
                config.CreateMap<UserFromFileDto, ApplicationUser>();
                config.CreateMap<ApplicationUser, UserFromFileDto>();
                config.CreateMap<EventDto, Event>();
                config.CreateMap<Event, EventDto>();
                config.CreateMap<TeacherAvailableSchedule, TeacherAvailableScheduleDto>();
                config.CreateMap<TeacherAvailableScheduleDto, TeacherAvailableSchedule>();
                config.CreateMap<TeacherAvailableSchedule, UpdateScheduleDto>();
                config.CreateMap<UpdateScheduleDto, TeacherAvailableSchedule>();
                config.CreateMap<BookedTeacherSession, BookedTeacherSessionDto>();
                config.CreateMap<BookedTeacherSessionDto, BookedTeacherSession>();
                config.CreateMap<BookedTeacherSession, GetSessionsByUserIdDto>();
                config.CreateMap<GetSessionsByUserIdDto, BookedTeacherSession>();
                config.CreateMap<TestExam, TestModel>();

                config.CreateMap<Question, QuestionResponse>()
               .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));
                config.CreateMap<Answer, AnswerResponse>();

                config.CreateMap<SkillDto, Skill>()
                    .ForMember(dest => dest.Parts, opt => opt.MapFrom(src => src.Parts))
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type))
                    .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration));

                config.CreateMap<PartDto, Part>()
                    .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.Sections));

                config.CreateMap<SectionDto, Section>()
                    .ForMember(dest => dest.SectionQuestions, opt => opt.Ignore());

                config.CreateMap<QuestionDto, Question>()
                    .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));
                config.CreateMap<AnswerDto, Answer>();

                config.CreateMap<UserEducation, UserEducationDto>()
                    .ForMember(dest => dest.SpecializationIds,
                       opt => opt.MapFrom(src => src.Specializations.Select(s => s.Id).ToList()));
                config.CreateMap<UserEducationDto, UserEducation>()
                    .ForMember(dest => dest.Specializations, opt => opt.MapFrom(src =>
                        src.SpecializationIds.Select(id => new Specialization { Id = id }).ToList()));
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
                config.CreateMap<Gift, GiftModel>();
                config.CreateMap<GiftModel, Gift>();
                config.CreateMap<User_Gift, User_GiftModel>();
                config.CreateMap<User_GiftModel, User_Gift>();
                config.CreateMap<User_Ticket, User_TicketModel>();
                config.CreateMap<User_TicketModel, User_Ticket>();

                //Course
                config.CreateMap<Course, GetCourseListDto>()
                    .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.User.Name))
                    .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
                    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id));
                config.CreateMap<GetCourseListDto, Course>();
            });
            return mappingConfig;
        }
    }
}
