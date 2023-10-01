import IMessage from "./IMessage";
import IUser from "./IUser";

interface IRoom {
  name: string;
  id: number;
  users: IUser[];
  messages: IMessage[];
}

export default IRoom;
