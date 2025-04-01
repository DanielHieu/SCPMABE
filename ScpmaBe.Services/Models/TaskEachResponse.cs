namespace ScpmaBe.Services.Models
{
    public class TaskEachResponse
    {
        public int TaskEachId { get; set; }

        public int AssignedToId { get; set; }
        public string AssigneeName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
