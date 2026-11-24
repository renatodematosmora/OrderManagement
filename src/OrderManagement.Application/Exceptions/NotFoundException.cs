namespace OrderManagement.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entity, Guid id)
        : base($"{entity} com Id {id} não foi encontrado.") { }
}