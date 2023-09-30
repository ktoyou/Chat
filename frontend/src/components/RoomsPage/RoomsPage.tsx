import { ReactElement, useContext, useEffect, useState } from "react";
import styles from "./RoomsPage.module.css";
import RoomList from "./RoomList/RoomList";
import WebSocketContext from "../../context/WebSocketContext";
import IRoom from "../../types/IRoom";

const RoomsPage = (): ReactElement => {
  const context = useContext(WebSocketContext.wsContext);
  const [rooms, setRooms] = useState<IRoom[]>([]);

  context.on("GetRooms_Receive", (data) => {
    setRooms(JSON.parse(data));
  });

  useEffect(() => {
    context.invoke("GetRooms");
  }, []);

  return (
    <div className={styles.rooms_layout}>
      <RoomList rooms={rooms} />
    </div>
  );
};

export default RoomsPage;
