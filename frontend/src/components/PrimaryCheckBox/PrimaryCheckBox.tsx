import { ReactElement, useState } from "react";
import IPrimaryCheckBoxProps from "./PrimaryCheckBox.props";
import styles from "./PrimaryCheckBox.module.css";

const PrimaryCheckBox = ({
  placeholder,
  disabled,
  ...props
}: IPrimaryCheckBoxProps): ReactElement => {
  return (
    <div className={styles.primary_checkbox}>
      <p className={styles.primary_checkbox_placeholder}>{placeholder}</p>
      <input type="checkbox" {...props} />
    </div>
  );
};

export default PrimaryCheckBox;
