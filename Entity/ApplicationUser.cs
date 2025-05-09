﻿using Entity.Live;
using Entity.Payment;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.CourseFolder;
using Entity.TeacherFolder;
using System.Text.Json.Serialization;

namespace Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string? ImageURL { get; set; }
        public DateTime? DOB { get; set; }
        public double AverageRating { get; set; } = 0;
        public int RatingCount { get; set; } = 0;
        public string? LockoutReason { get; set; }
        public bool? LockoutForever { get; set; }
        public UserEducation? UserEducation { get; set; }
        public TeacherRequest? TeacherRequest { get; set; }
        public ICollection<Event>? Events { get; set; }
        public ICollection<Course>? Courses { get; set; }
        public ICollection<Enrollment>? Enrollments { get; set; }
        public ICollection<TeacherAvailableSchedule>? TeacherAvailableSchedules { get; set; }
        public ICollection<BookedTeacherSession>? BookedTeacherSessions { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
        [JsonIgnore]
        public ICollection<LiveStream>? LiveStreams { get; set; }
        public ICollection<User_Ticket>? User_Tickets { get; set; }
        public AccountBalance? AccountBalances { get; set; }
        public ICollection<User_Gift>? SentGifts { get; set; }
        public ICollection<User_Gift>? ReceivedGifts { get; set; }
        public ICollection<TeacherRating> TeacherRatings { get; set; } = new List<TeacherRating>();
    }
}
