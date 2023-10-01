import { ReactElement } from "react";
import { BsPlusSquare } from "react-icons/bs";
import styles from "./AddRoomBox.module.css";
import IAddRoomBoxProps from "./AddRoomBox.props";

const AddRoomBox = ({ addRoomHandler }: IAddRoomBoxProps): ReactElement => {
  return (
    <a onClick={() => addRoomHandler()} href="#" className={styles.room}>
      <BsPlusSquare size={40} color="white" />
    </a>
  );
};

export default AddRoomBox;
