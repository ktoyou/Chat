import { MouseEventHandler, ReactElement } from "react";
import { BiExit } from "react-icons/bi";

interface ILeaveRoomButtonProps {
  onClick: MouseEventHandler;
}

const LeaveRoomButton = ({ onClick }: ILeaveRoomButtonProps): ReactElement => {
  return (
    <div>
      <button onClick={onClick}>
        <BiExit color="white" size={32}></BiExit>
      </button>
    </div>
  );
};

export default LeaveRoomButton;
