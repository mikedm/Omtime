using System;

namespace Omtime
{
    public class Practice
    {
        public DateTime when { get; set; }
        public TimeSpan duration { get; set; }
        public string className { get; set; }
        public string classGenre { get; set; } // Consolidate similar class types
        public string classId { get; set; }
        public string instructor { get; set; }
        public string regularlyScheduledInstructor { get; set; }
        public string studio { get; set; }
        public string mindBodyOnlineId { get; set; }
        public bool isCancelled { get; set; }
    }
}
