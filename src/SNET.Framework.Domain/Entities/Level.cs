namespace SNET.Framework.Domain.Entities;

public class Level
{
    public int Id { get; private set; }
    public int Code { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public virtual ICollection<Auditory> Auditory { get; set; }

    public Level(int id, int code, string name, string description)
    {
        Id = id;
        Code = code;
        Name = name;
        Description = description;
    }

    public void Update(int code, string name, string description)
    {
        Code = code;
        Name = name;
        Description = description;
    }
}
