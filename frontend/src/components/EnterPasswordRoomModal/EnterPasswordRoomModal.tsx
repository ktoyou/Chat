import { ReactElement, useState } from "react";
import Modal from "../Modal/Modal";
import IEnterPasswordRoomModalProps from "./EnterPasswordRoomModal.props";
import PrimaryInput from "../PrimaryInput/PrimaryInput";
import PrimaryButton from "../PrimaryButton/PrimaryButton";

const EnterPasswordRoomModal = ({
  setOpened,
  onEnterPassword,
  forRoom,
}: IEnterPasswordRoomModalProps): ReactElement => {
  const [password, setPassword] = useState<string>("");

  return (
    <Modal
      onClose={() => setOpened(false)}
      header={`Ввод пароля для комнаты <<${forRoom?.name}>>`}
    >
      <PrimaryInput
        onChange={(e) => setPassword(e.target.value)}
        placeholder="Пароль"
        type="password"
      />
      <PrimaryButton onClick={() => onEnterPassword(password)} text="Вход" />
    </Modal>
  );
};

export default EnterPasswordRoomModal;
