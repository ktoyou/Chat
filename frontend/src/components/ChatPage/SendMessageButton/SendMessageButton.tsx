import { MouseEventHandler, ReactElement } from "react";
import { BsSendFill } from "react-icons/bs";
import styles from "./SendMessageButton.module.css";

interface ISendMessageButtonProps {
  onClick?: MouseEventHandler;
}

const SendMessageButton = ({
  onClick,
}: ISendMessageButtonProps): ReactElement => {
  return (
    <button className={styles.button} onClick={onClick}>
      <BsSendFill color="white" size={32} />
    </button>
  );
};

export default SendMessageButton;
