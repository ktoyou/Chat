import { ChangeEventHandler, ReactElement } from "react";
import styles from "./PrimaryInput.module.css";

interface IPrimaryInputProps {
  placeholder: string;
  onChange: Function;
}

const PrimaryInput = ({
  placeholder,
  onChange,
}: IPrimaryInputProps): ReactElement => {
  return (
    <input
      onChange={(e) => onChange(e.target.value)}
      className={styles.input}
      placeholder={placeholder}
    />
  );
};

export default PrimaryInput;
