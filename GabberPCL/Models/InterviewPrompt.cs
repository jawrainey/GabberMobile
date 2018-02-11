using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GabberPCL.Models
{
    public class InterviewPrompt
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int Start { get; set; }
        public int End { get; set; }

        [ForeignKey(typeof(Prompt))]
        public int PromptID { get; set; }
        [ForeignKey(typeof(InterviewSession))]
        public string InterviewID { get; set; }

        // Computing End must exist as we cannot update it once inserted as we do not know iff
        // one topic will be selected or how long the interview will last
        public static void ComputeEndForAllAnnotationsInSession(int _LengthOfInterviewInSeconds)
        {
            var AnnotationsForLastSession = Queries.AnnotationsForLastSession();

            // Only one annotation was chosen for the entire recording
            if (AnnotationsForLastSession.Count == 1)
            {
                // Although start is recorded, the first can be off by a few seconds due to threads, etc.
                AnnotationsForLastSession[0].Start = 0;
                AnnotationsForLastSession[0].End = _LengthOfInterviewInSeconds;
                Queries.UpdateAnnotation(AnnotationsForLastSession[0]);
            }
            // This ensures if two (or more) annotations are made then the first and last will be updated correctly
            if (AnnotationsForLastSession.Count > 1)
            {
                AnnotationsForLastSession[0].Start = 0;
                // By default, the end time for the first annotation 
                AnnotationsForLastSession[0].End = AnnotationsForLastSession[1].Start;
                Queries.UpdateAnnotation(AnnotationsForLastSession[0]);

                // The last annotation continues until the end of the audio
                var LastAnnotation = AnnotationsForLastSession[AnnotationsForLastSession.Count - 1];
                LastAnnotation.End = _LengthOfInterviewInSeconds;
                Queries.UpdateAnnotation(LastAnnotation);
            }

            // ensures the end time between the first and last annotations are correct
            if (AnnotationsForLastSession.Count > 2)
            {
                // Start at the second and go to the second from last
                // Skip the last element as it is equal to seconds ...
                for (int i = 1; i < AnnotationsForLastSession.Count - 1; i++)
                {
                    AnnotationsForLastSession[i].End = AnnotationsForLastSession[i + 1].Start;
                    Queries.UpdateAnnotation(AnnotationsForLastSession[i]);
                }
            }
        }
    }
}