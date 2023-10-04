import { MouseEventHandler, ReactElement } from "react";
import styles from "./PrimaryButton.module.css";
import IPrimaryButtonProps from "./PrimaryButton.props";

const PrimaryButton = ({
  text,
  onClick,
  ...props
}: IPrimaryButtonProps): ReactElement => {
  return (
    <button onClick={onClick} className={`${styles.button}`} {...props}>
      <span className={`${styles.button_text}`}>{text}</span>
    </button>
  );
};

export default PrimaryButton;
