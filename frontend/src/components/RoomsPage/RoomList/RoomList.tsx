import { ReactElement, useContext } from "react";
import IRoom from "../../../types/IRoom";
import Room from "../Room/Room";
import styles from "./RoomList.module.css";
import ApiResponseType from "../../../types/ResponseType";
import WebSocketContext from "../../../context/WebSocketContext";
import ClientContext from "../../../context/ClientContext";

interface IRoomListProps {
  rooms: IRoom[];
}

const RoomList = ({ rooms }: IRoomListProps): ReactElement => {
  const context = useContext(WebSocketContext.wsContext);
  const clientContext = useContext(ClientContext.Context);

  context.on("JoinRoom_Receive", (status: ApiResponseType, data) => {
    if (status == ApiResponseType.UserNotFound) {
      //Todo: Выводим ошибку
    } else if (status == ApiResponseType.RoomNotFound) {
      //Todo: Выводим ошибку
    } else if (status == ApiResponseType.RoomFull) {
      //Todo: Выводим ошибку
    } else {
      const room = JSON.parse(data);
      clientContext.onRoomJoined(room);
    }
  });

  return (
    <div>
      {rooms.map((room) => (
        <Room key={room.id} room={room} />
      ))}
    </div>
  );
};

export default RoomList;
