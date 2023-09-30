import { ReactElement } from "react";
import IMessage from "../../../types/IMessage";
import styles from "./Message.module.css";

interface IMessageProps {
  message: IMessage;
  isMyMessage: boolean;
}

const Message = ({ message, isMyMessage }: IMessageProps): ReactElement => {
  return isMyMessage ? (
    <div className={styles.my_message}>
      <span className={styles.my_message_from}>{message.from.name}</span>
      <div className={styles.my_message_content}>{message.content}</div>
    </div>
  ) : (
    <div className={styles.message}>
      <span className={styles.message_from}>{message.from.name}</span>
      <div className={styles.message_content}>{message.content}</div>
    </div>
  );
};

export default Message;
