namespace SNET.Framework.Domain.Entities;

public class CrudOperation
{
    public CrudOperation(int id, int code, string name)
    {
        Id = id;
        Code = code;
        Name = name;
    }
    public int Id { get; private set; }
    public int Code { get; private set; }
    public string Name { get; private set; }
    public virtual ICollection<Auditory> Auditory { get; set; }
    public void Update(int code, string name)
    {
        Code = code;
        Name = name;
    }
}
