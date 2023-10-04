import { ReactElement, useContext, useState } from "react";
import IRoom from "../../../types/IRoom";
import styles from "./Room.module.css";
import PrimaryButton from "../../PrimaryButton/PrimaryButton";
import WebSocketContext from "../../../context/WebSocketContext";
import Cookies from "js-cookie";
import EnterPasswordRoomModal from "../../EnterPasswordRoomModal/EnterPasswordRoomModal";

interface IRoomProps {
  room: IRoom;
}

const Room = ({ room }: IRoomProps): ReactElement => {
  const [inputPasswordModal, setInputPasswordModal] = useState<boolean>(false);

  const context = useContext(WebSocketContext.wsContext);

  const joinRoomHandlerWithPassword = (password: string) => {
    context.invoke("JoinRoom", Cookies.get("id"), room.id, password);
  };

  const joinRoomHandler = () => {
    context.invoke("JoinRoom", Cookies.get("id"), room.id, "");
  };

  return (
    <div className={styles.room}>
      <div>
        <label className={styles.room_name}>Комната: </label>
        <span className={styles.room_name}>{room.name}</span>
      </div>
      <PrimaryButton
        onClick={() => {
          room.withPassword ? setInputPasswordModal(true) : joinRoomHandler();
        }}
        text="Зайти в комнату"
      />
      <p className={styles.room_online}>
        В онлайне: {room.users.length}/{room.maxUsers}
      </p>
      <p className={styles.with_password}>
        Пароль: {room.withPassword ? "да" : "нет"}
      </p>
      {inputPasswordModal && (
        <EnterPasswordRoomModal
          onEnterPassword={joinRoomHandlerWithPassword}
          setOpened={setInputPasswordModal}
          forRoom={room}
        />
      )}
    </div>
  );
};

export default Room;
