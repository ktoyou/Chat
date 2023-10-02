import IMessage from "./IMessage";
import IUser from "./IUser";

interface IRoom {
  name: string;
  maxUsers: number;
  id: string;
  creator: IUser;
  users: IUser[];
  messages: IMessage[];
}

export default IRoom;
