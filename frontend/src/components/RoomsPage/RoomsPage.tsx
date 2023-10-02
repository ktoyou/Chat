import {
  ReactElement,
  useContext,
  useEffect,
  useState,
  MouseEventHandler,
} from "react";
import styles from "./RoomsPage.module.css";
import RoomList from "./RoomList/RoomList";
import WebSocketContext from "../../context/WebSocketContext";
import IRoom from "../../types/IRoom";
import AddRoomBox from "./AddRoomBox/AddRoomBox";
import CreateRoomModal from "../CreateRoomModal/CreateRoomModal";
import ApiResponseType from "../../types/ResponseType";
import Cookies from "js-cookie";

const RoomsPage = (): ReactElement => {
  const context = useContext(WebSocketContext.wsContext);
  const [rooms, setRooms] = useState<IRoom[]>([]);
  const [openCreateRoomModal, setOpenCreateRoomModal] =
    useState<boolean>(false);

  context.on("GetRooms_Receive", (data) => {
    setRooms(JSON.parse(data));
  });

  context.on("CreateRoom_Receive", (status: ApiResponseType) => {
    if (status == ApiResponseType.RoomExists) {
      //Todo: Обработка ошибки
    } else {
      setOpenCreateRoomModal(false);
    }
  });

  useEffect(() => {
    context.invoke("GetRooms");
  }, []);

  const createRoomHandler = (name: string) => {
    context.invoke("CreateRoom", Cookies.get("id"), name);
  };

  return (
    <div className={styles.rooms_layout}>
      <div>
        <h1 className={styles.rooms_layout_title}>Все комнаты</h1>
        <RoomList rooms={rooms} />
        <AddRoomBox addRoomHandler={() => setOpenCreateRoomModal(true)} />
        {openCreateRoomModal && (
          <CreateRoomModal
            setModalState={setOpenCreateRoomModal}
            createRoomHandler={createRoomHandler}
          />
        )}
      </div>
    </div>
  );
};

export default RoomsPage;
