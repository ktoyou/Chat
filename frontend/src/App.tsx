import { ReactElement, useState, useContext, useEffect } from "react";
import Layout from "./components/Layout/Layout";
import LoginForm from "./components/LoginForm/LoginForm";
import RoomsPage from "./components/RoomsPage/RoomsPage";
import ChatPage from "./components/ChatPage/ChatPage";
import WebSocketContext from "./context/WebSocketContext";
import Cookies from "js-cookie";
import ClientContext from "./context/ClientContext";
import IRoom from "./types/IRoom";
import IUser from "./types/IUser";
import ApiResponseType from "./types/ResponseType";

const App = (): ReactElement => {
  const [logged, setLogged] = useState<boolean>(false);
  const [currentRoom, setCurrentRoom] = useState<IRoom | null>(null);
  const wsContext = useContext(WebSocketContext.wsContext);
  const clientContext = useContext(ClientContext.Context);

  wsContext.on("ConnectUser_Receive", (status: ApiResponseType) => {
    if (status == ApiResponseType.UserExists) {
      setLogged(true);
    } else {
      setLogged(false);
    }
  });

  clientContext.onRoomJoined = (room: IRoom) => {
    setCurrentRoom(room);
  };

  clientContext.onRoomLeave = () => {
    setCurrentRoom(null);
  };

  clientContext.onLoginSuccess = (user: IUser) => {
    setLogged(true);
    Cookies.set("id", user.id);
  };

  clientContext.onLoginError = () => {
    setLogged(false);
    Cookies.remove("id");
  };

  let content;
  if (logged && currentRoom === null) content = <RoomsPage />;
  else if (!logged) content = <LoginForm />;
  else content = currentRoom && <ChatPage room={currentRoom} />;

  return (
    <WebSocketContext.wsContext.Provider value={WebSocketContext.connection}>
      <ClientContext.Context.Provider value={ClientContext.Client}>
        <Layout>{content}</Layout>
      </ClientContext.Context.Provider>
    </WebSocketContext.wsContext.Provider>
  );
};

export default App;
