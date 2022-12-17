namespace Sample.Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte Age { get; set; }
        public DateTime CreateTime { get; set; }
        public double Credit { get; set; }
    }
}