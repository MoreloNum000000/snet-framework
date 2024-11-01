namespace SNET.Framework.Domain.Audit
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class AuditDescriptionAttribute : Attribute
    {
        public string Description { get; }
        public AuditDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
