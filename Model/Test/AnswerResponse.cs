﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class AnswerResponse
    {
        public Guid Id { get; set; }
        public string AnswerText { get; set; }
        public int IsCorrect { get; set; }
    }
}
