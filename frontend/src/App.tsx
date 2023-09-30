import { ReactElement, useState, useContext } from "react";
import Layout from "./components/Layout/Layout";
import LoginForm from "./components/LoginForm/LoginForm";
import RoomsPage from "./components/RoomsPage/RoomsPage";
import ChatPage from "./components/ChatPage/ChatPage";
import WebSocketContext from "./context/WebSocketContext";
import Cookies from "js-cookie";
import ClientContext from "./context/ClientContext";
import IRoom from "./types/IRoom";

const App = (): ReactElement => {
  const [logged, setLogged] = useState<boolean>(false);
  const [currentRoom, setCurrentRoom] = useState<IRoom | null>(null);
  const clientContext = useContext(ClientContext.Context);

  clientContext.onRoomJoined = (room: IRoom) => {
    setCurrentRoom(room);
  };

  clientContext.onRoomLeave = () => {
    setCurrentRoom(null);
  };

  clientContext.onLoginSuccess = (login: string) => {
    setLogged(true);
    Cookies.set("name", login);
  };

  clientContext.onLoginError = () => {
    setLogged(false);
    Cookies.remove("name");
  };

  if (logged && currentRoom === null) {
    return (
      <WebSocketContext.wsContext.Provider value={WebSocketContext.connection}>
        <ClientContext.Context.Provider value={ClientContext.Client}>
          <Layout>
            <RoomsPage />
          </Layout>
        </ClientContext.Context.Provider>
      </WebSocketContext.wsContext.Provider>
    );
  } else if (!logged) {
    return (
      <WebSocketContext.wsContext.Provider value={WebSocketContext.connection}>
        <ClientContext.Context.Provider value={ClientContext.Client}>
          <Layout>
            <LoginForm />
          </Layout>
        </ClientContext.Context.Provider>
      </WebSocketContext.wsContext.Provider>
    );
  } else {
    return (
      <WebSocketContext.wsContext.Provider value={WebSocketContext.connection}>
        <ClientContext.Context.Provider value={ClientContext.Client}>
          <Layout>{currentRoom && <ChatPage room={currentRoom} />}</Layout>
        </ClientContext.Context.Provider>
      </WebSocketContext.wsContext.Provider>
    );
  }
};

export default App;
