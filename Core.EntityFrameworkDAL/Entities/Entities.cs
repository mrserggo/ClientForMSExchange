namespace MyClient.Core.Entities
{
    /// <summary>
    /// The enteties.
    /// </summary>
    public class Enteties
    {
        public class EmailSubject
        {
            public string Subject { get; set; }

            public string Date { get; set; }
        }

        public class SubjectWithId
        {
            public string Subject { get; set; }
            public string Id { get; set; }
            public string DateCreation { get; set; }
        }

        public class MailItem
        {
            public string Id { get; set; }

            public string From;

            public string[] Recipients;
            public string Subject;
            public string Body;
        }
    }
}
