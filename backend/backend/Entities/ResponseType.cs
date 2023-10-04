namespace backend.Entities;

public enum ResponseType
{
    WithoutErrors = 0,
    LoginExists = 1,
    UserNotFound = 2,
    RoomNotFound = 3,
    RoomFull = 4,
    RoomCreated = 5,
    RoomExists = 6,
    UserExists = 7,
    InvalidPassword = 8
}