import { ReactElement, useContext, useEffect, useState } from "react";
import styles from "./RoomsPage.module.css";
import RoomList from "./RoomList/RoomList";
import WebSocketContext from "../../context/WebSocketContext";
import IRoom from "../../types/IRoom";
import AddRoomBox from "./AddRoomBox/AddRoomBox";
import CreateRoomModal from "../CreateRoomModal/CreateRoomModal";
import ApiResponseType from "../../types/ResponseType";
import Cookies from "js-cookie";
import Skeleton, { SkeletonTheme } from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";

const RoomsPage = (): ReactElement => {
  const [rooms, setRooms] = useState<IRoom[]>([]);
  const [loadingRooms, setLoadingRooms] = useState<boolean>(true);
  const [openCreateRoomModal, setOpenCreateRoomModal] =
    useState<boolean>(false);

  const context = useContext(WebSocketContext.wsContext);

  context.on("GetRooms_Receive", (data) => {
    setRooms(JSON.parse(data));
    setLoadingRooms(false);
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
        {!loadingRooms ? (
          <div>
            <RoomList rooms={rooms} />
            <AddRoomBox addRoomHandler={() => setOpenCreateRoomModal(true)} />
            {openCreateRoomModal && (
              <CreateRoomModal
                setModalState={setOpenCreateRoomModal}
                createRoomHandler={createRoomHandler}
              />
            )}
          </div>
        ) : (
          <SkeletonTheme baseColor="#11192f" highlightColor="#151e34">
            <p className={styles.rooms_layout_skeleton}>
              <Skeleton width={225} height={128} />
            </p>
            <p className={styles.rooms_layout_skeleton}>
              <Skeleton width={225} height={128} />
            </p>
          </SkeletonTheme>
        )}
      </div>
    </div>
  );
};

export default RoomsPage;
