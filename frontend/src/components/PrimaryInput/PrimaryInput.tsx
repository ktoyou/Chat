import { ChangeEventHandler, ReactElement } from "react";
import styles from "./PrimaryInput.module.css";
import IPrimaryInputProps from "./PrimaryInput.props";

const PrimaryInput = ({
  placeholder,
  disabled,
  ...props
}: IPrimaryInputProps): ReactElement => {
  return (
    <input
      className={disabled ? styles.input_disabled : styles.input}
      placeholder={placeholder}
      disabled={disabled}
      {...props}
    />
  );
};

export default PrimaryInput;
