import { ReactElement } from "react";
import IMessage from "../../../types/IMessage";
import Message from "../Message/Message";
import styles from "./MessagesList.module.css";
import Cookies from "js-cookie";

interface IMessagesListProps {
  messages: IMessage[];
}

const MessagesList = ({ messages }: IMessagesListProps): ReactElement => {
  return (
    <div className={styles.messages_list}>
      {messages.map((msg) => (
        <Message
          isMyMessage={msg.from.id === Cookies.get("id")}
          key={msg.id}
          message={msg}
        />
      ))}
    </div>
  );
};

export default MessagesList;
