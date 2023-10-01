import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { createContext } from "react";
import config from "../config";

const connection = new HubConnectionBuilder()
  .withUrl(`http://${config.ip}:${config.port}/chat`)
  .build();

connection.start().then(() => {
  console.log("Connected to WS");
});

const wsContext = createContext<HubConnection>(connection);

export default { wsContext, connection };
