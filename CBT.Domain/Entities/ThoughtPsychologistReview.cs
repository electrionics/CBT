﻿using CBT.Domain.Entities.Base;

namespace CBT.Domain.Entities
{
    public class ThoughtPsychologistReview : TrackingEntity
    {
        public int ThoughtId { get; set; }

        public int PsychologistId { get; set; }

        public string RationalAnswerComment { get; set; }

        public bool SentBack { get; set; }

        public AutomaticThought Thought {  get; set; }

        public Psychologist Psychologist { get; set; }
    }
}
