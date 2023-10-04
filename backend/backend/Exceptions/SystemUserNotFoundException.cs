namespace backend.Exceptions;

public class SystemUserNotFoundException : Exception 
{
    public SystemUserNotFoundException() : base("System user not found in database. Please add system user.") { }
}