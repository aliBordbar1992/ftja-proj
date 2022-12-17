namespace Sample.Ado;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entity) : base($"Entity {entity} not found")
    {

    }
}