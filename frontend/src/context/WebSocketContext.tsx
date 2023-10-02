import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { createContext } from "react";
import config from "../config";
import Cookies from "js-cookie";

const connection = new HubConnectionBuilder()
  .withUrl(`http://${config.ip}:${config.port}/chat`)
  .build();

connection.start().then(() => {
  console.log("Connected to WS");

  const userId = Cookies.get("id");
  if (userId) {
    connection.invoke("ConnectUser", Cookies.get("id"));
  }
});

const wsContext = createContext<HubConnection>(connection);

export default { wsContext, connection };
