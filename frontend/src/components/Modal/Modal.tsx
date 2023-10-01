import { ReactElement } from "react";
import { AiOutlineClose } from "react-icons/ai";
import styles from "./Modal.module.css";
import IModalProps from "./Modal.props";

const Modal = ({ children, header, onClose }: IModalProps): ReactElement => {
  return (
    <div className={styles.modal_layout}>
      <div className={styles.modal}>
        <div className={styles.modal_title}>
          {header}
          <a onClick={() => onClose()} className={styles.modal_close}>
            <AiOutlineClose color="white" size={32} />
          </a>
        </div>
        <div>{children}</div>
      </div>
    </div>
  );
};

export default Modal;
