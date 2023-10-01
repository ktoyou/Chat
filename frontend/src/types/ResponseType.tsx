enum ApiResponseType {
  WithoutErrors = 0,
  LoginExists = 1,
  UserNotFound = 2,
  RoomNotFound = 3,
  RoomFull = 4,
  RoomCreated = 5,
  RoomExists = 6,
}

export default ApiResponseType;
