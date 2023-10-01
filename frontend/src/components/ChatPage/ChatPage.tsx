import {
  ReactElement,
  useContext,
  useState,
  useEffect,
  KeyboardEvent,
} from "react";
import styles from "./ChatPage.module.css";
import SendMessageButton from "./SendMessageButton/SendMessageButton";
import MessagesList from "./MessagesList/MessagesList";
import WebSocketContext from "../../context/WebSocketContext";
import IRoom from "../../types/IRoom";
import Cookies from "js-cookie";
import IMessage from "../../types/IMessage";
import LeaveRoomButton from "./LeaveRoomButton/LeaveRoomButton";
import ApiResponseType from "../../types/ResponseType";
import ClientContext from "../../context/ClientContext";
import EmojiPicker, { EmojiClickData } from "emoji-picker-react";
import EmojiButton from "./EmojiButton/EmojiButton";

interface IChatPageProps {
  room: IRoom;
}

const ChatPage = ({ room }: IChatPageProps): ReactElement => {
  const [messages, setMessages] = useState<IMessage[]>([]);
  const [messageContent, setMessageContent] = useState<string>("");
  const [emojiPanelOpen, setEmojiPanelOpen] = useState<boolean>(false);
  const context = useContext(WebSocketContext.wsContext);
  const clientContext = useContext(ClientContext.Context);

  context.on("SendMessageToRoom_Receive", (data) => {
    const message: IMessage = JSON.parse(data);
    setMessages([...messages, message]);
  });

  context.on("GetMessagesFromRoom_Receive", (messages) => {
    setMessages(JSON.parse(messages));
  });

  context.on("LeaveRoom_Receive", (status: ApiResponseType) => {
    if (status == ApiResponseType.UserNotFound) {
      //Todo: Обработка ошибки
    } else if (status == ApiResponseType.RoomNotFound) {
      //Todo: Обработка ошибки
    } else {
      clientContext.onRoomLeave();
    }
  });

  const sendMessageHandler = () => {
    context.invoke(
      "SendMessageToRoom",
      room.id,
      Cookies.get("name"),
      messageContent
    );
    setMessageContent("");
  };

  const onEmojiClick = (e: EmojiClickData) => {
    setMessageContent(messageContent + e.emoji);
  };

  const onKeyDownMessageInput = (e: KeyboardEvent) => {
    if (e.code === "Enter") sendMessageHandler();
  };

  const leaveRoomHandler = () => {
    context.invoke("LeaveRoom", room.id, Cookies.get("name"));
  };

  useEffect(() => {
    context.invoke("GetMessagesFromRoom", room.id);
  }, []);

  return (
    <div className={styles.chat_layout}>
      <div className={styles.chat_layout_send_message_block}>
        {emojiPanelOpen && (
          <div className="absolute bottom-20">
            <EmojiPicker onEmojiClick={onEmojiClick} />
          </div>
        )}
        <EmojiButton
          onOpenEmojiPanel={() => setEmojiPanelOpen(!emojiPanelOpen)}
        />
        <input
          placeholder="Написать сообщение..."
          value={messageContent}
          onKeyDown={onKeyDownMessageInput}
          onChange={(e) => setMessageContent(e.target.value)}
          className={styles.chat_layout_send_message_block_input}
        />
        <SendMessageButton onClick={sendMessageHandler} />
        <LeaveRoomButton onClick={leaveRoomHandler} />
      </div>
      <div className={styles.chat_layout_messages}>
        <MessagesList messages={messages} />
      </div>
    </div>
  );
};

export default ChatPage;
