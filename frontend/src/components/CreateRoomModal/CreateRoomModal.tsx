import { ReactElement, useState } from "react";
import Modal from "../Modal/Modal";
import PrimaryInput from "../PrimaryInput/PrimaryInput";
import PrimaryButton from "../PrimaryButton/PrimaryButton";
import ICreateRoomModal from "./CreateRoomModal.props";
import PrimaryCheckBox from "../PrimaryCheckBox/PrimaryCheckBox";

const CreateRoomModal = ({
  createRoomHandler,
  setModalState,
}: ICreateRoomModal): ReactElement => {
  const [roomName, setRoomName] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [withPassword, setWithPassword] = useState<boolean>(false);

  return (
    <Modal onClose={() => setModalState(false)} header="Создание комнаты">
      <PrimaryInput
        onChange={(e) => setRoomName(e.target.value)}
        placeholder="Имя комнаты.."
      />
      <PrimaryInput
        disabled={!withPassword}
        onChange={(e) => setPassword(e.target.value)}
        placeholder="Пароль.."
      />
      <PrimaryCheckBox
        placeholder="Комната с паролем?"
        onChange={(e) => setWithPassword(e.target.checked)}
      />
      <PrimaryButton
        onClick={() => createRoomHandler(roomName, withPassword, password)}
        text="Создать"
      />
    </Modal>
  );
};

export default CreateRoomModal;
