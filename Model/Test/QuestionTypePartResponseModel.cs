﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class QuestionTypePartResponseModel
    {
        public string QuestionGuide { get; set; }
        public QuestionTypeENum QuestionType { get; set; }
        public List<QuestionResponseModel> Questions { get; set; } = new List<QuestionResponseModel>();
    }
}
