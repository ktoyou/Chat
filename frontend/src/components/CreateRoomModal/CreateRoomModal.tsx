import { ReactElement, useState } from "react";
import Modal from "../Modal/Modal";
import PrimaryInput from "../PrimaryInput/PrimaryInput";
import PrimaryButton from "../PrimaryButton/PrimaryButton";
import ICreateRoomModal from "./CreateRoomModal.props";

const CreateRoomModal = ({
  createRoomHandler,
  setModalState,
}: ICreateRoomModal): ReactElement => {
  const [roomName, setRoomName] = useState<string>("");

  return (
    <Modal onClose={() => setModalState(false)} header="Создание комнаты">
      <PrimaryInput
        onChange={(value: string) => setRoomName(value)}
        placeholder="Имя комнаты.."
      />
      <PrimaryButton
        onClick={() => createRoomHandler(roomName)}
        text="Создать"
      />
    </Modal>
  );
};

export default CreateRoomModal;
