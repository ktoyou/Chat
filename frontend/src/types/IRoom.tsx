import IMessage from "./IMessage";
import IUser from "./IUser";

interface IRoom {
  id: string;
  name: string;
  maxUsers: number;
  withPassword: boolean;
  creator: IUser;
  users: IUser[];
  messages: IMessage[];
}

export default IRoom;
