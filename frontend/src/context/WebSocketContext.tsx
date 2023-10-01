import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { createContext } from "react";

const connection = new HubConnectionBuilder()
  .withUrl("http://46.175.122.36:5013/chat")
  .build();

connection.start().then(() => {
  console.log("Connected to WS");
});

const wsContext = createContext<HubConnection>(connection);

export default { wsContext, connection };
