import { ReactElement, useContext } from "react";
import IRoom from "../../../types/IRoom";
import styles from "./Room.module.css";
import PrimaryButton from "../../PrimaryButton/PrimaryButton";
import WebSocketContext from "../../../context/WebSocketContext";
import Cookies from "js-cookie";

interface IRoomProps {
  room: IRoom;
}

const Room = ({ room }: IRoomProps): ReactElement => {
  const context = useContext(WebSocketContext.wsContext);

  const joinRoomHandler = () => {
    context.invoke("JoinRoom", Cookies.get("name"), room.id);
  };

  return (
    <div className={styles.room}>
      <div>
        <label className={styles.room_name}>Комната: </label>
        <span className={styles.room_name}>{room.name}</span>
      </div>
      <PrimaryButton onClick={joinRoomHandler} text="Зайти в комнату" />
      <p className={styles.room_online}>В онлайне: {room.users.length}</p>
    </div>
  );
};

export default Room;
