import IUser from "./IUser";

interface IMessage {
  id: number;
  from: IUser;
  content: string;
  unixTime: number;
}

export default IMessage;
