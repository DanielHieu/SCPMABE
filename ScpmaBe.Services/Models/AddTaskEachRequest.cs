namespace ScpmaBe.Services.Models
{
    public class AddTaskEachRequest
    {
        public string Title { get; set; }
        public int AssignedToId { get; set; }

        public string Priority { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Description { get; set; }
    }
}
