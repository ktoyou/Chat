import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { createContext } from "react";

const connection = new HubConnectionBuilder().withUrl("").build();

connection.start().then(() => {
  console.log("Connected to WS");
});

const wsContext = createContext<HubConnection>(connection);

export default { wsContext, connection };
