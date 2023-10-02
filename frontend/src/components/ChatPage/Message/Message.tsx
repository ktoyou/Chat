import { ReactElement } from "react";
import IMessage from "../../../types/IMessage";
import styles from "./Message.module.css";
const format = require("date-format");

interface IMessageProps {
  message: IMessage;
  isMyMessage: boolean;
}

const Message = ({ message, isMyMessage }: IMessageProps): ReactElement => {
  const dateTime = new Date(message.unixTime * 1000);
  const formattedDateTime = `${dateTime.toLocaleDateString()} ${dateTime.toLocaleTimeString()}`;

  return isMyMessage ? (
    <div className={styles.my_message}>
      <span className={styles.my_message_from}>{message.from.name}</span>
      <div className={styles.my_message_content}>
        {message.content}
        <p className={styles.message_time}>{formattedDateTime}</p>
      </div>
    </div>
  ) : (
    <div className={styles.message}>
      <span className={styles.message_from}>{message.from.name}</span>
      <div className={styles.message_content}>
        {message.content}
        <p className={styles.message_time}>{formattedDateTime}</p>
      </div>
    </div>
  );
};

export default Message;
