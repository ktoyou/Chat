import { ReactElement, MouseEventHandler } from "react";
import { FaRegSmile } from "react-icons/fa";

interface IEmojiButtonProps {
  onOpenEmojiPanel: MouseEventHandler;
}

const EmojiButton = ({ onOpenEmojiPanel }: IEmojiButtonProps): ReactElement => {
  return (
    <div>
      <button onClick={onOpenEmojiPanel}>
        <FaRegSmile color="white" size={32} />
      </button>
    </div>
  );
};

export default EmojiButton;
