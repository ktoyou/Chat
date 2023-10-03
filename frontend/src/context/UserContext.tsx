import { createContext } from "react";
import IUser from "../types/IUser";

interface IUserContainer {
  user: IUser | null;
}

const UserContainer: IUserContainer = {
  user: null,
};

const UserContext = createContext<IUserContainer>(UserContainer);

export default { UserContext, UserContainer };
