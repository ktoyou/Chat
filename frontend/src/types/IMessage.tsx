import IUser from "./IUser";

interface IMessage {
  id: number;
  from: IUser;
  content: string;
}

export default IMessage;
