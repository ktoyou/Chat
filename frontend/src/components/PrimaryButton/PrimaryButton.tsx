import { MouseEventHandler, ReactElement } from "react";
import styles from "./PrimaryButton.module.css";

interface IPrimaryButtonProps {
  text?: string;
  onClick?: MouseEventHandler;
}

const PrimaryButton = ({
  text,
  onClick,
}: IPrimaryButtonProps): ReactElement => {
  return (
    <button onClick={onClick} className={`${styles.button}`}>
      <span className={`${styles.button_text}`}>{text}</span>
    </button>
  );
};

export default PrimaryButton;
