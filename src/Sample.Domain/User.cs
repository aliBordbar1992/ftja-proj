namespace Sample.Domain
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public byte Age { get; set; }
        public DateTime CreateTime { get; private set; }
        public double Credit { get; private set; }

        private User() { }

        public User(Guid id) : this(id, 0) { }

        public User(Guid id, double credit)
        {
            Id = id;
            Name = "";
            Credit = credit;
            CreateTime = DateTime.Now;
        }

        public void AddCredit(double credit)
        {
            if (credit < 0)
                throw new ArgumentException(nameof(credit));

            Credit += credit;
        }
    }
}