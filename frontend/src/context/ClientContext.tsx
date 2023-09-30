import IRoom from "../types/IRoom";
import IUser from "../types/IUser";
import { createContext } from "react";

interface IClientContext {
  onRoomJoined: Function;
  onRoomLeave: Function;
  onLoginSuccess: Function;
  onLoginError: Function;
}

const Client: IClientContext = {
  onRoomJoined: Function,
  onRoomLeave: Function,
  onLoginSuccess: Function,
  onLoginError: Function,
};

const Context = createContext<IClientContext>(Client);

export default { Context, Client };
